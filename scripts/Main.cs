using Godot;

namespace MercenaryBand.Scripts;

public partial class Main : Control
{
    public UI.HudDisplay Hud { get; private set; } = null!;

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;

        Hud = new UI.HudDisplay();
        Hud.Name = "Hud";
        AddChild(Hud);

        Hud.AddLog("=== MercenaryBand ===");
        Hud.AddLog("Click map to move party");
        Hud.AddLog("Enter settlements for towns");
    }
}
