using System;
using Godot;
using MercenaryBand.Data;
using MercenaryBand.Core;
using MercenaryBand.Systems;

namespace MercenaryBand.UI;

public partial class TownScreen : Control
{
    private SettlementDef _settlement = null!;
    private HudDisplay? _hud;
    private VBoxContainer _menuContainer = null!;
    private Control? _currentPanel;

    [Signal] public delegate void LeaveTownEventHandler();

    public override void _Ready()
    {
        _hud = GetNodeOrNull<HudDisplay>("/root/Main/Hud");

        _menuContainer = new VBoxContainer
        {
            Position = new Vector2(420, 50),
            Size = new Vector2(350, 500)
        };
        AddChild(_menuContainer);

        SetupBaseMenu();
    }

    public void SetSettlement(SettlementDef def)
    {
        _settlement = def;

        foreach (var child in GetChildren())
        {
            if (child is Label titleLabel)
            {
                titleLabel.QueueFree();
                break;
            }
        }

        var title = new Label
        {
            Text = $"-- {def.Name} ({def.Type}) --",
            HorizontalAlignment = HorizontalAlignment.Center,
            Position = new Vector2(0, 10),
            Size = new Vector2(800, 30)
        };
        title.AddThemeFontSizeOverride("font_size", 22);
        AddChild(title);

        Log($"Entered {def.Name} (Pop: {def.Population})");
    }

    private void SetupBaseMenu()
    {
        ClearMenu();

        AddButton("용병 모집", ShowRecruitPanel);
        AddButton("장비 상점", ShowTradePanel);
        AddButton("계약 의뢰", ShowContractPanel);
        AddButton("떠나기", OnLeave);
    }

    private void ClearMenu()
    {
        _currentPanel?.QueueFree();
        _currentPanel = null;
        foreach (var child in _menuContainer.GetChildren())
            child.QueueFree();
    }

    private Button AddButton(string text, Action action)
    {
        var btn = new Button
        {
            Text = text,
            CustomMinimumSize = new Vector2(300, 40)
        };
        btn.AddThemeFontSizeOverride("font_size", 16);
        btn.Pressed += () => action();
        _menuContainer.AddChild(btn);
        return btn;
    }

    private void ShowRecruitPanel()
    {
        ClearPanel();
        var panel = new RecruitPanel();
        panel.RecruitCompleted += (name) => { Log($"Recruited: {name}"); };
        _currentPanel = panel;
        _menuContainer.AddChild(panel);

        var data = GetNode<DataLoader>("/root/DataManager");
        panel.ShowRecruits(data, 3);
    }

    private void ShowTradePanel()
    {
        ClearPanel();
        var panel = new TradePanel();
        panel.ItemPurchased += (name, cost) => { Log($"Purchased: {name} ({cost}g)"); };
        _currentPanel = panel;
        _menuContainer.AddChild(panel);

        var data = GetNode<DataLoader>("/root/DataManager");
        panel.ShowItems(data);
    }

    private void ShowContractPanel()
    {
        ClearPanel();
        var panel = new ContractPanel();
        panel.ContractAccepted += (name, reward) => { Log($"Contract accepted: {name} ({reward}g)"); };
        _currentPanel = panel;
        _menuContainer.AddChild(panel);

        var data = GetNode<DataLoader>("/root/DataManager");
        panel.ShowContracts(data, _settlement.Id);
    }

    private void ClearPanel()
    {
        _currentPanel?.QueueFree();
    }

    private void OnLeave()
    {
        Log($"Left {_settlement.Name}");
        EmitSignal(SignalName.LeaveTown);
    }

    private void Log(string msg)
    {
        _hud?.AddLog(msg);
        GD.Print($"[TownScreen] {msg}");
    }
}
