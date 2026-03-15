using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of tag records from a LogixDb snapshot into an SQLite database.
/// This class processes tag entities by querying them from the snapshot source and inserting
/// them into the database using the configured tag table mapping.
/// </summary>
internal class SqliteTagImport : SqliteImport
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
            var programName = tag.Scope.Container;
            var tagName = tag.TagName.LocalPath;

            var tagRecord = new TagRecord(snapshotId, tag);
            
            tagRecords.Add(tagRecord);
            commentRecords.AddRange(GetTagComments(tag, snapshotId, programName, tagName));
        }

        await using var tagCommand = BuildCommand(_tagMap, session);
        await ImportRecords(tagRecords, _tagMap, tagCommand, token);

        await using var commentCommand = BuildCommand(_commentMap, session);
        await ImportRecords(commentRecords, _commentMap, commentCommand, token);
    }


    private static IEnumerable<TagCommentRecord> GetTagComments(Tag tag, int snapshotId, string program, string baseName)
    {
        if (tag.Description is not null)
            yield return new TagCommentRecord(snapshotId, program, baseName, tag.Description);

        if (tag.Comments is null)
            yield break;

        //we need to get all bit level comments of this tag.
        foreach (var comment in tag.Comments)
        {
            if (comment.Operand.Contains(tag.TagName.Operand) && comment.Operand.Element.All(char.IsDigit))
            {
                var bitTag = TagName.Combine(baseName, comment.Operand);
                //the comment should mimic pass through descriptions like base-description does. Maybe this is something we can update on L5Sharp
                var text = string.Concat(tag.Description, " ", comment.Value);
                yield return new TagCommentRecord(snapshotId, program, bitTag, text);
            }
        }
    }
}