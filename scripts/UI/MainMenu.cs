using Godot;
using static MercenaryBand.UI.Theme;

namespace MercenaryBand.UI;

public partial class MainMenu : Control
{
    [Signal] public delegate void NewGameEventHandler();
    [Signal] public delegate void ContinueEventHandler();
    [Signal] public delegate void QuitEventHandler();

    public override void _Ready()
    {
        var bg = MakeBackground();
        AddChild(bg);

        var accentLine = new ColorRect
        {
            Size = new Vector2(600, 1),
            Position = new Vector2(212, 110),
            Color = Gold
        };
        AddChild(accentLine);

        var title = MakeTitle("MERCENARY BAND", 40);
        title.Position = new Vector2(0, 75);
        title.Size = new Vector2(1024, 40);
        AddChild(title);

        var accentLine2 = new ColorRect
        {
            Size = new Vector2(600, 1),
            Position = new Vector2(212, 125),
            Color = Gold
        };
        AddChild(accentLine2);

        var version = MakeLabel("v0.1.0 — Godot 4.7 C#", 10);
        version.Position = new Vector2(0, 132);
        version.Size = new Vector2(1024, 20);
        version.HorizontalAlignment = HorizontalAlignment.Center;
        version.AddThemeColorOverride("font_color", TextDim);
        AddChild(version);

        var menuPanel = MakePanel(new Vector2(280, 300));
        menuPanel.Position = new Vector2(372, 170);
        AddChild(menuPanel);

        var btnNew = MakeButton("  New Game  ");
        btnNew.Position = new Vector2(30, 20);
        btnNew.CustomMinimumSize = new Vector2(220, 44);
        btnNew.Pressed += () => EmitSignal(SignalName.NewGame);
        menuPanel.AddChild(btnNew);

        var btnContinue = MakeButton("  Continue  ");
        btnContinue.Position = new Vector2(30, 80);
        btnContinue.CustomMinimumSize = new Vector2(220, 44);
        btnContinue.Pressed += () => EmitSignal(SignalName.Continue);
        menuPanel.AddChild(btnContinue);

        var btnSettings = MakeButton("  Settings  ");
        btnSettings.Position = new Vector2(30, 140);
        btnSettings.CustomMinimumSize = new Vector2(220, 44);
        btnSettings.Pressed += () => GD.Print("[MainMenu] Settings clicked");
        menuPanel.AddChild(btnSettings);

        var btnQuit = MakeButton("  Quit  ");
        btnQuit.Position = new Vector2(30, 200);
        btnQuit.CustomMinimumSize = new Vector2(220, 44);
        btnQuit.AddThemeColorOverride("font_color", AccentRed);
        btnQuit.Pressed += () => EmitSignal(SignalName.Quit);
        menuPanel.AddChild(btnQuit);

        var footer = MakeLabel("azuki-laboratory / MercenaryBand", 10);
        footer.Position = new Vector2(0, 570);
        footer.Size = new Vector2(1024, 20);
        footer.HorizontalAlignment = HorizontalAlignment.Center;
        footer.AddThemeColorOverride("font_color", TextDim);
        AddChild(footer);
    }
}
