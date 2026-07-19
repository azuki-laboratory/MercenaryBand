using System.Collections.Generic;
using MercenaryBand.Core;

namespace MercenaryBand.Data;

public class ActiveInjury
{
    public string InjuryId { get; set; } = string.Empty;
    public int DaysRemaining { get; set; }
}

public class CharacterData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BackgroundId { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int Experience { get; set; }

    public Dictionary<string, float> Stats { get; set; } = new();

    public string? HeadArmorId { get; set; }
    public string? BodyArmorId { get; set; }
    public string? MainHandId { get; set; }
    public string? OffHandId { get; set; }
    public List<string> AccessoryIds { get; set; } = new();

    public List<string> PerkIds { get; set; } = new();
    public List<string> TraitIds { get; set; } = new();
    public List<ActiveInjury> Injuries { get; set; } = new();

    public float GetStat(string statName, DataLoader data)
    {
        float baseValue = Stats.GetValueOrDefault(statName, 0f);
        float add = 0f;
        float mult = 1f;

        void ApplyModifiers(IEnumerable<StatModifier> modifiers)
        {
            foreach (var mod in modifiers)
            {
                if (mod.Stat != statName)
                    continue;

                if (mod.Type == StatModType.Add)
                    add += mod.Value;
                else if (mod.Type == StatModType.Multiply)
                    mult *= mod.Value;
            }
        }

        foreach (var perkId in PerkIds)
        {
            var perk = data.GetPerk(perkId);
            if (perk != null)
                ApplyModifiers(perk.Modifiers);
        }

        foreach (var traitId in TraitIds)
        {
            var trait = data.GetTrait(traitId);
            if (trait != null)
                ApplyModifiers(trait.Modifiers);
        }

        foreach (var injury in Injuries)
        {
            var injuryDef = data.GetInjury(injury.InjuryId);
            if (injuryDef != null)
                ApplyModifiers(injuryDef.Modifiers);
        }

        return (baseValue + add) * mult;
    }
}
