using Godot;
using MercenaryBand.Data;
using MercenaryBand.Combat;
using static MercenaryBand.UI.Theme;

namespace MercenaryBand.Overworld;

public partial class SettlementNode : Node2D
{
    public SettlementDef Def { get; }
    private bool _mouseOver;
    private ColorRect _bg = null!;
    private Label _label = null!;
    private float _pulse;

    public SettlementNode(SettlementDef def)
    {
        Def = def;
        GlobalPosition = SettlePos(def);
    }

    public static Vector2 SettlePos(SettlementDef def)
    {
        return new HexCoord(def.Coord.Q, def.Coord.R).ToPixel(28f) + new Vector2(512, 310);
    }

    public override void _Ready()
    {
        int size = GetSettleSize();

        _bg = new ColorRect
        {
            Size = new Vector2(size + 4, size + 4),
            Color = new Color(GoldDim, 0.4f),
            Position = new Vector2(-(size + 4) / 2f, -(size + 4) / 2f)
        };
        AddChild(_bg);

        var inner = new ColorRect
        {
            Size = new Vector2(size, size),
            Color = GetSettleColor(),
            Position = new Vector2(-size / 2f, -size / 2f)
        };
        AddChild(inner);

        _label = new Label
        {
            Text = Def.Name,
            Position = new Vector2(-40, size / 2f + 4),
            Size = new Vector2(80, 16),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        _label.AddThemeColorOverride("font_color", TextPrimary);
        _label.AddThemeFontSizeOverride("font_size", 10);
        AddChild(_label);

        var typeLabel = new Label
        {
            Text = Def.Type,
            Position = new Vector2(-30, size / 2f + 19),
            Size = new Vector2(60, 14),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        typeLabel.AddThemeColorOverride("font_color", TextDim);
        typeLabel.AddThemeFontSizeOverride("font_size", 9);
        AddChild(typeLabel);
    }

    public override void _Process(double delta)
    {
        _pulse += (float)delta * 2f;
        float glow = 0.35f + Mathf.Sin(_pulse) * 0.1f;
        _bg.Color = new Color(GoldDim, glow);
    }

    private int GetSettleSize() => Def.Type switch
    {
        "City" => 30,
        "Fortress" => 24,
        "Town" => 18,
        _ => 13
    };

    private Color GetSettleColor() => Def.Type switch
    {
        "City" => new Color(0.65f, 0.55f, 0.25f),
        "Fortress" => new Color(0.50f, 0.25f, 0.25f),
        "Town" => new Color(0.45f, 0.42f, 0.28f),
        _ => new Color(0.30f, 0.35f, 0.22f)
    };
}
