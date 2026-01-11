using KataBaseProject.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddDbContext<KataDbContext>(options =>
    options.UseNpgsql(
        "Host=localhost;Port=5432;Database=kata_db;Username=kata;Password=kata"
    ));

using var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<KataDbContext>();

db.Database.Migrate();
