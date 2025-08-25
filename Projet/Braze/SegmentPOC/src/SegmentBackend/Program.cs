
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SegmentBackend.Data;
using SegmentBackend.Models;
using SegmentBackend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("UsersDb"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Segment API", Version = "v1" });
});

var brazeEndpoint = builder.Configuration["Braze:Endpoint"] ?? Environment.GetEnvironmentVariable("BRAZE_REST_ENDPOINT");
var brazeApiKey   = builder.Configuration["Braze:ApiKey"]   ?? Environment.GetEnvironmentVariable("BRAZE_REST_API_KEY");
if (!string.IsNullOrWhiteSpace(brazeEndpoint) && !string.IsNullOrWhiteSpace(brazeApiKey))
{
    builder.Services.AddHttpClient<BrazeClient>()
        .AddTypedClient((http, sp) => new BrazeClient(http, brazeEndpoint!, brazeApiKey!));
}

builder.Services.AddHostedService<BrazeJobWorker>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Users.Any())
    {
        db.Users.AddRange([
            new User { Id = "u1", Country = "FR", Plan = "premium", OrdersCount = 10, TotalSpend = 300, CreatedUtc = DateTime.UtcNow.AddDays(-50) },
            new User { Id = "u2", Country = "BE", Plan = "free", OrdersCount = 2, TotalSpend = 50,  CreatedUtc = DateTime.UtcNow.AddDays(-10) },
            new User { Id = "u3", Country = "NL", Plan = "premium", OrdersCount = 6, TotalSpend = 220, CreatedUtc = DateTime.UtcNow.AddDays(-90) },
            new User { Id = "u4", Country = "FR", Plan = "pro", OrdersCount = 1, TotalSpend = 20,  CreatedUtc = DateTime.UtcNow.AddDays(-5)  }
        ]);
        db.SaveChanges();
    }
}

app.Run();
