using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Represents a transformer that processes a data snapshot and maps its contents
/// into database-compatible table structures. Specifically designed to handle
/// the processing and structuring of tag-related information within a given snapshot.
/// </summary>
/// <remarks>
/// The <see cref="TagTransformer"/> extracts the tag-related data from
/// a provided <see cref="Snapshot"/> and converts it into multiple collections of
/// records, each corresponding to different table structures such as tags,
/// comments, produce information, consume information, and aliases.
/// The transformation process yields the resulting data as a sequence of
/// <see cref="DataTable"/> objects, which can then be persisted to a database.
/// </remarks>
internal class TagTransformer : ILogixDbTransformer
{
    private readonly TagMap _tagMap = new();
    private readonly TagCommentMap _commentMap = new();
    private readonly TagProducerMap _producerMap = new();
    private readonly TagConsumerMap _consumerMap = new();
    private readonly TagAliasMap _aliasMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var tagRecords = new List<TagRecord>();
        var commentRecords = new List<TagCommentRecord>();
        var producerRecords = new List<TagProduceInfoRecord>();
        var consumerRecords = new List<TagConsumeInfoRecord>();
        var aliasRecords = new List<TagAliasRecord>();

        var tags = source.Query<Tag>().SelectMany(t => t.Members());

        foreach (var tag in tags)
        {
            var snapshotId = snapshot.SnapshotId;
            var tagRecord = new TagRecord(snapshotId, tag);
            var tagId = tagRecord.TagId;
            var tagName = tag.TagName.LocalPath;

            tagRecords.Add(tagRecord);
            commentRecords.AddRange(GetTagComments(tag, snapshotId, tagId, tagName));

            if (tag.ProduceInfo is not null)
                producerRecords.Add(new TagProduceInfoRecord(snapshotId, tagId, tag.ProduceInfo));

            if (tag.ConsumeInfo is not null)
                consumerRecords.Add(new TagConsumeInfoRecord(snapshotId, tagId, tag.ConsumeInfo));

            if (tag.AliasFor is not null)
                aliasRecords.Add(new TagAliasRecord(snapshotId, tagId, tag.AliasFor.LocalPath));
        }

        yield return _tagMap.GenerateTable(tagRecords);
        yield return _commentMap.GenerateTable(commentRecords);
        yield return _producerMap.GenerateTable(producerRecords);
        yield return _consumerMap.GenerateTable(consumerRecords);
        yield return _aliasMap.GenerateTable(aliasRecords);
    }

    private static IEnumerable<TagCommentRecord> GetTagComments(Tag tag, int snapshotId, Guid tagId, string baseName)
    {
        if (tag.Description is not null)
            yield return new TagCommentRecord(snapshotId, tagId, baseName, tag.Description);

        if (tag.Comments is null)
            yield break;

        foreach (var comment in tag.Comments)
        {
            if (comment.Operand.Contains(tag.TagName.Operand) && comment.Operand.Element.All(char.IsDigit))
            {
                var tagName = TagName.Combine(baseName, comment.Operand);

                var tagComment = !string.IsNullOrWhiteSpace(tag.Description)
                    ? string.Concat(tag.Description, " ", comment.Value).Trim()
                    : comment.Value;

                yield return new TagCommentRecord(snapshotId, tagId, tagName, tagComment);
            }
        }
    }
}