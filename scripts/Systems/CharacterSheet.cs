using System.Collections.Generic;
using Godot;
using MercenaryBand.Data;
using MercenaryBand.Core;

namespace MercenaryBand.Systems;

public class CharacterSheet
{
    private readonly CharacterData _data;
    private readonly DataLoader _dataLoader;

    public CharacterData Data => _data;
    public string Name => _data.Name;
    public int Level => _data.Level;

    public CharacterSheet(CharacterData data, DataLoader dataLoader)
    {
        _data = data;
        _dataLoader = dataLoader;
    }

    public float GetStat(string statName) => _data.GetStat(statName, _dataLoader);

    public void EquipMainHand(string weaponId)
    {
        _data.MainHandId = weaponId;
        GD.Print($"[CharacterSheet] {_data.Name} equipped {weaponId}");
    }

    public void EquipOffHand(string? itemId)
    {
        _data.OffHandId = itemId;
    }

    public void EquipHead(string armorId)
    {
        _data.HeadArmorId = armorId;
    }

    public void EquipBody(string armorId)
    {
        _data.BodyArmorId = armorId;
    }

    public void AddInjury(string injuryId, int days)
    {
        _data.Injuries.Add(new ActiveInjury
        {
            InjuryId = injuryId,
            DaysRemaining = days
        });
    }

    public void TickInjuries()
    {
        _data.Injuries.RemoveAll(i =>
        {
            i.DaysRemaining--;
            return i.DaysRemaining <= 0;
        });
    }

    public Combat.CombatUnit ToCombatUnit()
    {
        var unit = new Combat.CombatUnit
        {
            UnitName = _data.Name,
            Team = Combat.Team.Player,
            Hitpoints = (int)GetStat("Hitpoints"),
            MaxHitpoints = (int)GetStat("Hitpoints"),
            Initiative = (int)GetStat("Initiative"),
            MeleeSkill = (int)GetStat("MeleeSkill"),
            RangedSkill = (int)GetStat("RangedSkill"),
            MeleeDefense = (int)GetStat("MeleeDefense"),
            RangedDefense = (int)GetStat("RangedDefense"),
            Fatigue = 0,
            MaxFatigue = (int)GetStat("Fatigue"),
            Resolve = (int)GetStat("Resolve"),
            ActionPoints = 9,
            MaxActionPoints = 9,
            MainHandWeapon = _data.MainHandId != null ? _dataLoader.GetWeapon(_data.MainHandId) : null,
            BodyArmor = _data.BodyArmorId != null ? _dataLoader.GetArmor(_data.BodyArmorId)?.Armor ?? 0 : 0,
            HeadArmor = _data.HeadArmorId != null ? _dataLoader.GetArmor(_data.HeadArmorId)?.Armor ?? 0 : 0,
            CharacterData = _data
        };

        return unit;
    }
}
