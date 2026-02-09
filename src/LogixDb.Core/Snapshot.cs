namespace LogixDb.Core;


public record Snapshot(
    int SnapshotId,
    int TargetId,
    string? TargetType = null,
    string? TargetName = null,
    bool? IsPartial = null,
    string? SchemaRevision = null,
    string? SoftwareRevision = null,
    DateTime ExportDate = default,
    string? ExportOptions = null,
    DateTime ImportDate = default,
    string? ImportUser = null,
    string? ImportMachine = null,
    string? SourceHash = null
);