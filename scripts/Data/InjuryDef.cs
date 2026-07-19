using System.Collections.Generic;

namespace MercenaryBand.Data;

public class DurationRange
{
    public int Min { get; set; }
    public int Max { get; set; }
}

public class InjuryDef
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DurationRange DurationDays { get; set; } = new();
    public List<StatModifier> Modifiers { get; set; } = new();
}
