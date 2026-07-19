using System.Collections.Generic;
using System.Linq;
using Godot;
using MercenaryBand.Data;

namespace MercenaryBand.Combat;

public partial class CombatScene : Node2D
{
    private HexGrid _hexGrid = null!;
    private TurnOrder _turnOrder = null!;
    private Pathfinding _pathfinding = null!;
    private CombatCalculator _calculator = null!;
    private ActionResolver _actionResolver = null!;
    private readonly List<CombatUnit> _units = new();

    [Export] public int MapRadius { get; set; } = 6;

    public override void _Ready()
    {
        _hexGrid = new HexGrid { HexSize = 32f, Radius = MapRadius };
        AddChild(_hexGrid);
        _hexGrid.GenerateCircularMap(MapRadius);

        _turnOrder = new TurnOrder();
        _calculator = new CombatCalculator();
        _pathfinding = new Pathfinding(_hexGrid);
        _actionResolver = new ActionResolver(_pathfinding, _calculator);

        GD.Print("==========================================");
        GD.Print("  CombatScene Ready");
        GD.Print($"  Map: {_hexGrid.AllTiles.Count()} hex tiles");
        GD.Print("==========================================");
    }

    public void AddUnit(CombatUnit unit)
    {
        _units.Add(unit);
        AddChild(unit);
        GD.Print($"[CombatScene] Unit added: {unit.UnitName} ({unit.Team})");
    }

    public CombatUnit? GetUnitAt(HexCoord coord) =>
        _units.FirstOrDefault(u => u.HexPosition == coord && !u.IsDead);

    public void StartCombat()
    {
        _turnOrder.Initialize(_units);
        GD.Print("[CombatScene] Combat started!");
        ProcessCurrentTurn();
    }

    private void ProcessCurrentTurn()
    {
        if (!_turnOrder.SkipToNextAlive())
        {
            EndCombat();
            return;
        }

        var unit = _turnOrder.CurrentUnit!;
        unit.ResetTurn();
        GD.Print($"[CombatScene] Turn: {unit.UnitName} (AP: {unit.ActionPoints})");
        SimulateAITurn(unit);
    }

    private void SimulateAITurn(CombatUnit unit)
    {
        var actions = _actionResolver.GetAvailableActions(unit, _hexGrid);
        var enemies = _units.Where(u => u.Team != unit.Team && !u.IsDead).ToList();

        if (enemies.Count == 0)
        {
            EndCombat();
            return;
        }

        var attackAction = actions.FirstOrDefault(a => a.Type == ActionType.AttackMelee);
        if (attackAction != null)
        {
            unit.PerformAttack(attackAction.TargetUnit!, _calculator);
        }
        else
        {
            GD.Print($"[CombatScene] {unit.UnitName} passes");
        }

        if (_turnOrder.IsCombatOver())
        {
            EndCombat();
            return;
        }

        _turnOrder.AdvanceTurn();
        ProcessCurrentTurn();
    }

    private void EndCombat()
    {
        var alive = _units.Where(u => !u.IsDead).ToList();
        var winner = alive.FirstOrDefault()?.Team;
        GD.Print($"[CombatScene] Combat ended! Winner: {winner}");
    }

    public void SpawnTestCombat()
    {
        var dataManager = GetNode<Core.DataLoader>("/root/DataManager");
        var farmhand = dataManager.GetBackground("farmhand");

        var player = new CombatUnit
        {
            UnitName = "용병 대장",
            Team = Team.Player,
            HexPosition = new HexCoord(-2, 1),
            Hitpoints = 70,
            MaxHitpoints = 70,
            MeleeSkill = 60,
            MeleeDefense = 10,
            Initiative = 110,
            MainHandWeapon = dataManager.GetWeapon("shortsword"),
            BodyArmor = 30
        };
        AddUnit(player);

        var enemy1 = new CombatUnit
        {
            UnitName = "도적",
            Team = Team.Enemy,
            HexPosition = new HexCoord(2, -1),
            Hitpoints = 45,
            MaxHitpoints = 45,
            MeleeSkill = 50,
            MeleeDefense = 5,
            Initiative = 100,
            MainHandWeapon = dataManager.GetWeapon("shortsword"),
            BodyArmor = 10
        };
        AddUnit(enemy1);

        var enemy2 = new CombatUnit
        {
            UnitName = "도적 궁수",
            Team = Team.Enemy,
            HexPosition = new HexCoord(1, 1),
            Hitpoints = 35,
            MaxHitpoints = 35,
            MeleeSkill = 40,
            MeleeDefense = 0,
            Initiative = 105,
            MainHandWeapon = dataManager.GetWeapon("shortsword"),
            BodyArmor = 5
        };
        AddUnit(enemy2);

        StartCombat();
    }
}
