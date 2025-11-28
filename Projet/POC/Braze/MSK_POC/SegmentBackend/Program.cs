using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserContext>(opt =>
    opt.UseInMemoryDatabase("Users")); 

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<UserContext>();
    ctx.Users.AddRange(new UserData { UserId = "123", Plan = "Premium", Country = "FR" },
                       new UserData { UserId = "456", Plan = "Free", Country = "FR" },
                       new UserData { UserId = "789", Plan = "Premium", Country = "US" });
    ctx.SaveChanges();
}

app.MapPost("/segments/run", async (SegmentFilter filter, UserContext ctx) =>
{
    // TODO : Here is supposed to be the lib who check result from
    // a segment and his filter when we decoded the json created by
    // react query

    var users = ctx.Users
        .Where(u => (filter.Plan == null || u.Plan == filter.Plan)
                 && (filter.Country == null || u.Country == filter.Country))
        .Select(u => u.UserId)
        .ToList();

    var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
    using var producer = new ProducerBuilder<Null, string>(config).Build();

    foreach (var userId in users)
    {
        var message = JsonSerializer.Serialize(new { type = "UserMatchedSegment", segmentId = filter.SegmentId, userId });
        await producer.ProduceAsync("user-segments", new Message<Null, string> { Value = message });
    }

    return Results.Ok(new { matched = users.Count });
});

app.Run();

record SegmentFilter(string Plan, string Country, string SegmentId);

class UserData
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Plan { get; set; } = "";
    public string Country { get; set; } = "";
}

class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }
    public DbSet<UserData> Users => Set<UserData>();
}
