using Godot;
using Godot.Collections;

namespace MercenaryBand.Combat;

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public partial class HexGrid : Node2D
{
    public override void _Ready()
    {
        GD.Print("[HexGrid] Initialized");
    }
}
