using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Extensions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

public class TagTransformer : IDbTransformer
{
    private readonly TagMap _tagMap = new();
    private readonly TagMemberMap _memberMap = new();
    private readonly TagValueMap _valueMap = new();
    private readonly TagMemberCommentMap _memberCommentMap = new();
    private readonly TagProducerMap _producerMap = new();
    private readonly TagConsumerMap _consumerMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource(scrub: true);
        var tagRecords = new List<Tag>();
        var memberRecords = new List<Tag>();
        var valueRecords = new List<TagValueRecord>();
        var commentRecords = new List<TagCommentRecord>();
        var producerRecords = new List<ProduceInfo>();
        var consumerRecords = new List<ConsumeInfo>();

        var tags = source.Query<Tag>();

        foreach (var tag in tags)
        {
            // This forces the hash for the parent to be computed and cached so we can pass to child records.
            var tagHash = _tagMap.ComputeHash(tag);
            tagRecords.Add(tag);

            // this checks both that the produce info object exists and that an expected attribute (ProduceCount) exists.
            // Found old projects where a ProduceInfo element exists with no attribute,
            // which I want to skip to avoid parse errors, but also since an empty ProduceInfo seems to add no value.
            if (tag.ProduceInfo?.Serialize().Attribute(L5XName.ProduceCount) is not null)
                producerRecords.Add(tag.ProduceInfo);

            if (tag.ConsumeInfo is not null)
                consumerRecords.Add(tag.ConsumeInfo);

            // We will store each member since we potentially won't have corresponding data type definitions to resolve against.
            // Only store atomic member values as a slim table. This data will change with each version, so we want to split
            // it out from the tag definition/structure.
            foreach (var member in tag.Members())
            {
                memberRecords.Add(member);

                if (member is { Value: AtomicData atomic, TagName.MemberPath: not null })
                {
                    valueRecords.Add(new TagValueRecord(
                        tagHash,
                        member.TagName.MemberPath,
                        atomic.ToSqlFormat())
                    );
                }
            }

            // We are only going to store tag comment overrides and not use L5Sharp to compute/resolve pass-through comments.
            // This reduces space, but more importantly, if a data type description is changed, a hash of the tag element
            // won't capture that change and would therefore invalidate our deduplication model.
            foreach (var comment in tag.Comments ?? [])
            {
                if (comment.Operand.MemberPath is null)
                    continue;

                commentRecords.Add(new TagCommentRecord(
                    tagHash,
                    comment.Operand.MemberPath,
                    comment.Value)
                );
            }
        }

        yield return _tagMap.GenerateTable(tagRecords);
        yield return _memberMap.GenerateTable(memberRecords);
        yield return _valueMap.GenerateTable(valueRecords);
        yield return _memberCommentMap.GenerateTable(commentRecords);
        yield return _producerMap.GenerateTable(producerRecords);
        yield return _consumerMap.GenerateTable(consumerRecords);
    }
}