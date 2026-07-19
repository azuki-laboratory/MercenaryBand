namespace MercenaryBand.Data;

public enum ArmorSlot
{
    Head,
    Body
}

public class ArmorDef
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ArmorSlot Slot { get; set; }
    public int Armor { get; set; }
    public int Durability { get; set; }
    public int MaxFatiguePenalty { get; set; }
    public int Value { get; set; }
}
