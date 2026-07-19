using Godot;

namespace MercenaryBand.Scripts;

public partial class Main : Control
{
    public UI.HudDisplay Hud { get; private set; } = null!;

    public override void _Ready()
    {
        var bg = new ColorRect
        {
            Size = new Vector2(800, 600),
            Color = new Color(0.1f, 0.1f, 0.15f)
        };
        AddChild(bg);

        Hud = new UI.HudDisplay();
        Hud.Name = "Hud";
        AddChild(Hud);

        Hud.AddLog("=== MercenaryBand ===");
        Hud.AddLog("Click map to move party");
        Hud.AddLog("Enter settlements for towns");
    }
}
