using System.ComponentModel;
using System.Linq.Expressions;
using System.Text.Json;
using SegmentBackend.Models;

namespace SegmentBackend.Services
{
    public static class SegmentToExpression
    {
        private static string MapField(string field) => field switch
        {
            "country" => nameof(Models.User.Country),
            "plan" => nameof(Models.User.Plan),
            "orders_count" => nameof(Models.User.OrdersCount),
            "total_spend" => nameof(Models.User.TotalSpend),
            "created_utc" => nameof(Models.User.CreatedUtc),
            _ => throw new NotSupportedException($"Champ non supporté: {field}")
        };

        public static Expression<Func<Models.User, bool>> BuildPredicate(RuleGroup root)
        {
            var param = Expression.Parameter(typeof(User), "u");
            var body = BuildNode(root, param);
            return Expression.Lambda<Func<User, bool>>(body, param);
        }

        private static Expression BuildNode(object node, ParameterExpression param) => node switch
        {
            RuleGroup g => BuildGroup(g, param),
            Rule r => BuildRule(r, param),
            _ => throw new InvalidOperationException("Node inconnu")
        };

        private static Expression BuildGroup(RuleGroup g, ParameterExpression param)
        {
            if (g.Children.Count == 0) return Expression.Constant(true);
            var exprs = g.Children.Select(c => BuildNode(c, param)).ToList();
            Expression combined = exprs.First();
            foreach (var e in exprs.Skip(1))
            {
                combined = g.Type.ToLower() switch
                {
                    "and" => Expression.AndAlso(combined, e),
                    "or" => Expression.OrElse(combined, e),
                    "not" => Expression.AndAlso(combined, Expression.Not(e)),
                    _ => throw new NotSupportedException($"Combinator {g.Type} non supporté")
                };
            }
            return combined;
        }

        private static Expression BuildRule(Rule r, ParameterExpression param)
        {
            var prop = Expression.PropertyOrField(param, MapField(r.Field));
            var op = r.Operator.ToLowerInvariant();

            if (op == "between")
            {
                object? min = null, max = null;

                if (r.Value is JsonElement je && je.ValueKind == JsonValueKind.Object)
                {
                    if (je.TryGetProperty("min", out var minEl)) min = ConvertJsonElement(minEl, prop.Type);
                    if (je.TryGetProperty("max", out var maxEl)) max = ConvertJsonElement(maxEl, prop.Type);
                }
                else
                {
                    var minProp = r.Value.GetType().GetProperty("min");
                    var maxProp = r.Value.GetType().GetProperty("max");
                    if (minProp != null) min = ConvertObjectToType(minProp.GetValue(r.Value), prop.Type);
                    if (maxProp != null) max = ConvertObjectToType(maxProp.GetValue(r.Value), prop.Type);
                }

                Expression expr = Expression.Constant(true);
                if (min is not null)
                    expr = Expression.AndAlso(expr, Expression.GreaterThanOrEqual(prop, Expression.Constant(min, prop.Type)));
                if (max is not null)
                    expr = Expression.AndAlso(expr, Expression.LessThanOrEqual(prop, Expression.Constant(max, prop.Type)));
                return expr;
            }
            if (op == "exists")
            {
                return prop.Type == typeof(string)
                    ? Expression.AndAlso(
                        Expression.NotEqual(prop, Expression.Constant(null, typeof(string))),
                        Expression.GreaterThan(Expression.Property(prop, nameof(string.Length)), Expression.Constant(0)))
                    : Expression.NotEqual(prop, Expression.Constant(GetDefault(prop.Type), prop.Type));
            }
            if (op == "notexists")
            {
                return Expression.Not(BuildRule(new Rule { Field = r.Field, Operator = "exists", Kind = r.Kind }, param));
            }

            var (constExpr, elemType, isEnumerable) = BuildConstant(prop.Type, r.Value);

            return op switch
            {
                "eq" => Expression.Equal(prop, constExpr),
                "neq" => Expression.NotEqual(prop, constExpr),
                "lt" => Expression.LessThan(prop, constExpr),
                "lte" => Expression.LessThanOrEqual(prop, constExpr),
                "gt" => Expression.GreaterThan(prop, constExpr),
                "gte" => Expression.GreaterThanOrEqual(prop, constExpr),

                "contains" => CallString("Contains"),
                "startswith" => CallString("StartsWith"),
                "endswith" => CallString("EndsWith"),

                "in" => CallIn(),
                "nin" => Expression.Not(CallIn()),

                _ => throw new NotSupportedException($"Opérateur {r.Operator} non supporté")
            };

            Expression CallString(string method)
            {
                if (prop.Type != typeof(string)) throw new NotSupportedException($"Opérateur {op} uniquement sur string");
                if (constExpr.Type != typeof(string)) throw new NotSupportedException("Valeur string attendue");
                return Expression.Call(prop, typeof(string).GetMethod(method, new[] { typeof(string) })!, constExpr);
            }

            Expression CallIn()
            {
                if (!isEnumerable) throw new NotSupportedException("Valeur array attendue pour IN/NIN");
                var contains = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(elemType);
                return Expression.Call(contains, constExpr, prop);
            }
        }

        private static (Expression constExpr, Type elemType, bool isEnumerable) BuildConstant(Type targetType, object? value)
        {
            if (value is null) return (Expression.Constant(null, targetType), targetType, false);

            // JSON 
            if (value is JsonElement je)
            {
                if (je.ValueKind == JsonValueKind.Array)
                {
                    var list = ToList(je, targetType, out var elemType);
                    return (Expression.Constant(list, list.GetType()), elemType, true);
                }
                else
                {
                    var converted = ConvertJsonElement(je, targetType);
                    return (Expression.Constant(converted, targetType), targetType, false);
                }
            }

            var nonNullable = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (!(value is string) && value is System.Collections.IEnumerable rawEnum)
            {
                var elemType = nonNullable; 
                var materialized = Materialize(rawEnum, elemType);
                return (Expression.Constant(materialized, materialized.GetType()), elemType, true);
            }

            // Enum
            if (nonNullable.IsEnum)
            {
                var enumVal = ConvertToEnum(nonNullable, value);
                return (Expression.Constant(enumVal, targetType), nonNullable, false);
            }

            // Types spéciaux
            if (nonNullable == typeof(Guid))
            {
                var guid = value is Guid g ? g : Guid.Parse(value.ToString()!);
                return (Expression.Constant(guid, targetType), nonNullable, false);
            }
            if (nonNullable == typeof(DateTimeOffset))
            {
                var dto = value is DateTimeOffset d0 ? d0 : DateTimeOffset.Parse(value.ToString()!);
                return (Expression.Constant(dto, targetType), nonNullable, false);
            }
            if (nonNullable == typeof(TimeSpan))
            {
                var ts = value is TimeSpan ts0 ? ts0 : TimeSpan.Parse(value.ToString()!);
                return (Expression.Constant(ts, targetType), nonNullable, false);
            }
            if (nonNullable == typeof(DateTime))
            {
                var dt = value is DateTime d ? d : DateTime.Parse(value.ToString()!);
                return (Expression.Constant(dt, targetType), nonNullable, false);
            }

            // TypeConverter (ex: string -> type)
            var tc = TypeDescriptor.GetConverter(nonNullable);
            if (tc is not null && tc.CanConvertFrom(value.GetType()))
            {
                var converted = tc.ConvertFrom(value)!;
                return (Expression.Constant(converted, targetType), nonNullable, false);
            }

            // IConvertible → ChangeType
            if (value is IConvertible)
            {
                var convertedValue = Convert.ChangeType(value, nonNullable);
                return (Expression.Constant(convertedValue, targetType), nonNullable, false);
            }

            try
            {
                var json = value is string s ? s : JsonSerializer.Serialize(value);
                var des = JsonSerializer.Deserialize(json, nonNullable);
                return (Expression.Constant(des, targetType), nonNullable, false);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Cannot convert value of type '{value.GetType()}' to '{targetType}'.", ex);
            }
        }

        private static object ConvertJsonElement(JsonElement je, Type targetType)
        {
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (je.ValueKind == JsonValueKind.Null)
            {
                if (Nullable.GetUnderlyingType(targetType) is not null) return null!;
                throw new InvalidCastException($"Cannot assign null to non-nullable '{targetType}'.");
            }

            if (t.IsEnum)
            {
                if (je.ValueKind == JsonValueKind.Number && je.TryGetInt32(out var num))
                    return Enum.ToObject(t, num);
                var str = je.ToString();
                return Enum.Parse(t, str, ignoreCase: true);
            }

            if (t == typeof(string)) return je.ToString()!;
            if (t == typeof(Guid)) return Guid.Parse(je.ToString()!);
            if (t == typeof(DateTime)) return je.ValueKind == JsonValueKind.String ? je.GetDateTime() : DateTime.Parse(je.ToString()!);
            if (t == typeof(DateTimeOffset)) return DateTimeOffset.Parse(je.ToString()!);
            if (t == typeof(TimeSpan)) return TimeSpan.Parse(je.ToString()!);
            if (t == typeof(bool)) return je.GetBoolean();

            if (t == typeof(byte)) return je.GetByte();
            if (t == typeof(sbyte)) return (sbyte)je.GetInt32();
            if (t == typeof(short)) return (short)je.GetInt32();
            if (t == typeof(ushort)) return (ushort)je.GetInt32();
            if (t == typeof(int)) return je.GetInt32();
            if (t == typeof(uint)) return je.GetUInt32();
            if (t == typeof(long)) return je.GetInt64();
            if (t == typeof(ulong)) return je.GetUInt64();
            if (t == typeof(float)) return je.GetSingle();
            if (t == typeof(double)) return je.GetDouble();
            if (t == typeof(decimal)) return je.GetDecimal();

            var json = je.GetRawText();
            return JsonSerializer.Deserialize(json, t)!;
        }

        private static object ToList(JsonElement array, Type targetScalarType, out Type elemType)
        {
            var t = Nullable.GetUnderlyingType(targetScalarType) ?? targetScalarType;
            elemType = t;
            var list = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType))!;

            if (array.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in array.EnumerateArray())
                    list.Add(ConvertJsonElement(el, elemType));
            }
            else
            {
                list.Add(ConvertJsonElement(array, elemType));
            }

            return list;
        }

        private static System.Collections.IList Materialize(System.Collections.IEnumerable rawEnum, Type elemType)
        {
            var list = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType))!;
            foreach (var v in rawEnum)
            {
                if (v is JsonElement je)
                {
                    list.Add(ConvertJsonElement(je, elemType));
                }
                else
                {
                    list.Add(ConvertObjectToType(v, elemType));
                }
            }
            return list;
        }

        private static object ConvertObjectToType(object? value, Type targetType)
        {
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (value is null)
            {
                if (Nullable.GetUnderlyingType(targetType) is not null) return null!;
                throw new InvalidCastException($"Cannot assign null to non-nullable '{targetType}'.");
            }

            if (t.IsEnum) return ConvertToEnum(t, value);

            if (t == typeof(Guid)) return value is Guid g ? g : Guid.Parse(value.ToString()!);
            if (t == typeof(DateTime)) return value is DateTime d ? d : DateTime.Parse(value.ToString()!);
            if (t == typeof(DateTimeOffset)) return value is DateTimeOffset d0 ? d0 : DateTimeOffset.Parse(value.ToString()!);
            if (t == typeof(TimeSpan)) return value is TimeSpan ts ? ts : TimeSpan.Parse(value.ToString()!);

            var tc = TypeDescriptor.GetConverter(t);
            if (tc is not null && tc.CanConvertFrom(value.GetType()))
                return tc.ConvertFrom(value)!;

            if (value is IConvertible)
                return Convert.ChangeType(value, t);

            var json = value is string s ? s : JsonSerializer.Serialize(value);
            return JsonSerializer.Deserialize(json, t)!;
        }

        private static object ConvertToEnum(Type enumType, object value)
        {
            if (value is null) throw new InvalidCastException($"Cannot convert null to enum '{enumType}'.");
            if (value is JsonElement je) return ConvertJsonElement(je, enumType);
            if (value is string s) return Enum.Parse(enumType, s, ignoreCase: true);

            var underlying = Enum.GetUnderlyingType(enumType);
            var num = (IConvertible)Convert.ChangeType(value, underlying);
            return Enum.ToObject(enumType, num);
        }

        private static object? GetDefault(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}
