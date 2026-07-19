namespace MercenaryBand.Data;

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
