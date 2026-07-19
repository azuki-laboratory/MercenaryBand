namespace MercenaryBand.Data;

public class SettlementDef
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Faction { get; set; } = string.Empty;
    public SettlementCoord Coord { get; set; } = new();
    public int Population { get; set; }
    public int Defense { get; set; }
    public string[] AttachedLocations { get; set; } = [];
}

public class SettlementCoord
{
    public int Q { get; set; }
    public int R { get; set; }
}
