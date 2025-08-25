namespace SegmentBackend.DAL
{
    public class UserIdentityMap
    {
        public string LocalUserId { get; set; } = default!;
        public string BrazeExternalId { get; set; } = default!;
        public string? BrazeUserAlias { get; set; }
        public DateTime? LastSyncedUtc { get; set; }
    }

    public class Segment
    {
        public Guid SegmentId { get; set; }
        public string Name { get; set; } = default!;
        public string RulesJson { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public string? BrazeSegmentId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }

        public ICollection<SegmentVersion> Versions { get; set; } = new List<SegmentVersion>();
        public ICollection<SegmentRun> Runs { get; set; } = new List<SegmentRun>();
        public ICollection<SegmentSnapshot> Snapshots { get; set; } = new List<SegmentSnapshot>();
        public ICollection<SegmentPreviewSample> PreviewSamples { get; set; } = new List<SegmentPreviewSample>();
    }

    public class SegmentVersion
    {
        public Guid SegmentVersionId { get; set; }
        public Guid SegmentId { get; set; }
        public int Version { get; set; }
        public string RulesJson { get; set; } = default!;
        public byte[]? RulesHash { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedUtc { get; set; }

        public Segment Segment { get; set; } = default!;
    }

    public class SegmentRun
    {
        public Guid SegmentRunId { get; set; }
        public Guid SegmentId { get; set; }
        public int Version { get; set; }
        public string Mode { get; set; } = default!;   // 'Preview' | 'SyncAttributes' | 'SyncCohort'
        public string Status { get; set; } = default!; // 'Pending' | 'Running' | 'Succeeded' | 'Failed'
        public DateTime StartedUtc { get; set; }
        public DateTime? EndedUtc { get; set; }
        public int? Take { get; set; }
        public int? UserCount { get; set; }
        public string? ErrorMessage { get; set; }

        public Segment Segment { get; set; } = default!;
        public ICollection<SegmentRunMember> Members { get; set; } = new List<SegmentRunMember>();
    }

    public class SegmentRunMember
    {
        public long SegmentRunMemberId { get; set; }
        public Guid SegmentRunId { get; set; }
        public string LocalUserId { get; set; } = default!;
        public string? BrazeExternalId { get; set; }
        public bool Included { get; set; } = true;
        public string? AttributesJson { get; set; }

        public SegmentRun Run { get; set; } = default!;
    }

    public class SegmentSnapshot
    {
        public Guid SnapshotId { get; set; }
        public Guid SegmentId { get; set; }
        public int Version { get; set; }
        public DateTime TakenUtc { get; set; }
        public int UserCount { get; set; }

        public Segment Segment { get; set; } = default!;
    }

    public class SegmentPreviewSample
    {
        public Guid PreviewId { get; set; }
        public Guid SegmentId { get; set; }
        public int Version { get; set; }
        public DateTime TakenUtc { get; set; }
        public string SampleJson { get; set; } = default!;

        public Segment Segment { get; set; } = default!;
    }

    // Keyless entity mapped to the SQL view dbo.vw_Segments_LatestVersion
    public class VwSegmentLatestVersion
    {
        public Guid SegmentId { get; set; }
        public string Name { get; set; } = default!;
        public int Version { get; set; }
        public string RulesJson { get; set; } = default!;
        public bool IsActive { get; set; }
        public string? BrazeSegmentId { get; set; }
        public DateTime UpdatedUtc { get; set; }
    }
}
