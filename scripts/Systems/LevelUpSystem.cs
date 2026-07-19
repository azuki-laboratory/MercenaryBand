using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MercenaryBand.Data;
using MercenaryBand.Core;

namespace MercenaryBand.Systems;

public class LevelUpSystem
{
    private readonly DataLoader _data;

    public LevelUpSystem(DataLoader data)
    {
        _data = data;
    }

    public LevelUpResult? ApplyExperience(CharacterData character, int xp)
    {
        character.Experience += xp;
        character.Stats["Experience"] = character.Experience;

        int oldLevel = character.Level;
        while (character.Level < ExperienceTable.MaxLevel &&
               character.Experience >= ExperienceTable.GetXpForLevel(character.Level + 1))
        {
            character.Level++;
            character.Stats["Level"] = character.Level;

            int points = CharacterRng.Range(2, 4);
            var upgrades = RollStatUpgrades(points);

            foreach (var (stat, value) in upgrades)
            {
                if (character.Stats.ContainsKey(stat))
                    character.Stats[stat] += value;
            }

            GD.Print($"[LevelUp] {character.Name} reached level {character.Level}! +{upgrades.Count} stats");
        }

        if (character.Level > oldLevel)
        {
            var availablePerks = GetAvailablePerks(character, character.Level - oldLevel);
            return new LevelUpResult
            {
                OldLevel = oldLevel,
                NewLevel = character.Level,
                AvailablePerks = availablePerks
            };
        }

        return null;
    }

    private List<(string stat, int value)> RollStatUpgrades(int points)
    {
        var stats = new[] { "Hitpoints", "Fatigue", "Resolve", "MeleeSkill", "RangedSkill", "MeleeDefense", "RangedDefense" };
        var chosen = CharacterRng.PickWeighted(stats.ToList(), s => 1, Math.Min(points, stats.Length));

        return chosen.Select(s =>
        {
            int val = CharacterRng.Range(1, 4);
            return (s, val);
        }).ToList();
    }

    public List<PerkDef> GetAvailablePerks(CharacterData character, int count)
    {
        var available = _data.AllPerks
            .Where(p => MeetsRequirements(p, character))
            .ToList();

        return CharacterRng.PickWeighted(available, p => 1, Math.Min(count, available.Count));
    }

    private bool MeetsRequirements(PerkDef perk, CharacterData character)
    {
        if (perk.Requirements == null)
            return true;

        if (character.Level < perk.Requirements.MinLevel)
            return false;

        if (perk.Requirements.RequiredStats != null)
        {
            foreach (var (stat, requiredValue) in perk.Requirements.RequiredStats)
            {
                if (character.Stats.GetValueOrDefault(stat, 0) < requiredValue)
                    return false;
            }
        }

        if (perk.Requirements.RequiredPerks != null)
        {
            foreach (var reqPerk in perk.Requirements.RequiredPerks)
            {
                if (!character.PerkIds.Contains(reqPerk))
                    return false;
            }
        }

        return true;
    }

    public void ApplyPerk(CharacterData character, string perkId)
    {
        if (character.PerkIds.Contains(perkId))
            return;

        var perk = _data.GetPerk(perkId);
        if (perk == null)
            return;

        character.PerkIds.Add(perkId);
        GD.Print($"[LevelUp] {character.Name} learned perk: {perk.Name}");
    }
}

public class LevelUpResult
{
    public int OldLevel { get; set; }
    public int NewLevel { get; set; }
    public List<PerkDef> AvailablePerks { get; set; } = new();
}
