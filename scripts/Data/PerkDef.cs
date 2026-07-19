using System.Collections.Generic;

namespace MercenaryBand.Data;

public class PerkRequirements
{
    public int MinLevel { get; set; }
    public Dictionary<string, int> RequiredStats { get; set; } = new();
    public List<string> RequiredPerks { get; set; } = new();
}

public class PerkDef
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Tier { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<StatModifier> Modifiers { get; set; } = new();
    public PerkRequirements? Requirements { get; set; }
}
