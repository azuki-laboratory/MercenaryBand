using System.Collections.Generic;
using System.Linq;
using Godot;
using MercenaryBand.Core;
using MercenaryBand.Data;
using MercenaryBand.UI;
using static MercenaryBand.UI.Theme;

namespace MercenaryBand.Overworld;

public partial class WorldMap : Node2D
{
    private readonly List<SettlementNode> _settlements = new();
    private PartyController _party = null!;
    private TimeSystem _timeSystem = null!;
    private DataLoader? _data;
    private GameHud? _gameHud;
    private Node2D _mapLayer = null!;
    private int _gold = 500;
    private int _food = 30;

    public Vector2 PartyPosition => _party?.GlobalPosition ?? Vector2.Zero;
    public TimeData Time => _timeSystem?.Time ?? new TimeData();

    public override void _Ready()
    {
        _data = GetNode<DataLoader>("/root/DataManager");
        _gameHud = GetNodeOrNull<UI.GameHud>("/root/Main/GameHud");

        _timeSystem = new TimeSystem();
        AddChild(_timeSystem);

        DrawMapBackground();
        DrawRoads();
        SpawnSettlements();
        SpawnParty();

        CallDeferred(nameof(UpdateHud));
    }

    private void DrawMapBackground()
    {
        var bg = new ColorRect
        {
            Size = new Vector2(1024, 600),
            Color = MapLand
        };
        AddChild(bg);

        var center = GetViewportRect().Size / 2f;
        for (int i = 0; i < 20; i++)
        {
            var grass = new ColorRect
            {
                Size = new Vector2(GD.RandRange(18, 40), GD.RandRange(12, 28)),
                Position = new Vector2(GD.RandRange(0, 1000), GD.RandRange(0, 580)),
                Color = new Color(0.22f, 0.28f, 0.16f, 0.3f)
            };
            AddChild(grass);
        }

        var title = new Label
        {
            Text = "THE REALM",
            Position = new Vector2(860, 12)
        };
        title.AddThemeColorOverride("font_color", new Color(Gold, 0.3f));
        title.AddThemeFontSizeOverride("font_size", 22);
        AddChild(title);
    }

    private void DrawRoads()
    {
        if (_data == null) return;
        var settlements = _data.AllSettlements.ToList();

        for (int i = 0; i < settlements.Count; i++)
        {
            var rng = new System.Random(i * 7 + 3);
            int connections = rng.Next(1, 3);
            var others = settlements.Where((_, j) => j != i).OrderBy(_ => rng.Next()).Take(connections);

            foreach (var other in others)
            {
                var from = SettlementNode.SettlePos(settlements[i]);
                var to = SettlementNode.SettlePos(other);

                var road = new ColorRect
                {
                    Color = new Color(0.40f, 0.35f, 0.22f, 0.35f),
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };

                Vector2 dir = to - from;
                float length = dir.Length();
                road.Size = new Vector2(length, 2.5f);
                road.Position = from;
                road.PivotOffset = Vector2.Zero;
                road.Rotation = dir.Angle();

                var roadCast = new Sprite2D();
                AddChild(roadCast);
                roadCast.AddChild(new Control { MouseFilter = Control.MouseFilterEnum.Ignore });
            }
        }
    }

    private void SpawnSettlements()
    {
        foreach (var def in _data!.AllSettlements)
        {
            var node = new SettlementNode(def);
            AddChild(node);
            _settlements.Add(node);
        }
    }

    private void SpawnParty()
    {
        _party = new PartyController { Speed = 100f };
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
            UpdateHud();
        }
    }

    public override void _Process(double delta)
    {
        _party?.UpdatePosition((float)delta);

        var nearby = GetNearestSettlement(PartyPosition, 60f);
        if (nearby != null && !_party.IsMoving)
        {
            if (_gameHud != null)
                _gameHud.InfoText = $"At {nearby.Def.Name} - Click to enter";
        }
        else if (_party.IsMoving)
        {
            if (_gameHud != null)
                _gameHud.InfoText = "Travelling...";
        }

        UpdateHud();
    }

    private void OnArriveAtSettlement()
    {
        var settlement = GetNearestSettlement(PartyPosition, 80f);
        if (settlement != null)
        {
            GD.Print($"[WorldMap] Arrived at {settlement.Def.Name}");
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

    private void UpdateHud()
    {
        if (_gameHud == null) return;
        _gameHud.SetGold(_gold);
        _gameHud.SetFood(_food);
        _gameHud.SetTime(_timeSystem.Time.TimeString);
        _gameHud.SetParty(3);
        _gameHud.Refresh();
    }

    public ContractDef? GetContract(string id) => _data?.GetContract(id);
    public DataLoader? Data => _data;
}
