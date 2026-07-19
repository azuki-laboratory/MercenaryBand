using System.Collections.Generic;
using System.Text.Json;
using Godot;
using MercenaryBand.Data;

namespace MercenaryBand.Core;

public partial class DataLoader : Node
{
    private readonly Dictionary<string, BackgroundDef> _backgrounds = new();
    private readonly Dictionary<string, PerkDef> _perks = new();
    private readonly Dictionary<string, TraitDef> _traits = new();
    private readonly Dictionary<string, InjuryDef> _injuries = new();
    private readonly Dictionary<string, WeaponDef> _weapons = new();
    private readonly Dictionary<string, ArmorDef> _armors = new();
    private readonly Dictionary<string, SettlementDef> _settlements = new();
    private readonly Dictionary<string, ContractDef> _contracts = new();

    public override void _Ready()
    {
        LoadAll();
        GD.Print($"[DataLoader] Loaded {_backgrounds.Count}bg {_perks.Count}pk {_traits.Count}tr {_injuries.Count}inj {_weapons.Count}wp {_armors.Count}ar {_settlements.Count}st {_contracts.Count}ct");
    }

    private void LoadAll()
    {
        LoadList("res://data/backgrounds.json", _backgrounds, b => b.Id);
        LoadList("res://data/perks.json", _perks, p => p.Id);
        LoadList("res://data/traits.json", _traits, t => t.Id);
        LoadList("res://data/injuries.json", _injuries, i => i.Id);
        LoadList("res://data/weapons.json", _weapons, w => w.Id);
        LoadList("res://data/armors.json", _armors, a => a.Id);
        LoadList("res://data/settlements.json", _settlements, s => s.Id);
        LoadList("res://data/contracts.json", _contracts, c => c.Id);
    }

    private void LoadList<T>(string path, Dictionary<string, T> target, System.Func<T, string> keySelector)
    {
        var json = ReadJsonFile(path);
        if (string.IsNullOrEmpty(json)) return;
        var items = JsonSerializer.Deserialize<List<T>>(json);
        if (items == null) return;
        foreach (var item in items) target[keySelector(item)] = item;
    }

    private static string? ReadJsonFile(string path)
    {
        if (!Godot.FileAccess.FileExists(path))
        {
            GD.PushWarning($"[DataLoader] File not found: {path}");
            return null;
        }
        using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
        return file.GetAsText();
    }

    public BackgroundDef? GetBackground(string id) => _backgrounds.GetValueOrDefault(id);
    public PerkDef? GetPerk(string id) => _perks.GetValueOrDefault(id);
    public TraitDef? GetTrait(string id) => _traits.GetValueOrDefault(id);
    public InjuryDef? GetInjury(string id) => _injuries.GetValueOrDefault(id);
    public WeaponDef? GetWeapon(string id) => _weapons.GetValueOrDefault(id);
    public ArmorDef? GetArmor(string id) => _armors.GetValueOrDefault(id);
    public SettlementDef? GetSettlement(string id) => _settlements.GetValueOrDefault(id);
    public ContractDef? GetContract(string id) => _contracts.GetValueOrDefault(id);

    public IEnumerable<BackgroundDef> AllBackgrounds => _backgrounds.Values;
    public IEnumerable<PerkDef> AllPerks => _perks.Values;
    public IEnumerable<TraitDef> AllTraits => _traits.Values;
    public IEnumerable<InjuryDef> AllInjuries => _injuries.Values;
    public IEnumerable<WeaponDef> AllWeapons => _weapons.Values;
    public IEnumerable<ArmorDef> AllArmors => _armors.Values;
    public IEnumerable<SettlementDef> AllSettlements => _settlements.Values;
    public IEnumerable<ContractDef> AllContracts => _contracts.Values;
}
