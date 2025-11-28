using Microsoft.EntityFrameworkCore;

namespace SegmentBackend.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserIdentityMap> UserIdentityMaps => Set<UserIdentityMap>();
    public DbSet<Segment> Segments => Set<Segment>();
    public DbSet<SegmentVersion> SegmentVersions => Set<SegmentVersion>();
    public DbSet<SegmentRun> SegmentRuns => Set<SegmentRun>();
    public DbSet<SegmentRunMember> SegmentRunMembers => Set<SegmentRunMember>();
    public DbSet<SegmentSnapshot> SegmentSnapshots => Set<SegmentSnapshot>();
    public DbSet<SegmentPreviewSample> SegmentPreviewSamples => Set<SegmentPreviewSample>();

    public DbSet<VwSegmentLatestVersion> VwSegmentsLatestVersion => Set<VwSegmentLatestVersion>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // UserIdentityMap
        b.Entity<UserIdentityMap>(e =>
        {
            e.ToTable("UserIdentityMap");
            e.HasKey(x => x.LocalUserId);
            e.Property(x => x.LocalUserId).HasMaxLength(128).IsRequired();
            e.Property(x => x.BrazeExternalId).HasMaxLength(128).IsRequired();
            e.HasIndex(x => x.BrazeExternalId).IsUnique();
            e.Property(x => x.BrazeUserAlias).HasMaxLength(128);
            e.Property(x => x.LastSyncedUtc);
        });

        // Segment
        b.Entity<Segment>(e =>
        {
            e.ToTable("Segments");
            e.HasKey(x => x.SegmentId);
            e.Property(x => x.SegmentId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.RulesJson).IsRequired();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.Property(x => x.BrazeSegmentId).HasMaxLength(100);
            e.Property(x => x.Notes);
            e.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedUtc).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasMany(x => x.Versions)
             .WithOne(v => v.Segment)
             .HasForeignKey(v => v.SegmentId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(x => x.Runs)
             .WithOne(r => r.Segment)
             .HasForeignKey(r => r.SegmentId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(x => x.Snapshots)
             .WithOne(s => s.Segment)
             .HasForeignKey(s => s.SegmentId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(x => x.PreviewSamples)
             .WithOne(s => s.Segment)
             .HasForeignKey(s => s.SegmentId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // SegmentVersion
        b.Entity<SegmentVersion>(e =>
        {
            e.ToTable("SegmentVersions");
            e.HasKey(x => x.SegmentVersionId);
            e.Property(x => x.SegmentVersionId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.RulesJson).IsRequired();
            e.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.CreatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.SegmentId, x.Version }).IsUnique();
            e.Property(x => x.RulesHash).HasColumnType("varbinary(32)");
        });

        // SegmentRun
        b.Entity<SegmentRun>(e =>
        {
            e.ToTable("SegmentRuns");
            e.HasKey(x => x.SegmentRunId);
            e.Property(x => x.SegmentRunId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.Mode).HasMaxLength(32).IsRequired();
            e.Property(x => x.Status).HasMaxLength(32).IsRequired();
            e.Property(x => x.StartedUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.EndedUtc);
            e.Property(x => x.Take);
            e.Property(x => x.UserCount);
            e.Property(x => x.ErrorMessage);

            e.HasIndex(x => new { x.SegmentId, x.StartedUtc }).HasDatabaseName("IX_SegmentRuns_Segment");
            e.HasIndex(x => x.Status).HasDatabaseName("IX_SegmentRuns_Status");
        });

        // SegmentRunMember
        b.Entity<SegmentRunMember>(e =>
        {
            e.ToTable("SegmentRunMembers");
            e.HasKey(x => x.SegmentRunMemberId);
            e.Property(x => x.SegmentRunMemberId).UseIdentityColumn(); // BIGINT IDENTITY(1,1)
            e.Property(x => x.LocalUserId).HasMaxLength(128).IsRequired();
            e.Property(x => x.BrazeExternalId).HasMaxLength(128);
            e.Property(x => x.Included).HasDefaultValue(true);
            e.Property(x => x.AttributesJson);

            e.HasOne(x => x.Run)
             .WithMany(r => r.Members)
             .HasForeignKey(x => x.SegmentRunId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.SegmentRunId).HasDatabaseName("IX_SegmentRunMembers_Run");
            e.HasIndex(x => x.LocalUserId).HasDatabaseName("IX_SegmentRunMembers_User");
        });

        // SegmentSnapshot
        b.Entity<SegmentSnapshot>(e =>
        {
            e.ToTable("SegmentSnapshots");
            e.HasKey(x => x.SnapshotId);
            e.Property(x => x.SnapshotId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.TakenUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UserCount).IsRequired();

            e.HasOne(x => x.Segment)
             .WithMany(s => s.Snapshots)
             .HasForeignKey(x => x.SegmentId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.SegmentId, x.Version, x.TakenUtc })
             .HasDatabaseName("IX_SegmentSnapshots_Segment");
        });

        // SegmentPreviewSample
        b.Entity<SegmentPreviewSample>(e =>
        {
            e.ToTable("SegmentPreviewSamples");
            e.HasKey(x => x.PreviewId);
            e.Property(x => x.PreviewId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.TakenUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.SampleJson).IsRequired();

            e.HasOne(x => x.Segment)
             .WithMany(s => s.PreviewSamples)
             .HasForeignKey(x => x.SegmentId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.SegmentId, x.Version, x.TakenUtc })
             .HasDatabaseName("IX_SegmentPreviewSamples_Segment");
        });

        // View mapping (keyless)
        b.Entity<VwSegmentLatestVersion>(e =>
        {
            e.ToView("vw_Segments_LatestVersion");
            e.HasNoKey();
            e.Property(p => p.Name).HasMaxLength(200);
            e.Property(p => p.RulesJson);
            e.Property(p => p.BrazeSegmentId).HasMaxLength(100);
        });
    }
}
