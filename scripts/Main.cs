using Godot;
using MercenaryBand.Core;
using MercenaryBand.Combat;
using MercenaryBand.Systems;

namespace MercenaryBand.Scripts;

public partial class Main : Node2D
{
    public override void _Ready()
    {
        var dataManager = GetNode<DataLoader>("/root/DataManager");

        GD.Print("============================================");
        GD.Print("  MercenaryBand - System Integration Test");
        GD.Print("============================================");

        TestDataLoading(dataManager);
        TestHexSystem();
        TestCharacterCreation(dataManager);
        TestCombat(dataManager);

        GD.Print("============================================");
        GD.Print("  All Tests Complete!");
        GD.Print("============================================");
    }

    private void TestDataLoading(DataLoader dm)
    {
        GD.Print("\n--- Test: Data Loading ---");
        GD.Print($"  Backgrounds: {System.Linq.Enumerable.Count(dm.AllBackgrounds)}");
        GD.Print($"  Perks:       {System.Linq.Enumerable.Count(dm.AllPerks)}");
        GD.Print($"  Traits:      {System.Linq.Enumerable.Count(dm.AllTraits)}");
        GD.Print($"  Injuries:    {System.Linq.Enumerable.Count(dm.AllInjuries)}");
        GD.Print($"  Weapons:     {System.Linq.Enumerable.Count(dm.AllWeapons)}");
        GD.Print($"  Armors:      {System.Linq.Enumerable.Count(dm.AllArmors)}");
    }

    private void TestHexSystem()
    {
        GD.Print("\n--- Test: Hex System ---");

        var grid = new HexGrid { HexSize = 32f, Radius = 4 };
        grid.GenerateCircularMap(4);
        GD.Print($"  Tiles generated: {System.Linq.Enumerable.Count(grid.AllTiles)}");

        var pathfinding = new Pathfinding(grid);
        var start = new HexCoord(0, 0);
        var target = new HexCoord(3, -1);
        int dist = start.DistanceTo(target);
        GD.Print($"  Distance from {start} to {target}: {dist}");

        var reachable = pathfinding.GetReachableTiles(start, 6);
        GD.Print($"  Reachable tiles (AP=6): {reachable.Count}");

        var path = pathfinding.FindPath(start, target, 9);
        GD.Print($"  Path found: {(path != null ? $"{path.Count} steps" : "none")}");
    }

    private void TestCharacterCreation(DataLoader dm)
    {
        GD.Print("\n--- Test: Character Creation ---");

        var factory = new CharacterFactory(dm);
        var levelUp = new LevelUpSystem(dm);

        var merc = factory.Create("farmhand");
        var sheet = new CharacterSheet(merc, dm);
        GD.Print($"  Created: {merc.Name} (Background: {merc.BackgroundId})");
        GD.Print($"  Stats: HP={sheet.GetStat("Hitpoints")}, MSkill={sheet.GetStat("MeleeSkill")}, MDef={sheet.GetStat("MeleeDefense")}, Init={sheet.GetStat("Initiative")}");

        sheet.EquipMainHand("woodcutter_axe");
        sheet.EquipBody("leather_armor");

        GD.Print($"  Equipped: {merc.MainHandId}, Body: {merc.BodyArmorId}");

        var result = levelUp.ApplyExperience(merc, 600);
        GD.Print($"  XP: 600 -> Level {merc.Level} (was {result?.OldLevel ?? merc.Level})");
        if (result?.AvailablePerks != null)
        {
            foreach (var perk in result.AvailablePerks)
                GD.Print($"    Available perk: {perk.Name} (Tier {perk.Tier})");
        }

        if (result?.AvailablePerks.Count > 0)
        {
            levelUp.ApplyPerk(merc, result.AvailablePerks[0].Id);
        }
    }

    private void TestCombat(DataLoader dm)
    {
        GD.Print("\n--- Test: Combat Simulation ---");

        var scene = new CombatScene();
        AddChild(scene);

        var player = new CombatUnit
        {
            UnitName = "용병 대장",
            Team = Team.Player,
            HexPosition = new HexCoord(0, 0),
            Hitpoints = 70,
            MaxHitpoints = 70,
            MeleeSkill = 60,
            MeleeDefense = 10,
            Initiative = 110,
            MainHandWeapon = dm.GetWeapon("shortsword"),
            BodyArmor = 30
        };
        scene.AddUnit(player);

        var enemy = new CombatUnit
        {
            UnitName = "도적",
            Team = Team.Enemy,
            HexPosition = new HexCoord(1, 0),
            Hitpoints = 45,
            MaxHitpoints = 45,
            MeleeSkill = 50,
            MeleeDefense = 5,
            Initiative = 100,
            MainHandWeapon = dm.GetWeapon("shortsword"),
            BodyArmor = 10
        };
        scene.AddUnit(enemy);

        GD.Print($"  Player HP: {player.Hitpoints}/{player.MaxHitpoints}");
        GD.Print($"  Enemy  HP: {enemy.Hitpoints}/{enemy.MaxHitpoints}");

        var calc = new CombatCalculator();
        GD.Print("  --- Simulating attacks ---");
        for (int i = 0; i < 5; i++)
        {
            calc.CalculateMeleeHit(player, enemy);
            if (enemy.IsDead) break;

            calc.CalculateMeleeHit(enemy, player);
            if (player.IsDead) break;
        }

        GD.Print($"  Result: Player HP={player.Hitpoints}, Enemy HP={enemy.Hitpoints}");
        GD.Print($"  Player dead: {player.IsDead}, Enemy dead: {enemy.IsDead}");
    }
}
