using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Represents a transformer that processes a data Target and maps its contents
/// into database-compatible table structures. Specifically designed to handle
/// the processing and structuring of tag-related information within a given Target.
/// </summary>
/// <remarks>
/// The <see cref="TagTransformer"/> extracts the tag-related data from
/// a provided <see cref="Target"/> and converts it into multiple collections of
/// records, each corresponding to different table structures such as tags,
/// comments, produce information, consume information, and aliases.
/// The transformation process yields the resulting data as a sequence of
/// <see cref="DataTable"/> objects, which can then be persisted to a database.
/// </remarks>
internal class TagTransformer : IDbTransformer
{
    private readonly TagMap _tagMap = new();
    private readonly TagMemberMap _memberMap = new();
    private readonly TagValueMap _valueMap = new();
    private readonly TagCommentMap _commentMap = new();
    private readonly TagProducerMap _producerMap = new();
    private readonly TagConsumerMap _consumerMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var tagRecords = new List<Tag>();
        var memberRecords = new List<Tag>();
        var valueRecords = new List<TagValueRecord>();
        var commentRecords = new List<TagCommentRecord>();
        var producerRecords = new List<TagProduceInfoRecord>();
        var consumerRecords = new List<TagConsumeInfoRecord>();

        var tags = source.Query<Tag>();

        foreach (var tag in tags)
        {
            tagRecords.Add(tag);
            var tagHash = tag.Hash();

            if (TagType.Produced.Equals(tag.TagType) && tag.ProduceInfo is not null)
                producerRecords.Add(new TagProduceInfoRecord(tagHash, tag.ProduceInfo));

            if (TagType.Consumed.Equals(tag.TagType) && tag.ConsumeInfo is not null)
                consumerRecords.Add(new TagConsumeInfoRecord(tagHash, tag.ConsumeInfo));

            foreach (var member in tag.Members())
            {
                var memberHash = member.Hash();
                memberRecords.Add(member);
                commentRecords.AddRange(GetTagComments(memberHash, member));

                if (member.Value.IsAtomic())
                {
                    valueRecords.Add(new TagValueRecord(target.VersionId, memberHash, tag.Value.ToSqlFormat()));
                }
            }
        }

        yield return _tagMap.GenerateTable(tagRecords);
        yield return _memberMap.GenerateTable(memberRecords);
        yield return _valueMap.GenerateTable(valueRecords);
        yield return _commentMap.GenerateTable(commentRecords);
        yield return _producerMap.GenerateTable(producerRecords);
        yield return _consumerMap.GenerateTable(consumerRecords);
    }

    /// <summary>
    /// Generates a collection of tag comment records for the specified member ID and tag.
    /// </summary>
    /// <param name="memberId">The unique identifier of the member to associate the comments with.</param>
    /// <param name="tag">The tag from which comments and descriptions will be extracted.</param>
    /// <returns>A collection of <see cref="TagCommentRecord"/> containing the associated comments and descriptions.</returns>
    private static IEnumerable<TagCommentRecord> GetTagComments(string? memberId, Tag? tag)
    {
        if (tag?.Description is not null)
        {
            yield return new TagCommentRecord(memberId, tag.TagName.LocalPath, tag.Description);
        }

        if (tag?.Comments is null)
            yield break;

        // The following code is for bit-level comments only.
        // All based tags are covered by the code above using L5Sharp internal logic.
        foreach (var comment in tag.Comments)
        {
            if (comment.Operand.Contains(tag.TagName.Operand) && comment.Operand.Element.All(char.IsDigit))
            {
                var tagName = TagName.Combine(tag.TagName.Base, comment.Operand);

                var tagComment = !string.IsNullOrWhiteSpace(tag.Description)
                    ? string.Concat(tag.Description, " ", comment.Value).Trim()
                    : comment.Value;

                yield return new TagCommentRecord(memberId, tagName, tagComment);
            }
        }
    }
}