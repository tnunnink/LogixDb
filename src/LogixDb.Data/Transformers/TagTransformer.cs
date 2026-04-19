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
internal class TagTransformer : ISnapshotTransformer
{
    private readonly TagMap _tagMap = new();
    private readonly TagMemberMap _memberMap = new();
    private readonly TagCommentMap _commentMap = new();
    private readonly TagProducerMap _producerMap = new();
    private readonly TagConsumerMap _consumerMap = new();
    private readonly TagAliasMap _aliasMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var tagRecords = new List<TagRecord>();
        var memberRecords = new List<TagMemberRecord>();
        var commentRecords = new List<TagCommentRecord>();
        var producerRecords = new List<TagProduceInfoRecord>();
        var consumerRecords = new List<TagConsumeInfoRecord>();
        var aliasRecords = new List<TagAliasRecord>();
        var memberLookup = new Dictionary<TagName, TagMemberRecord>();

        var tags = source.Query<Tag>();

        foreach (var tag in tags)
        {
            var tagRecord = new TagRecord(snapshot.SnapshotId, tag.Program?.Metadata.Get<Guid>("id"), tag);
            tagRecords.Add(tagRecord);

            if (tag.ProduceInfo is not null)
                producerRecords.Add(new TagProduceInfoRecord(snapshot.SnapshotId, tagRecord.TagId, tag.ProduceInfo));

            if (tag.ConsumeInfo is not null)
                consumerRecords.Add(new TagConsumeInfoRecord(snapshot.SnapshotId, tagRecord.TagId, tag.ConsumeInfo));

            if (tag.AliasFor is not null)
                aliasRecords.Add(new TagAliasRecord(snapshot.SnapshotId, tagRecord.TagId, tag.AliasFor.LocalPath));

            foreach (var member in tag.Members())
            {
                //Get parent id using the lookup
                var parentName = member.Parent?.TagName ?? TagName.Empty;
                Guid? parentId = memberLookup.TryGetValue(parentName, out var match) ? match.MemberId : null;

                //Generate member record and comment records and add to collections.
                var memberRecord = new TagMemberRecord(snapshot.SnapshotId, tagRecord.TagId, parentId, member);

                if (!memberLookup.TryAdd(member.TagName, memberRecord))
                    throw new InvalidOperationException(
                        $"Duplicate member TagName encountered: '{member.TagName}'. Each member must have a unique TagName within the tag.");

                memberRecords.Add(memberRecord);
                commentRecords.AddRange(GetTagComments(snapshot.SnapshotId, memberRecord));
            }
        }

        yield return _tagMap.GenerateTable(tagRecords);
        yield return _memberMap.GenerateTable(memberRecords);
        yield return _commentMap.GenerateTable(commentRecords);
        yield return _producerMap.GenerateTable(producerRecords);
        yield return _consumerMap.GenerateTable(consumerRecords);
        yield return _aliasMap.GenerateTable(aliasRecords);
    }

    /// <summary>
    /// Extracts comments from a provided tag member record and generates a collection of tag comment records.
    /// </summary>
    /// <param name="snapshotId">The unique identifier of the snapshot to associate with the generated comments.</param>
    /// <param name="record">The tag member record containing tag metadata and associated details.</param>
    /// <returns>A collection of tag comment records generated from the provided tag member record.</returns>
    private static IEnumerable<TagCommentRecord> GetTagComments(int snapshotId, TagMemberRecord record)
    {
        if (record.Tag.Description is not null)
        {
            yield return new TagCommentRecord(
                snapshotId,
                record.MemberId,
                record.Tag.TagName.LocalPath,
                record.Tag.Description
            );
        }

        if (record.Tag.Comments is null)
            yield break;

        // The following code is for bit-level comments only.
        // All based tags are covered by the code above using L5Sharp internal logic.
        foreach (var comment in record.Tag.Comments)
        {
            if (comment.Operand.Contains(record.Tag.TagName.Operand) && comment.Operand.Element.All(char.IsDigit))
            {
                var tagName = TagName.Combine(record.Tag.TagName.Base, comment.Operand);

                var tagComment = !string.IsNullOrWhiteSpace(record.Tag.Description)
                    ? string.Concat(record.Tag.Description, " ", comment.Value).Trim()
                    : comment.Value;

                yield return new TagCommentRecord(snapshotId, record.MemberId, tagName, tagComment);
            }
        }
    }
}