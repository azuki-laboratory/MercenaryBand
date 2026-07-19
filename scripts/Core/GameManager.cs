using Godot;
using MercenaryBand.Data;
using MercenaryBand.UI;
using MercenaryBand.Overworld;
using MercenaryBand.Combat;

namespace MercenaryBand.Core;

public enum GameMode
{
    MainMenu,
    Overworld,
    Combat,
    Town,
    Camp
}

public partial class GameManager : Node2D
{
    public GameMode CurrentMode { get; private set; } = GameMode.Overworld;
    public SettlementDef? CurrentSettlement { get; set; }

    private Node? _currentScene;
    private CombatScene? _combatScene;

    public override void _Ready()
    {
        GD.Print("[GameManager] Initialized - starting Overworld");
        ChangeMode(GameMode.Overworld);
    }

    public void ChangeMode(GameMode newMode)
    {
        var prev = CurrentMode;
        CurrentMode = newMode;
        GD.Print($"[GameManager] Mode: {prev} -> {newMode}");

        _currentScene?.QueueFree();

        switch (newMode)
        {
            case GameMode.Overworld:
                var worldMap = new WorldMap();
                _currentScene = worldMap;
                break;
            case GameMode.Combat:
                _combatScene = new CombatScene { AutoPlay = true };
                _currentScene = _combatScene;
                break;
            case GameMode.Town:
                var town = new TownScreen();
                town.LeaveTown += () => ChangeMode(GameMode.Overworld);
                if (CurrentSettlement != null)
                    town.SetSettlement(CurrentSettlement);
                _currentScene = town;
                break;
        }

        if (_currentScene != null)
            AddChild(_currentScene);
    }

    public void StartCombat()
    {
        ChangeMode(GameMode.Combat);
        _combatScene?.CallDeferred(nameof(CombatScene.SpawnTestCombat));
    }
}
