using Microsoft.EntityFrameworkCore;
using SegmentBackend.Models;

namespace SegmentBackend.Data
{
    public sealed class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }
    }
}