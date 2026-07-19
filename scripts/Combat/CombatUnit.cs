using Godot;

namespace MercenaryBand.Combat;

public partial class CombatUnit : Node2D
{
    public string UnitId { get; set; } = string.Empty;
    public int Initiative { get; set; }

    public override void _Ready()
    {
        GD.Print($"[CombatUnit] Unit ready: {UnitId}");
    }
}
