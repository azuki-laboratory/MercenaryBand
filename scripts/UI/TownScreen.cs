using System;
using Godot;
using MercenaryBand.Data;
using MercenaryBand.Core;
using MercenaryBand.Systems;
using static MercenaryBand.UI.Theme;

namespace MercenaryBand.UI;

public partial class TownScreen : Control
{
    private SettlementDef _settlement = null!;
    private HudDisplay? _hud;
    private GameHud? _gameHud;
    private VBoxContainer _menuContainer = null!;
    private Control? _currentPanel;

    [Signal] public delegate void LeaveTownEventHandler();

    public override void _Ready()
    {
        var bg = MakeBackground();
        AddChild(bg);

        _hud = GetNodeOrNull<HudDisplay>("/root/Main/Hud");
        _gameHud = GetNodeOrNull<GameHud>("/root/Main/GameHud");

        var panel = MakePanel(new Vector2(340, 480));
        panel.Position = new Vector2(30, 80);
        AddChild(panel);

        _menuContainer = new VBoxContainer
        {
            Position = new Vector2(50, 100)
        };
        _menuContainer.AddThemeConstantOverride("separation", 8);
        panel.AddChild(_menuContainer);

        SetupBaseMenu();
    }

    public void SetSettlement(SettlementDef def)
    {
        _settlement = def;

        foreach (var child in GetChildren())
        {
            if (child is Label titleLabel && titleLabel.Name?.ToString() == "SettleTitle")
            {
                titleLabel.QueueFree();
                break;
            }
        }

        var title = MakeTitle($"  {def.Name}  ", 24);
        title.Name = "SettleTitle";
        title.Position = new Vector2(0, 20);
        title.Size = new Vector2(400, 40);
        AddChild(title);

        var typeLabel = MakeLabel($"{def.Type}  |  Pop: {def.Population}  |  Defense: {def.Defense}", 13);
        typeLabel.Position = new Vector2(0, 52);
        typeLabel.Size = new Vector2(400, 20);
        typeLabel.HorizontalAlignment = HorizontalAlignment.Center;
        AddChild(typeLabel);

        var accentLine = new ColorRect
        {
            Size = new Vector2(320, 1),
            Position = new Vector2(40, 74),
            Color = new Color(Gold, 0.3f)
        };
        AddChild(accentLine);

        Log($"Entered {def.Name}");
    }

    private void SetupBaseMenu()
    {
        ClearMenu();
        AddMenuButton("  Recruit Mercenaries  ", ShowRecruitPanel);
        AddMenuButton("  Trade Equipment  ", ShowTradePanel);
        AddMenuButton("  View Contracts  ", ShowContractPanel);
        AddMenuButton("  Leave Settlement  ", OnLeave);
    }

    private void ClearMenu()
    {
        _currentPanel?.QueueFree();
        _currentPanel = null;
        foreach (var child in _menuContainer.GetChildren())
            child.QueueFree();
    }

    private void AddMenuButton(string text, Action action)
    {
        var btn = MakeButton(text);
        btn.CustomMinimumSize = new Vector2(290, 40);
        btn.Pressed += () => action();
        _menuContainer.AddChild(btn);
    }

    private void ShowRecruitPanel()
    {
        ClearPanel();
        _currentPanel = new RecruitPanel();
        _menuContainer.AddChild(_currentPanel);
        var data = GetNode<DataLoader>("/root/DataManager");
        ((RecruitPanel)_currentPanel).ShowRecruits(data, 3);
    }

    private void ShowTradePanel()
    {
        ClearPanel();
        _currentPanel = new TradePanel();
        _menuContainer.AddChild(_currentPanel);
        var data = GetNode<DataLoader>("/root/DataManager");
        ((TradePanel)_currentPanel).ShowItems(data);
    }

    private void ShowContractPanel()
    {
        ClearPanel();
        _currentPanel = new ContractPanel();
        _menuContainer.AddChild(_currentPanel);
        var data = GetNode<DataLoader>("/root/DataManager");
        ((ContractPanel)_currentPanel).ShowContracts(data, _settlement.Id);
    }

    private void ClearPanel() { _currentPanel?.QueueFree(); }

    private void OnLeave()
    {
        Log($"Left {_settlement.Name}");
        EmitSignal(SignalName.LeaveTown);
    }

    private void Log(string msg)
    {
        _hud?.AddLog(msg);
        GD.Print($"[Town] {msg}");
    }
}
