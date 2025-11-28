
using Microsoft.EntityFrameworkCore;
using SegmentBackend.Models;

namespace SegmentBackend.Data
{
    public sealed class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<SegmentEntity> Segments => Set<SegmentEntity>();
        public DbSet<SegmentRevision> SegmentRevisions => Set<SegmentRevision>();
        public DbSet<BrazeJob> BrazeJobs => Set<BrazeJob>();
        public DbSet<BrazeJobRun> BrazeJobRuns => Set<BrazeJobRun>();
        public DbSet<BrazeSyncLog> BrazeSyncLogs => Set<BrazeSyncLog>();

        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }
    }
}
