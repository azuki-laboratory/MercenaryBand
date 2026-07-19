using System.Collections.Generic;

namespace MercenaryBand.Data;

public enum WeaponType
{
    OneHanded,
    TwoHanded
}

public class WeaponDef
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public WeaponType Type { get; set; }
    public int DamageMin { get; set; }
    public int DamageMax { get; set; }
    public int ArmorDamage { get; set; }
    public int ArmorPenetration { get; set; }
    public int FatiguePerSwing { get; set; }
    public List<string> Skills { get; set; } = new();
    public int Value { get; set; }
    public int Durability { get; set; }
    public int MaxFatiguePenalty { get; set; }
    public bool TwoHanded { get; set; }
}
