using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MercenaryBand.Data;
using MercenaryBand.Core;

namespace MercenaryBand.Systems;

public class CharacterFactory
{
    private readonly DataLoader _data;

    public CharacterFactory(DataLoader data)
    {
        _data = data;
    }

    public CharacterData Create(string backgroundId, int level = 1)
    {
        var bg = _data.GetBackground(backgroundId);
        if (bg == null)
        {
            GD.PushError($"[CharacterFactory] Unknown background: {backgroundId}");
            return new CharacterData { Id = System.Guid.NewGuid().ToString(), Name = "Unknown" };
        }

        var character = new CharacterData
        {
            Id = System.Guid.NewGuid().ToString(),
            Name = bg.Name,
            BackgroundId = backgroundId,
            Level = level,
            Stats = RollBaseStats(bg)
        };

        var traits = RollTraits(bg);
        character.TraitIds.AddRange(traits.Select(t => t.Id));

        GD.Print($"[CharacterFactory] Created {character.Name} (Lv.{level}) with {traits.Count} traits");
        return character;
    }

    private Dictionary<string, float> RollBaseStats(BackgroundDef bg)
    {
        return new Dictionary<string, float>
        {
            ["Hitpoints"] = CharacterRng.Range(bg.BaseStats["Hitpoints"].Min, bg.BaseStats["Hitpoints"].Max),
            ["Fatigue"] = CharacterRng.Range(bg.BaseStats["Fatigue"].Min, bg.BaseStats["Fatigue"].Max),
            ["Resolve"] = CharacterRng.Range(bg.BaseStats["Resolve"].Min, bg.BaseStats["Resolve"].Max),
            ["Initiative"] = CharacterRng.Range(bg.BaseStats["Initiative"].Min, bg.BaseStats["Initiative"].Max),
            ["MeleeSkill"] = CharacterRng.Range(bg.BaseStats["MeleeSkill"].Min, bg.BaseStats["MeleeSkill"].Max),
            ["RangedSkill"] = CharacterRng.Range(bg.BaseStats["RangedSkill"].Min, bg.BaseStats["RangedSkill"].Max),
            ["MeleeDefense"] = CharacterRng.Range(bg.BaseStats["MeleeDefense"].Min, bg.BaseStats["MeleeDefense"].Max),
            ["RangedDefense"] = CharacterRng.Range(bg.BaseStats["RangedDefense"].Min, bg.BaseStats["RangedDefense"].Max),
            ["Level"] = 1,
            ["Experience"] = 0,
            ["ActionPoints"] = 9
        };
    }

    private List<TraitDef> RollTraits(BackgroundDef bg)
    {
        var available = _data.AllTraits
            .Where(t => !bg.ExcludedTraits.Contains(t.Id))
            .ToList();

        int traitCount = CharacterRng.Range(0, 2);
        var picked = CharacterRng.PickWeighted(available, t =>
        {
            var weighted = bg.WeightedTraits.FirstOrDefault(w => w.TraitId == t.Id);
            return weighted?.Weight ?? 1;
        }, traitCount);

        return picked;
    }
}
