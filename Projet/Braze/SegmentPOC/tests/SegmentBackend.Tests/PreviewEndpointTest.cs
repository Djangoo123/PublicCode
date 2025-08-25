using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegmentBackend.Controllers;
using SegmentBackend.Data;
using SegmentBackend.Models;
using Xunit;

namespace SegmentBackend.Tests
{
    public class PreviewEndpointTests
    {
        private static AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"segments-tests-{Guid.NewGuid()}")
                .Options;

            var db = new AppDbContext(options);

            db.Users.AddRange(
                new User { Id = "1", Country = "FR", Plan = "premium", OrdersCount = 6, TotalSpend = 120m, CreatedUtc = DateTime.UtcNow.AddDays(-10) },
                new User { Id = "2", Country = "BE", Plan = "premium", OrdersCount = 1, TotalSpend = 250m, CreatedUtc = DateTime.UtcNow.AddDays(-20) },
                new User { Id = "3", Country = "NL", Plan = "basic", OrdersCount = 10, TotalSpend = 500m, CreatedUtc = DateTime.UtcNow.AddDays(-30) },
                new User { Id = "4", Country = "US", Plan = "premium", OrdersCount = 8, TotalSpend = 80m, CreatedUtc = DateTime.UtcNow.AddDays(-40) },
                new User { Id = "5", Country = "FR", Plan = "premium", OrdersCount = 1, TotalSpend = 50m, CreatedUtc = DateTime.UtcNow.AddDays(-5) }
            );

            db.SaveChanges();
            return db;
        }

        [Fact]
        public async Task Preview_ReturnsOk_WithCountAndSample()
        {
            try
            {
                using var db = CreateInMemoryDb();

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var controller = new SegmentsController(db, jsonOptions);

                var queryBuilderJson = @"
                {
                  ""name"": ""VIP EU"",
                  ""root"": {
                    ""type"": ""and"",
                    ""children"": [
                      { ""kind"": ""attribute"", ""field"": ""country"", ""operator"": ""in"", ""value"": [""FR"", ""BE"", ""NL""] },
                      { ""kind"": ""attribute"", ""field"": ""plan"", ""operator"": ""eq"", ""value"": ""premium"" },
                      {
                        ""type"": ""or"",
                        ""children"": [
                          { ""kind"": ""metric"", ""field"": ""orders_count"", ""operator"": ""gte"", ""value"": 5 },
                          { ""kind"": ""metric"", ""field"": ""total_spend"",  ""operator"": ""gte"", ""value"": 200 }
                        ]
                      }
                    ]
                  }
                }";

                using var doc = JsonDocument.Parse(queryBuilderJson);
                var segmentJson = doc.RootElement;

                var actionResult = await controller.Preview(segmentJson, take: 50, timeoutMs: 5000, ct: CancellationToken.None);

                var checkIfOf = Assert.IsType<OkObjectResult>(actionResult.Result);
                Assert.NotNull(checkIfOf.Value);

                var payloadJson = JsonSerializer.Serialize(checkIfOf.Value);
                using var outDoc = JsonDocument.Parse(payloadJson);

                var root = outDoc.RootElement;
                Assert.True(root.TryGetProperty("Count", out var countEl));
                Assert.True(root.TryGetProperty("Sample", out var sampleEl));

                Assert.Equal(2, countEl.GetInt32());
                Assert.Equal(JsonValueKind.Array, sampleEl.ValueKind);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
