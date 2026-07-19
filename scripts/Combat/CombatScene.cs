using System.Collections.Generic;
using System.Linq;
using Godot;
using MercenaryBand.Core;
using MercenaryBand.Data;
using MercenaryBand.Systems;
using MercenaryBand.UI;

namespace MercenaryBand.Combat;

public partial class CombatScene : Node2D
{
    private HexGrid _hexGrid = null!;
    private HexRenderer _hexRenderer = null!;
    private TurnOrder _turnOrder = null!;
    private Pathfinding _pathfinding = null!;
    private CombatCalculator _calculator = null!;
    private ActionResolver _actionResolver = null!;
    private readonly List<CombatUnit> _units = new();
    private readonly Dictionary<string, UnitRenderer> _unitRenderers = new();
    private HudDisplay? _hud;
    private bool _combatActive;

    [Export] public int MapRadius { get; set; } = 6;
    [Export] public float HexSize { get; set; } = 30f;
    public bool AutoPlay { get; set; } = true;

    public override void _Ready()
    {
        _hexGrid = new HexGrid { HexSize = HexSize, Radius = MapRadius };
        AddChild(_hexGrid);
        _hexGrid.GenerateCircularMap(MapRadius);

        _hexRenderer = new HexRenderer();
        AddChild(_hexRenderer);
        _hexRenderer.SetHexSize(HexSize);
        _hexRenderer.QueueAll(_hexGrid.AllTiles);
        _hexRenderer.Flush();

        _turnOrder = new TurnOrder();
        _calculator = new CombatCalculator();
        _pathfinding = new Pathfinding(_hexGrid);
        _actionResolver = new ActionResolver(_pathfinding, _calculator);

        _hud = GetNodeOrNull<HudDisplay>("/root/Main/Hud");

        Log("CombatScene Ready");
        Log($"  {_hexGrid.AllTiles.Count()} hex tiles");
    }

    public void AddUnit(CombatUnit unit)
    {
        _units.Add(unit);
        var renderer = new UnitRenderer();
        AddChild(renderer);
        renderer.UpdateFromUnit(unit, HexSize);
        _unitRenderers[unit.UnitName] = renderer;
        Log($"Unit added: {unit.UnitName} ({unit.Team})");
    }

    public CombatUnit? GetUnitAt(HexCoord coord) =>
        _units.FirstOrDefault(u => u.HexPosition == coord && !u.IsDead);

    public void StartCombat()
    {
        _combatActive = true;
        _turnOrder.Initialize(_units);
        Log("Combat started!");
        CallDeferred(nameof(ProcessTurn));
    }

    private void ProcessTurn()
    {
        if (!_combatActive) return;

        if (!_turnOrder.SkipToNextAlive())
        {
            EndCombat();
            return;
        }

        var unit = _turnOrder.CurrentUnit!;
        unit.ResetTurn();

        if (AutoPlay)
        {
            SimulateAITurn(unit);
        }
    }

    private async void SimulateAITurn(CombatUnit unit)
    {
        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

        var enemies = _units.Where(u => u.Team != unit.Team && !u.IsDead).ToList();
        if (enemies.Count == 0)
        {
            EndCombat();
            return;
        }

        var closest = enemies.OrderBy(e => unit.HexPosition.DistanceTo(e.HexPosition)).First();
        int dist = unit.HexPosition.DistanceTo(closest.HexPosition);

        if (dist == 1)
        {
            unit.PerformAttack(closest, _calculator);
            Log($"  {unit.UnitName} attacks {closest.UnitName}! HP: {closest.Hitpoints}/{closest.MaxHitpoints}");
        }
        else if (dist <= 3)
        {
            var path = _pathfinding.FindPath(unit.HexPosition, closest.HexPosition, unit.ActionPoints);
            if (path != null && path.Count > 1)
            {
                var next = path[1];
                unit.MoveTo(next, _hexGrid.GetMoveCost(next));
                Log($"  {unit.UnitName} moves to {next}");
            }
        }

        UpdateAllRenderers();

        if (_turnOrder.IsCombatOver())
        {
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
            EndCombat();
            return;
        }

        _turnOrder.AdvanceTurn();
        ProcessTurn();
    }

    private void UpdateAllRenderers()
    {
        foreach (var unit in _units)
        {
            if (_unitRenderers.TryGetValue(unit.UnitName, out var renderer))
                renderer.UpdateFromUnit(unit, HexSize);
        }
    }

    private void EndCombat()
    {
        _combatActive = false;
        var alive = _units.Where(u => !u.IsDead).ToList();
        var winner = alive.FirstOrDefault()?.Team;
        Log($"Combat ended! Winner: {winner}");
    }

    public void SpawnTestCombat()
    {
        var data = GetNode<DataLoader>("/root/DataManager");
        var factory = new CharacterFactory(data);

        var playerChar = factory.Create("farmhand", 1);
        var player = new CombatUnit
        {
            UnitName = playerChar.Name,
            Team = Team.Player,
            CharacterData = playerChar,
            HexPosition = new HexCoord(0, 0),
            Hitpoints = (int)playerChar.Stats["Hitpoints"],
            MaxHitpoints = (int)playerChar.Stats["Hitpoints"],
            MeleeSkill = (int)playerChar.Stats["MeleeSkill"],
            MeleeDefense = (int)playerChar.Stats["MeleeDefense"],
            Resolve = (int)playerChar.Stats["Resolve"],
            ActionPoints = (int)playerChar.Stats["ActionPoints"],
            MaxActionPoints = (int)playerChar.Stats["ActionPoints"],
            MaxFatigue = (int)playerChar.Stats["Fatigue"]
        };
        AddUnit(player);

        var enemyChar = factory.Create("apprentice", 1);
        var enemy = new CombatUnit
        {
            UnitName = enemyChar.Name,
            Team = Team.Enemy,
            CharacterData = enemyChar,
            HexPosition = new HexCoord(3, 0),
            Hitpoints = (int)enemyChar.Stats["Hitpoints"],
            MaxHitpoints = (int)enemyChar.Stats["Hitpoints"],
            MeleeSkill = (int)enemyChar.Stats["MeleeSkill"],
            MeleeDefense = (int)enemyChar.Stats["MeleeDefense"],
            Resolve = (int)enemyChar.Stats["Resolve"],
            ActionPoints = (int)enemyChar.Stats["ActionPoints"],
            MaxActionPoints = (int)enemyChar.Stats["ActionPoints"],
            MaxFatigue = (int)enemyChar.Stats["Fatigue"]
        };
        AddUnit(enemy);

        StartCombat();
    }

    private void Log(string msg)
    {
        _hud?.AddLog(msg);
        GD.Print($"[CombatScene] {msg}");
    }
}
