using Godot;

namespace MercenaryBand.Scripts;

public partial class Main : Control
{
    public UI.HudDisplay Hud { get; private set; } = null!;
    public UI.GameHud GameHud { get; private set; } = null!;

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;

        Hud = new UI.HudDisplay();
        Hud.Name = "Hud";
        AddChild(Hud);

        GameHud = new UI.GameHud();
        GameHud.Name = "GameHud";
        AddChild(GameHud);
    }

    public void ShowGameHud(bool show)
    {
        if (GameHud != null) GameHud.Visible = show;
    }
}
