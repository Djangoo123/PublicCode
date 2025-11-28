SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* =======================================================================
   1) Identity mapping (local -> Braze)
- LocalUserId: identifier on your information system side (often a string)
- BrazeExternalId: main identifier on Braze side
   ======================================================================= */
IF OBJECT_ID('dbo.UserIdentityMap', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserIdentityMap (
        LocalUserId        NVARCHAR(128) NOT NULL PRIMARY KEY,
        BrazeExternalId    NVARCHAR(128) NOT NULL UNIQUE,
        BrazeUserAlias     NVARCHAR(128) NULL,        
        LastSyncedUtc      DATETIME2(3)  NULL
    );
END
GO

/* =======================================================================
   2) Segments (current definition) + versions
   ======================================================================= */
IF OBJECT_ID('dbo.Segments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Segments (
        SegmentId        UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Segments PRIMARY KEY DEFAULT NEWID(),
        Name             NVARCHAR(200)    NOT NULL,
        RulesJson        NVARCHAR(MAX)    NOT NULL,   -- JSON react-query-builder saved 
        IsActive         BIT              NOT NULL CONSTRAINT DF_Segments_IsActive DEFAULT (1),
        BrazeSegmentId   NVARCHAR(100)    NULL,       -- if you need a native segment in Braze
        Notes            NVARCHAR(MAX)    NULL,

        CreatedUtc       DATETIME2(3)     NOT NULL CONSTRAINT DF_Segments_Created DEFAULT SYSUTCDATETIME(),
        UpdatedUtc       DATETIME2(3)     NOT NULL CONSTRAINT DF_Segments_Updated DEFAULT SYSUTCDATETIME()
    );

    CREATE UNIQUE INDEX UX_Segments_Name ON dbo.Segments (Name);
END
GO

-- Simple trigger for UpdatedUtc
IF OBJECT_ID('dbo.trg_Segments_UpdateTimestamp', 'TR') IS NULL
EXEC('
CREATE TRIGGER dbo.trg_Segments_UpdateTimestamp
ON dbo.Segments
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE s
    SET UpdatedUtc = SYSUTCDATETIME()
    FROM dbo.Segments s
    INNER JOIN inserted i ON i.SegmentId = s.SegmentId;
END
');
GO

IF OBJECT_ID('dbo.SegmentVersions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SegmentVersions (
        SegmentVersionId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_SegmentVersions PRIMARY KEY DEFAULT NEWID(),
        SegmentId        UNIQUEIDENTIFIER NOT NULL,
        Version          INT              NOT NULL,
        RulesJson        NVARCHAR(MAX)    NOT NULL,
        RulesHash        VARBINARY(32)    NULL,       -- optional: hash for change detection
        CreatedBy        NVARCHAR(100)    NULL,
        CreatedUtc       DATETIME2(3)     NOT NULL CONSTRAINT DF_SegmentVersions_Created DEFAULT SYSUTCDATETIME(),

        CONSTRAINT FK_SegmentVersions_Segments
            FOREIGN KEY (SegmentId) REFERENCES dbo.Segments(SegmentId) ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX UX_SegmentVersions_Segment_Version
        ON dbo.SegmentVersions (SegmentId, Version);
END
GO

/* =======================================================================
   3) Executions (preview/sync) and members sent
   ======================================================================= */
IF OBJECT_ID('dbo.SegmentRuns', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SegmentRuns (
        SegmentRunId   UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_SegmentRuns PRIMARY KEY DEFAULT NEWID(),
        SegmentId      UNIQUEIDENTIFIER NOT NULL,
        Version        INT              NOT NULL,     -- references the version executed
        Mode           NVARCHAR(32)     NOT NULL,     -- 'Preview' | 'SyncAttributes' | 'SyncCohort'
        Status         NVARCHAR(32)     NOT NULL,     -- 'Pending' | 'Running' | 'Succeeded' | 'Failed'
        StartedUtc     DATETIME2(3)     NOT NULL CONSTRAINT DF_SegmentRuns_Started DEFAULT SYSUTCDATETIME(),
        EndedUtc       DATETIME2(3)     NULL,
        Take           INT              NULL,         
        UserCount      INT              NULL,         
        ErrorMessage   NVARCHAR(MAX)    NULL,

        CONSTRAINT FK_SegmentRuns_Segments
            FOREIGN KEY (SegmentId) REFERENCES dbo.Segments(SegmentId) ON DELETE CASCADE
    );

    CREATE INDEX IX_SegmentRuns_Segment ON dbo.SegmentRuns (SegmentId, StartedUtc DESC);
    CREATE INDEX IX_SegmentRuns_Status  ON dbo.SegmentRuns (Status);
END
GO

IF OBJECT_ID('dbo.SegmentRunMembers', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SegmentRunMembers (
        SegmentRunMemberId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SegmentRunMembers PRIMARY KEY,
        SegmentRunId       UNIQUEIDENTIFIER NOT NULL,
        LocalUserId        NVARCHAR(128)   NOT NULL,
        BrazeExternalId    NVARCHAR(128)   NULL,      -- filled if known/resolved via UserIdentityMap
        Included           BIT             NOT NULL CONSTRAINT DF_SegmentRunMembers_Included DEFAULT (1),
        AttributesJson     NVARCHAR(MAX)   NULL,      -- payload sent to Braze (attributes/traits)

        CONSTRAINT FK_SegmentRunMembers_Runs
            FOREIGN KEY (SegmentRunId) REFERENCES dbo.SegmentRuns(SegmentRunId) ON DELETE CASCADE
    );

    CREATE INDEX IX_SegmentRunMembers_Run ON dbo.SegmentRunMembers (SegmentRunId);
    CREATE INDEX IX_SegmentRunMembers_User ON dbo.SegmentRunMembers (LocalUserId);
END
GO

/* =======================================================================
   4) Snapshots ( optionnal)
   ======================================================================= */
IF OBJECT_ID('dbo.SegmentSnapshots', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SegmentSnapshots (
        SnapshotId   UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_SegmentSnapshots PRIMARY KEY DEFAULT NEWID(),
        SegmentId    UNIQUEIDENTIFIER NOT NULL,
        Version      INT              NOT NULL,
        TakenUtc     DATETIME2(3)     NOT NULL CONSTRAINT DF_SegmentSnapshots_Taken DEFAULT SYSUTCDATETIME(),
        UserCount    INT              NOT NULL,

        CONSTRAINT FK_SegmentSnapshots_Segments
            FOREIGN KEY (SegmentId) REFERENCES dbo.Segments(SegmentId) ON DELETE CASCADE
    );

    CREATE INDEX IX_SegmentSnapshots_Segment ON dbo.SegmentSnapshots (SegmentId, Version, TakenUtc DESC);
END
GO

/* =======================================================================
   5) (Optionnal) Cache  previews 
   ======================================================================= */
IF OBJECT_ID('dbo.SegmentPreviewSamples', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SegmentPreviewSamples (
        PreviewId     UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_SegmentPreviewSamples PRIMARY KEY DEFAULT NEWID(),
        SegmentId     UNIQUEIDENTIFIER NOT NULL,
        Version       INT              NOT NULL,
        TakenUtc      DATETIME2(3)     NOT NULL CONSTRAINT DF_SegmentPreviewSamples_Taken DEFAULT SYSUTCDATETIME(),
        SampleJson    NVARCHAR(MAX)    NOT NULL,      

        CONSTRAINT FK_SegmentPreviewSamples_Segments
            FOREIGN KEY (SegmentId) REFERENCES dbo.Segments(SegmentId) ON DELETE CASCADE
    );

    CREATE INDEX IX_SegmentPreviewSamples_Segment ON dbo.SegmentPreviewSamples (SegmentId, Version, TakenUtc DESC);
END
GO

/* =======================================================================
   6) Helpers
   ======================================================================= */

-- View: latest version of a segment
IF OBJECT_ID('dbo.vw_Segments_LatestVersion', 'V') IS NOT NULL
    DROP VIEW dbo.vw_Segments_LatestVersion;
GO
CREATE VIEW dbo.vw_Segments_LatestVersion AS
SELECT s.SegmentId, s.Name, sv.Version, sv.RulesJson, s.IsActive, s.BrazeSegmentId, s.UpdatedUtc
FROM dbo.Segments s
JOIN (
    SELECT SegmentId, MAX(Version) AS Version
    FROM dbo.SegmentVersions
    GROUP BY SegmentId
) lastv ON lastv.SegmentId = s.SegmentId
JOIN dbo.SegmentVersions sv ON sv.SegmentId = s.SegmentId AND sv.Version = lastv.Version;
GO
