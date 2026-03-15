using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class for importing tag data into a SQL Server database.
/// </summary>
internal class SqlServerTagImport : SqlServerImport
{
    private readonly TagMap _tagMap = new();
    private readonly TagCommentMap _commentMap = new();

    /// <inheritdoc />
    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var tagRecords = new List<TagRecord>();
        var commentRecords = new List<TagCommentRecord>();
        //alias
        //produce
        //consume
        //metadata
        
        var tags = source.Query<Tag>().SelectMany(t => t.Members());

        foreach (var tag in tags)
        {
            var snapshotId = snapshot.SnapshotId;
            var tagRecord = new TagRecord(snapshot.SnapshotId, tag);
            var tagName = tag.TagName.LocalPath;
            var tagHash = _tagMap.ComputeHash(tagRecord);

            tagRecords.Add(tagRecord);
            commentRecords.AddRange(AddTagComments(tag, snapshotId, tagHash, tagName));
        }

        await ImportRecords(tagRecords, _tagMap, session, token);
        await ImportRecords(commentRecords, _commentMap, session, token);
    }

    /// <summary>
    /// Generates a collection of tag comment records based on the specified tag, snapshot ID, tag hash, and tag name.
    /// </summary>
    /// <param name="tag">The tag object containing metadata and comments to process.</param>
    /// <param name="snapshotId">The unique identifier of the snapshot associated with the tag comments.</param>
    /// <param name="tagHash">The computed hash value of the tag, used for identifying the tag uniquely.</param>
    /// <param name="tagName">The name of the tag for which comment records are being created.</param>
    /// <returns>A collection of <see cref="TagCommentRecord"/> objects containing tag comment data.</returns>
    private static IEnumerable<TagCommentRecord> AddTagComments(Tag tag, int snapshotId, string tagHash, string tagName)
    {
        if (tag.Description is not null)
            yield return new TagCommentRecord(snapshotId, tagHash, tagName, tag.Description);

        //we need to get all bit level comments of this tag.
        foreach (var comment in tag.Comments ?? [])
        {
            if (!comment.Operand.Contains(tag.TagName.Operand)) continue;
            if (! comment.Operand.Element.All(char.IsDigit)) continue;
            var bitTag = TagName.Combine(tag.Name, comment.Operand);
            yield return new TagCommentRecord(snapshotId, tagHash, bitTag, comment.Value);
        }
    }
}