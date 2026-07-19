using System.Collections.Generic;
using Godot;
using MercenaryBand.Data;
using MercenaryBand.Combat;

namespace MercenaryBand.Overworld;

public partial class SettlementNode : Node2D
{
    public SettlementDef Def { get; }

    private static readonly Dictionary<string, Color> TypeColors = new()
    {
        ["City"] = Colors.Goldenrod,
        ["Town"] = Colors.SandyBrown,
        ["Fortress"] = Colors.LightCoral,
        ["Village"] = Colors.DarkKhaki
    };

    private static readonly Vector2 MapScale = new(25f, 25f);

    public SettlementNode(SettlementDef def)
    {
        Def = def;
        var pixel = new HexCoord(def.Coord.Q, def.Coord.R).ToPixel(20f);
        GlobalPosition = pixel + new Vector2(400, 300);
    }

    public override void _Ready()
    {
        int size = Def.Type switch
        {
            "City" => 24,
            "Fortress" => 22,
            "Town" => 18,
            _ => 14
        };

        var bg = new ColorRect
        {
            Size = new Vector2(size, size),
            Color = TypeColors.GetValueOrDefault(Def.Type, Colors.Gray),
            Position = new Vector2(-size / 2f, -size / 2f)
        };
        AddChild(bg);

        var border = new ColorRect
        {
            Size = new Vector2(size + 2, size + 2),
            Color = Colors.Black,
            Position = new Vector2(-size / 2f - 1, -size / 2f - 1)
        };
        AddChild(border);
        MoveChild(border, 0);

        var label = new Label
        {
            Text = Def.Name,
            Position = new Vector2(-30, size / 2f + 2)
        };
        label.AddThemeFontSizeOverride("font_size", 10);
        AddChild(label);
    }
}
