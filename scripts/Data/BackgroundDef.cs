using System.Collections.Generic;

namespace MercenaryBand.Data;

public class StatRange
{
    public int Min { get; set; }
    public int Max { get; set; }
}

public class WeightedTrait
{
    public string TraitId { get; set; } = string.Empty;
    public int Weight { get; set; }
}

public class BackgroundDef
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, StatRange> BaseStats { get; set; } = new();
    public List<string> ExcludedTraits { get; set; } = new();
    public List<WeightedTrait> WeightedTraits { get; set; } = new();
    public StatRange HiringCost { get; set; } = new();
    public int DailyWage { get; set; }
}
