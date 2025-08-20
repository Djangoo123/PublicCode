
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
        public string Operator { get; set; } = default!; // eq, neq, in, nin, lt, lte, gt, gte, between, contains...
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

    // Persistence entities
    public sealed class SegmentEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = default!;
        public string Json { get; set; } = default!;
        public int Version { get; set; } = 1;
        public string CreatedBy { get; set; } = "unknown";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }

    public sealed class SegmentRevision
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SegmentId { get; set; }
        public int Version { get; set; }
        public string Json { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "unknown";
    }

    public enum BrazeJobType { AddFlag, RemoveFlag, AddCohortId, RemoveCohortId }
    public enum BrazeJobStatus { Pending, Running, Succeeded, Failed }

    public sealed class BrazeJob
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public BrazeJobType Type { get; set; }
        public string Key { get; set; } = default!; // e.g., flag name or cohortId
        public string SegmentJson { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public BrazeJobStatus Status { get; set; } = BrazeJobStatus.Pending;
        public int Attempts { get; set; } = 0;
        public string? Error { get; set; }
    }

    public sealed class BrazeJobRun
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid JobId { get; set; }
        public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAtUtc { get; set; }
        public string Result { get; set; } = "";
        public bool Success { get; set; }
    }

    public sealed class BrazeSyncLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime AtUtc { get; set; } = DateTime.UtcNow;
        public string Action { get; set; } = "";
        public string PayloadSummary { get; set; } = "";
        public string? Error { get; set; }
    }
}
