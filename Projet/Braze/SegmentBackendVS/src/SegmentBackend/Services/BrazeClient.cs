using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SegmentBackend.Services
{
    public sealed class BrazeClient
    {
        private const int MaxBatch = 75;
        private readonly HttpClient _http;
        private readonly string _endpoint;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public BrazeClient(HttpClient http, string endpointBaseUrl, string apiKey)
        {
            _http = http;
            _endpoint = endpointBaseUrl.TrimEnd('/');
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<BrazeBatchResult> UpsertCohortAsync(IEnumerable<string> externalIds, string cohortKey, bool value = true, CancellationToken ct = default)
        {
            var result = new BrazeBatchResult();
            foreach (var chunk in externalIds.Chunk(MaxBatch))
            {
                var attributes = chunk.Select(id => new Dictionary<string, object>
                {
                    ["external_id"] = id,
                    [cohortKey] = value
                }).ToList();

                var payload = new Dictionary<string, object> { ["attributes"] = attributes };
                var content = new StringContent(JsonSerializer.Serialize(payload, _json), Encoding.UTF8, "application/json");

                var resp = await _http.PostAsync($"{_endpoint}/users/track", content, ct);
                if ((int)resp.StatusCode == 429)
                {
                    var delay = GetRetryAfter(resp) ?? TimeSpan.FromSeconds(2);
                    await Task.Delay(delay, ct);
                    resp = await _http.PostAsync($"{_endpoint}/users/track", content, ct);
                }

                var body = await resp.Content.ReadAsStringAsync(ct);
                if (!resp.IsSuccessStatusCode)
                {
                    result.Failures.Add(new BrazeBatchFailure
                    {
                        StatusCode = (int)resp.StatusCode,
                        RawResponse = body
                    });
                    continue;
                }
                result.SuccessfulBatches++;
            }
            return result;
        }

        private static TimeSpan? GetRetryAfter(HttpResponseMessage resp)
        {
            if (resp.Headers.TryGetValues("Retry-After", out var values))
            {
                var v = values.FirstOrDefault();
                if (int.TryParse(v, out var seconds)) return TimeSpan.FromSeconds(seconds);
                if (DateTimeOffset.TryParse(v, out var when)) return when - DateTimeOffset.UtcNow;
            }
            return null;
        }
    }

    public sealed class BrazeBatchResult
    {
        public int SuccessfulBatches { get; set; }
        public List<BrazeBatchFailure> Failures { get; } = new();
    }

    public sealed class BrazeBatchFailure
    {
        public int StatusCode { get; set; }
        public string RawResponse { get; set; } = "";
    }
}