using L5Sharp.Core;

namespace LogixDb.Data.Tests;

[TestFixture]
public class Test
{
    [Test]
    [Explicit("This is just for testing")]
    public void Testing()
    {
        var source = L5X.Load(@"c:\users\tnunnink\desktop\encrypted.L5X");

        var instructions = source.AddOnInstructions.ToList();
        
        Assert.That(instructions, Is.Not.Empty);
    }
}