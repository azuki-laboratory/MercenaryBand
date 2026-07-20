using Godot;
using MercenaryBand.Data;
using MercenaryBand.Overworld;
using MercenaryBand.Combat;
using MercenaryBand.UI;

namespace MercenaryBand.Core;

public enum GameMode { Splash, MainMenu, Overworld, Combat, Town, Camp }

public partial class GameManager : Node2D
{
    public GameMode CurrentMode { get; private set; } = GameMode.Splash;
    public SettlementDef? CurrentSettlement { get; set; }

    private Node? _currentScene;
    private CombatScene? _combatScene;
    private GameHud? _gameHud;

    public override void _Ready()
    {
        GD.Print("[GameManager] Start - SplashScreen");
        CallDeferred(nameof(ShowSplash));
    }

    public override void _Process(double delta)
    {
        if (_gameHud == null)
        {
            var main = GetTree().Root.GetNodeOrNull<Scripts.Main>("/root/Main");
            if (main != null)
                _gameHud = main.GameHud;
        }
    }

    public void ChangeMode(GameMode newMode)
    {
        var prev = CurrentMode;
        CurrentMode = newMode;
        GD.Print($"[GameManager] Mode: {prev} -> {newMode}");

        _currentScene?.QueueFree();
        _currentScene = null;

        switch (newMode)
        {
            case GameMode.Splash:     ShowSplash(); break;
            case GameMode.MainMenu:   ShowMainMenu(); break;
            case GameMode.Overworld:  ShowOverworld(); break;
            case GameMode.Combat:     ShowCombat(); break;
            case GameMode.Town:       ShowTown(); break;
        }

        if (_gameHud != null)
            _gameHud.Visible = newMode is GameMode.Overworld or GameMode.Combat or GameMode.Town;
    }

    private void ShowSplash()
    {
        var splash = new SplashScreen();
        splash.LoadingComplete += () => ChangeMode(GameMode.MainMenu);
        _currentScene = splash;
        AddChild(splash);
    }

    private void ShowMainMenu()
    {
        var menu = new MainMenu();
        menu.NewGame += () => ChangeMode(GameMode.Overworld);
        menu.Continue += () => ChangeMode(GameMode.Overworld);
        menu.Quit += () => GetTree().Quit();
        _currentScene = menu;
        AddChild(menu);
    }

    private void ShowOverworld() { AddScene(new WorldMap()); }
    private void ShowCombat()
    {
        _combatScene = new CombatScene { AutoPlay = true };
        AddScene(_combatScene);
        _combatScene.CallDeferred(nameof(CombatScene.SpawnTestCombat));
    }
    private void ShowTown()
    {
        var town = new TownScreen();
        town.LeaveTown += () => ChangeMode(GameMode.Overworld);
        if (CurrentSettlement != null)
            town.SetSettlement(CurrentSettlement);
        AddScene(town);
    }

    private void AddScene(Node scene)
    {
        _currentScene = scene;
        AddChild(scene);
    }

    public void StartCombat()
    {
        ChangeMode(GameMode.Combat);
    }
}
