using System.Diagnostics;
using FluentAssertions;
using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Tests;

[TestFixture]
public class SourceHasherTests
{
    [Test]
    [Explicit("This is just for local proof of concept")]
    public void IterationTime()
    {
        var content = L5X.Load(@"C:\Users\tnunn\Documents\Rockwell\Example.L5X");

        var stopwatch = Stopwatch.StartNew();

        var groups = content.Query<Tag>()
            .GroupBy(t => t.TagName.LocalPath)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in groups)
        {
            var tagName = group.Key;
            var hashes = group.Select(x => new { x.TagName, Hash = x.HashElement() });
            var distinct = hashes.DistinctBy(x => x.Hash).Count();
            Console.WriteLine($"{tagName} found {distinct} hashes");
        }

        stopwatch.Stop();
        Console.WriteLine(stopwatch.ElapsedMilliseconds);
    }
}