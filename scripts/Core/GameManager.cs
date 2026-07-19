using Godot;

namespace MercenaryBand.Core;

public enum GameMode
{
    MainMenu,
    Overworld,
    Combat,
    Town,
    Camp
}

public partial class GameManager : Node
{
    public GameMode CurrentMode { get; private set; } = GameMode.MainMenu;

    public override void _Ready()
    {
        GD.Print("[GameManager] Initialized");
    }

    public void ChangeMode(GameMode newMode)
    {
        var prev = CurrentMode;
        CurrentMode = newMode;
        GD.Print($"[GameManager] Mode changed: {prev} -> {newMode}");
    }
}
