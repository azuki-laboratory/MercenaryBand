using System.Collections.Generic;

namespace MercenaryBand.Data;

public class TraitDef
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsPositive { get; set; }
    public List<StatModifier> Modifiers { get; set; } = new();
}
