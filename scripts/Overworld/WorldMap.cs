using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MercenaryBand.Data;
using MercenaryBand.Core;
using MercenaryBand.UI;

namespace MercenaryBand.Overworld;

public partial class WorldMap : Node2D
{
    private readonly List<SettlementNode> _settlements = new();
    private PartyController _party = null!;
    private TimeSystem _timeSystem = null!;
    private HudDisplay? _hud;
    private DataLoader? _data;

    public Vector2 PartyPosition => _party?.GlobalPosition ?? Vector2.Zero;
    public TimeData Time => _timeSystem?.Time ?? new TimeData();

    public override void _Ready()
    {
        _hud = GetNodeOrNull<HudDisplay>("/root/Main/Hud");
        _data = GetNode<DataLoader>("/root/DataManager");

        _timeSystem = new TimeSystem();
        AddChild(_timeSystem);

        SetupBackground();
        SpawnSettlements();
        SpawnParty();

        Log("WorldMap Ready");
        Log($"  Settlements: {_settlements.Count}");
    }

    private void SetupBackground()
    {
        var bg = new ColorRect
        {
            Size = new Vector2(800, 600),
            Color = new Color(0.15f, 0.2f, 0.15f)
        };
        AddChild(bg);
    }

    private void SpawnSettlements()
    {
        foreach (var def in _data!.AllSettlements)
        {
            var node = new SettlementNode(def);
            AddChild(node);
            _settlements.Add(node);
            Log($"  Settlement: {def.Name} ({def.Type})");
        }
    }

    private void SpawnParty()
    {
        _party = new PartyController { Speed = 60f };
        AddChild(_party);
        if (_settlements.Count > 0)
            _party.GlobalPosition = _settlements[0].GlobalPosition;
        _party.DestinationReached += OnArriveAtSettlement;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
        {
            _party?.MoveTo(GetGlobalMousePosition());
        }
    }

    public override void _Process(double delta)
    {
        if (_party != null)
            _party.GlobalPosition = _party.GlobalPosition;

        var nearby = GetNearestSettlement(PartyPosition, 50f);
        if (nearby != null && !_party.IsMoving)
        {
            if (_hud != null)
                _hud.SetInfo($"Mat {nearby.Def.Name} - Click to enter");
        }
    }

    private void OnArriveAtSettlement()
    {
        var settlement = GetNearestSettlement(PartyPosition, 80f);
        if (settlement != null)
        {
            Log($"Arrived at {settlement.Def.Name}");
            OpenTown(settlement.Def);
        }
    }

    private void OpenTown(SettlementDef def)
    {
        var gameManager = GetNode<GameManager>("/root/GameState");
        gameManager.CurrentSettlement = def;
        gameManager.ChangeMode(GameMode.Town);
    }

    private SettlementNode? GetNearestSettlement(Vector2 pos, float maxDist)
    {
        SettlementNode? nearest = null;
        float minDist = maxDist;
        foreach (var s in _settlements)
        {
            float d = pos.DistanceTo(s.GlobalPosition);
            if (d < minDist)
            {
                minDist = d;
                nearest = s;
            }
        }
        return nearest;
    }

    private void Log(string msg)
    {
        _hud?.AddLog(msg);
        GD.Print($"[WorldMap] {msg}");
    }

    public ContractDef? GetContract(string id) => _data?.GetContract(id);
}
