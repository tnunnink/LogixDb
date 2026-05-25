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
            // This forces the hash for the parent to be computed and cached so we can pass to child records.
            var tagHash = _tagMap.ComputeHash(tag);
            tagRecords.Add(tag);

            if (TagType.Produced.Equals(tag.TagType) && tag.ProduceInfo is not null)
                producerRecords.Add(tag.ProduceInfo);

            if (TagType.Consumed.Equals(tag.TagType) && tag.ConsumeInfo is not null)
                consumerRecords.Add(tag.ConsumeInfo);

            foreach (var member in tag.Members())
            {
                memberRecords.Add(member);

                if (member.Value is AtomicData atomic)
                {
                    valueRecords.Add(new TagValueRecord(
                        target.VersionId,
                        tagHash,
                        member.TagName.LocalPath,
                        atomic.ToSqlFormat())
                    );
                }
            }

            if (tag.Parent is null && !string.IsNullOrWhiteSpace(tag.Description))
                commentRecords.Add(new TagCommentRecord(tagHash, tag.Name, tag.Description));

            // For now, I'm going to just insert whatever comment override exists for a tag and see if we can
            // emulate the pass-through documentation from SQL queries instead of code.
            // This would be more space-efficient and reduce processing time if we can keep this approach.
            var comments = tag.Comments?.Select(c => new TagCommentRecord(
                tagHash,
                string.Concat(tag.Name, c.Operand),
                c.Value
            )) ?? [];

            commentRecords.AddRange(comments);
        }

        yield return _tagMap.GenerateTable(tagRecords);
        yield return _memberMap.GenerateTable(memberRecords);
        yield return _valueMap.GenerateTable(valueRecords);
        yield return _commentMap.GenerateTable(commentRecords);
        yield return _producerMap.GenerateTable(producerRecords);
        yield return _consumerMap.GenerateTable(consumerRecords);
    }
}