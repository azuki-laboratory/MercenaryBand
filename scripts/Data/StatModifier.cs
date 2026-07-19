using System.Text.Json.Serialization;

namespace MercenaryBand.Data;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatModType
{
    Add,
    Multiply
}

public class StatModifier
{
    public string Stat { get; set; } = string.Empty;
    public StatModType Type { get; set; }
    public float Value { get; set; }
}
