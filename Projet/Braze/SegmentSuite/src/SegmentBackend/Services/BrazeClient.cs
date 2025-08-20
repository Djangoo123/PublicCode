
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

        public async Task<(int ok, List<string> errors)> UpsertAsync(IEnumerable<Dictionary<string, object>> attributes, CancellationToken ct)
        {
            var payload = new Dictionary<string, object> { ["attributes"] = attributes.ToList() };
            var content = new StringContent(JsonSerializer.Serialize(payload, _json), Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync($"{_endpoint}/users/track", content, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode) return (0, new List<string>{ body });
            return (1, new List<string>());
        }
    }
}
