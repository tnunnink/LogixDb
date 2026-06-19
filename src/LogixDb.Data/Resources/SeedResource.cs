using System.Text.Json;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Resources;

public static class SeedResource
{
    private const string Operands = "LogixDb.Data.Resources.operands.json";

    public static List<OperandRecord> LoadOperands()
    {
        var assembly = typeof(SeedResource).Assembly;
        using var stream = assembly.GetManifestResourceStream(Operands);

        if (stream == null)
            throw new FileNotFoundException($"Embedded resource '{Operands}' not found.");

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<List<OperandRecord>>(json) ?? [];
    }
}