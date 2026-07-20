using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MercenaryBand.Data;
using MercenaryBand.Core;
using MercenaryBand.Systems;
using MercenaryBand.UI;
using static MercenaryBand.UI.Theme;

namespace MercenaryBand.Combat;

public partial class CombatScene : Node2D
{
    private HexGrid _hexGrid = null!;
    private HexRenderer _hexRenderer = null!;
    private TurnOrder _turnOrder = null!;
    private Pathfinding _pathfinding = null!;
    private CombatCalculator _calculator = null!;
    private readonly List<CombatUnit> _units = new();
    private readonly Dictionary<string, UnitRenderer> _unitRenderers = new();
    private HudDisplay? _hud;
    private GameHud? _gameHud;
    private bool _combatActive;
    private Label? _turnLabel;

    [Export] public int MapRadius { get; set; } = 5;
    [Export] public float HexSize { get; set; } = 34f;
    public bool AutoPlay { get; set; } = true;

    public override void _Ready()
    {
        _hexGrid = new HexGrid { HexSize = HexSize, Radius = MapRadius };
        AddChild(_hexGrid);
        _hexGrid.GenerateCircularMap(MapRadius);

        _hexRenderer = new HexRenderer();
        AddChild(_hexRenderer);
        _hexRenderer.SetHexSize(HexSize);
        _hexRenderer.SetOffset(GetViewportRect().Size / 2f);
        _hexRenderer.QueueAll(_hexGrid.AllTiles);
        _hexRenderer.Flush();

        _turnOrder = new TurnOrder();
        _calculator = new CombatCalculator();
        _pathfinding = new Pathfinding(_hexGrid);

        _hud = GetNodeOrNull<HudDisplay>("/root/Main/Hud");
        _gameHud = GetNodeOrNull<UI.GameHud>("/root/Main/GameHud");

        _turnLabel = new Label
        {
            Position = new Vector2(20, 540)
        };
        _turnLabel.AddThemeColorOverride("font_color", Gold);
        _turnLabel.AddThemeFontSizeOverride("font_size", 16);
        AddChild(_turnLabel);

        Log("Combat Ready - " + _hexGrid.AllTiles.Count() + " tiles");
    }

    public void AddUnit(CombatUnit unit)
    {
        _units.Add(unit);
        var renderer = new UnitRenderer();
        AddChild(renderer);
        renderer.UpdateFromUnit(unit, HexSize);
        _unitRenderers[unit.UnitName] = renderer;
    }

    public CombatUnit? GetUnitAt(HexCoord coord) =>
        _units.FirstOrDefault(u => u.HexPosition == coord && !u.IsDead);

    public void StartCombat()
    {
        _combatActive = true;
        _turnOrder.Initialize(_units);
        Log("Battle begins!");
        CallDeferred(nameof(ProcessTurn));
    }

    private void ProcessTurn()
    {
        if (!_combatActive) return;
        if (!_turnOrder.SkipToNextAlive()) { EndCombat(); return; }

        var unit = _turnOrder.CurrentUnit!;
        unit.ResetTurn();
        if (_turnLabel != null)
            _turnLabel.Text = $"Turn: {unit.UnitName}  |  Round {_turnOrder.TurnNumber}";

        if (AutoPlay)
            SimulateAITurn(unit);
    }

    private async void SimulateAITurn(CombatUnit unit)
    {
        await ToSignal(GetTree().CreateTimer(0.4f), "timeout");

        var enemies = _units.Where(u => u.Team != unit.Team && !u.IsDead).ToList();
        if (enemies.Count == 0) { EndCombat(); return; }

        var closest = enemies.OrderBy(e => unit.HexPosition.DistanceTo(e.HexPosition)).First();
        int dist = unit.HexPosition.DistanceTo(closest.HexPosition);

        if (dist <= 1)
        {
            unit.PerformAttack(closest, _calculator);
            Log($"  {unit.UnitName} strikes {closest.UnitName}!");
        }
        else if (dist <= 4 && unit.ActionPoints >= 2)
        {
            var path = _pathfinding.FindPath(unit.HexPosition, closest.HexPosition, unit.ActionPoints);
            if (path != null && path.Count > 1)
            {
                unit.MoveTo(path[1], _hexGrid.GetMoveCost(path[1]));
                Log($"  {unit.UnitName} advances");
            }
        }

        UpdateAllRenderers();

        if (_turnOrder.IsCombatOver())
        {
            await ToSignal(GetTree().CreateTimer(0.6f), "timeout");
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
        Log($"Victory! {winner} side wins.");

        if (_turnLabel != null)
            _turnLabel.Text = $"Battle Over - {winner} wins";

        _gameHud?.SetGold(_gameHud.Gold + 150);
        _gameHud?.Refresh();
    }

    private void Log(string msg)
    {
        _hud?.AddLog(msg);
        GD.Print($"[Combat] {msg}");
    }

    public void SpawnTestCombat()
    {
        var dm = GetNode<DataLoader>("/root/DataManager");

        var player = new CombatUnit
        {
            UnitName = "Merc Captain",
            Team = Team.Player,
            HexPosition = new HexCoord(-2, 1),
            Hitpoints = 75, MaxHitpoints = 75,
            MeleeSkill = 62, MeleeDefense = 12,
            Initiative = 115,
            MainHandWeapon = dm.GetWeapon("woodcutter_axe"),
            BodyArmor = 35
        };
        AddUnit(player);

        var ally1 = new CombatUnit
        {
            UnitName = "Man-at-Arms",
            Team = Team.Player,
            HexPosition = new HexCoord(-3, 2),
            Hitpoints = 65, MaxHitpoints = 65,
            MeleeSkill = 55, MeleeDefense = 8,
            Initiative = 105,
            MainHandWeapon = dm.GetWeapon("militia_spear"),
            BodyArmor = 25
        };
        AddUnit(ally1);

        var enemy1 = new CombatUnit
        {
            UnitName = "Bandit",
            Team = Team.Enemy,
            HexPosition = new HexCoord(2, -1),
            Hitpoints = 50, MaxHitpoints = 50,
            MeleeSkill = 50, MeleeDefense = 5,
            Initiative = 100,
            MainHandWeapon = dm.GetWeapon("shortsword"),
            BodyArmor = 10
        };
        AddUnit(enemy1);

        var enemy2 = new CombatUnit
        {
            UnitName = "Bandit Archer",
            Team = Team.Enemy,
            HexPosition = new HexCoord(1, 1),
            Hitpoints = 40, MaxHitpoints = 40,
            MeleeSkill = 42, MeleeDefense = 2,
            Initiative = 108,
            MainHandWeapon = dm.GetWeapon("shortsword"),
            BodyArmor = 5
        };
        AddUnit(enemy2);

        var enemy3 = new CombatUnit
        {
            UnitName = "Bandit Thug",
            Team = Team.Enemy,
            HexPosition = new HexCoord(3, -2),
            Hitpoints = 55, MaxHitpoints = 55,
            MeleeSkill = 52, MeleeDefense = 6,
            Initiative = 98,
            MainHandWeapon = dm.GetWeapon("woodcutter_axe"),
            BodyArmor = 15
        };
        AddUnit(enemy3);

        StartCombat();
    }
}
