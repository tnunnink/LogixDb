using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Extensions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

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
        var producerRecords = new List<ProduceInfo>();
        var consumerRecords = new List<ConsumeInfo>();

        var tags = source.Query<Tag>();

        foreach (var tag in tags)
        {
            var tagHash = string.Empty; //todo compute hash hash here.
            tag.Metadata.Add("tag_hash", tagHash);

            tagRecords.Add(tag);

            if (TagType.Produced.Equals(tag.TagType) && tag.ProduceInfo is not null)
            {
                tag.ProduceInfo.Metadata.Add("tag_hash", tagHash);
                producerRecords.Add(tag.ProduceInfo);
            }

            if (TagType.Consumed.Equals(tag.TagType) && tag.ConsumeInfo is not null)
            {
                tag.ConsumeInfo.Metadata.Add("tag_hash", tagHash);
                consumerRecords.Add(tag.ConsumeInfo);
            }

            foreach (var member in tag.Members())
            {
                member.Metadata.Add("tag_hash", tagHash);
                memberRecords.Add(member);
                /*commentRecords.AddRange(GetTagComments(tagHash, member));*/

                if (member.Value.IsAtomic())
                {
                    valueRecords.Add(new TagValueRecord(
                        target.VersionId,
                        tagHash,
                        member.TagName.LocalPath,
                        tag.Value.ToSqlFormat())
                    );
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

    /*/// <summary>
    /// Generates a collection of tag comment records for the specified member ID and tag.
    /// </summary>
    /// <param name="memberId">The unique identifier of the member to associate the comments with.</param>
    /// <param name="tag">The tag from which comments and descriptions will be extracted.</param>
    /// <returns>A collection of <see cref="TagCommentRecord"/> containing the associated comments and descriptions.</returns>
    private static IEnumerable<TagCommentRecord> GetTagComments(Guid? memberId, Tag? tag)
    {
        if (tag is null) yield break;
        if (tag.Parent is not null) yield break;

        var description = tag.Description;
        if (string.IsNullOrEmpty(description)) yield break;

        yield return new TagCommentRecord(memberId, tag.TagName.LocalPath, description);

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
    }*/
}