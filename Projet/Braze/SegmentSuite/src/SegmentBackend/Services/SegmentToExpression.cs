
using System.Linq.Expressions;
using System.Text.Json;
using SegmentBackend.Models;

namespace SegmentBackend.Services
{
    public static class SegmentToExpression
    {
        private static string MapField(string field) => field switch
        {
            "country"       => nameof(Models.User.Country),
            "plan"          => nameof(Models.User.Plan),
            "orders_count"  => nameof(Models.User.OrdersCount),
            "total_spend"   => nameof(Models.User.TotalSpend),
            "created_utc"   => nameof(Models.User.CreatedUtc),
            _ => throw new NotSupportedException($"Champ non supporté: {field}")
        };

        public static Expression<Func<Models.User, bool>> BuildPredicate(RuleGroup root)
        {
            var param = Expression.Parameter(typeof(Models.User), "u");
            var body = BuildNode(root, param);
            return Expression.Lambda<Func<Models.User, bool>>(body, param);
        }

        private static Expression BuildNode(object node, ParameterExpression param) => node switch
        {
            RuleGroup g => BuildGroup(g, param),
            Rule r      => BuildRule(r, param),
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
                    "or"  => Expression.OrElse(combined, e),
                    "not" => Expression.AndAlso(combined, Expression.Not(e)),
                    _ => throw new NotSupportedException($"Combinator {g.Type} non supporté")
                };
            }
            return combined;
        }

        private static Expression BuildRule(Rule r, ParameterExpression param)
        {
            var prop = Expression.PropertyOrField(param, MapField(r.Field));
            var op   = r.Operator.ToLowerInvariant();

            if (op == "between")
            {
                if (r.Value is not JsonElement je || je.ValueKind != JsonValueKind.Object)
                    throw new NotSupportedException("Value for 'between' must be an object { min, max }");
                var min = je.TryGetProperty("min", out var minEl) ? ConvertJsonElement(minEl, prop.Type) : null;
                var max = je.TryGetProperty("max", out var maxEl) ? ConvertJsonElement(maxEl, prop.Type) : null;

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
                "eq"  => Expression.Equal(prop, constExpr),
                "neq" => Expression.NotEqual(prop, constExpr),
                "lt"  => Expression.LessThan(prop, constExpr),
                "lte" => Expression.LessThanOrEqual(prop, constExpr),
                "gt"  => Expression.GreaterThan(prop, constExpr),
                "gte" => Expression.GreaterThanOrEqual(prop, constExpr),

                "contains"   => CallString("Contains"),
                "startswith" => CallString("StartsWith"),
                "endswith"   => CallString("EndsWith"),

                "in"  => CallIn(),
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

            if (value is JsonElement je)
            {
                if (je.ValueKind == JsonValueKind.Array)
                {
                    var list = ToList(je, targetType, out var elemType);
                    return (Expression.Constant(list), elemType, true);
                }
                else
                {
                    var converted = ConvertJsonElement(je, targetType);
                    return (Expression.Constant(converted, targetType), targetType, false);
                }
            }

            var convertedValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(targetType) ?? targetType);
            return (Expression.Constant(convertedValue, targetType), targetType, false);
        }

        private static object ConvertJsonElement(JsonElement je, Type targetType)
        {
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            return je.ValueKind switch
            {
                JsonValueKind.String when t == typeof(string)  => je.GetString()!,
                JsonValueKind.String when t == typeof(DateTime)=> je.GetDateTime(),
                JsonValueKind.Number when t == typeof(int)     => je.GetInt32(),
                JsonValueKind.Number when t == typeof(long)    => je.GetInt64(),
                JsonValueKind.Number when t == typeof(decimal) => je.GetDecimal(),
                JsonValueKind.Number when t == typeof(double)  => je.GetDouble(),
                JsonValueKind.True  when t == typeof(bool)     => true,
                JsonValueKind.False when t == typeof(bool)     => false,
                _ => je.Deserialize(t, new JsonSerializerOptions{PropertyNameCaseInsensitive=true})!
            };
        }

        private static object ToList(JsonElement array, Type targetType, out Type elemType)
        {
            elemType = targetType == typeof(string) ? typeof(string)
                   : targetType == typeof(int) ? typeof(int)
                   : targetType == typeof(long) ? typeof(long)
                   : targetType == typeof(decimal) ? typeof(decimal)
                   : targetType == typeof(double)  ? typeof(double)
                   : typeof(string);

            var list = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType))!;
            foreach (var el in array.EnumerateArray())
                list.Add(ConvertJsonElement(el, elemType));
            return list;
        }

        private static object? GetDefault(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}
