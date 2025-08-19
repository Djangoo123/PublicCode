using System.Text.Json;
using System.Text.Json.Serialization;

namespace SegmentBackend.Models
{
    public sealed class SegmentDefinition
    {
        public string Name { get; set; } = default!;
        [JsonConverter(typeof(SegmentDefinitionRootConverter))]
        public RuleGroup Root { get; set; } = default!;
    }

    public sealed class RuleGroup
    {
        public string Type { get; set; } = "and"; // and | or | not
        public List<object> Children { get; set; } = new();
    }

    public sealed class Rule
    {
        public string Kind { get; set; } = default!;     // attribute | metric | event ...
        public string Field { get; set; } = default!;    // ex: country, plan, orders_count
        public string Operator { get; set; } = default!; // eq, neq, in, nin, lt, lte, gt, gte, contains...
        public object? Value { get; set; }
    }

    public sealed class SegmentDefinitionRootConverter : JsonConverter<RuleGroup>
    {
        public override RuleGroup? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return RuleOrGroupHelper.ReadGroup(doc.RootElement, options);
        }
        public override void Write(Utf8JsonWriter writer, RuleGroup value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    public static class RuleOrGroupHelper
    {
        public static RuleGroup ReadGroup(JsonElement element, JsonSerializerOptions options)
        {
            var group = new RuleGroup
            {
                Type = element.TryGetProperty("type", out var t) ? t.GetString() ?? "and" : "and"
            };

            if (element.TryGetProperty("children", out var childrenEl))
            {
                foreach (var child in childrenEl.EnumerateArray())
                {
                    if (child.ValueKind == JsonValueKind.Object && child.TryGetProperty("children", out _))
                    {
                        group.Children.Add(ReadGroup(child, options));
                    }
                    else
                    {
                        var rule = JsonSerializer.Deserialize<Rule>(child.GetRawText(), options);
                        if (rule != null) group.Children.Add(rule);
                    }
                }
            }
            return group;
        }
    }
}