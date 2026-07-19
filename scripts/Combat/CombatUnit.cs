using Godot;
using MercenaryBand.Data;

namespace MercenaryBand.Combat;

public enum Team
{
    Player,
    Enemy
}

public partial class CombatUnit : Node2D
{
    [Export] public string UnitName { get; set; } = "Unknown";
    [Export] public Team Team { get; set; } = Team.Player;

    public HexCoord HexPosition { get; set; }
    public int Initiative { get; set; } = 100;

    public int Hitpoints { get; set; } = 50;
    public int MaxHitpoints { get; set; } = 50;
    public int ActionPoints { get; set; } = 9;
    public int MaxActionPoints { get; set; } = 9;
    public int Fatigue { get; set; }
    public int MaxFatigue { get; set; } = 100;

    public int MeleeSkill { get; set; } = 50;
    public int RangedSkill { get; set; } = 40;
    public int MeleeDefense { get; set; } = 5;
    public int RangedDefense { get; set; } = 5;
    public int Resolve { get; set; } = 40;

    public int BodyArmor { get; set; }
    public int HeadArmor { get; set; }

    public WeaponDef? MainHandWeapon { get; set; }
    public CharacterData? CharacterData { get; set; }

    public bool IsDead => Hitpoints <= 0;
    public bool HasActed { get; set; }

    public CombatScene? CombatScene => GetParent() as CombatScene;

    public override void _Ready()
    {
        GD.Print($"[CombatUnit] {UnitName} ready (Team: {Team})");
    }

    public void ResetTurn()
    {
        ActionPoints = MaxActionPoints;
        HasActed = false;
    }

    public bool CanMove(int cost) => !IsDead && ActionPoints >= cost && !HasActed;

    public void MoveTo(HexCoord target, int cost)
    {
        HexPosition = target;
        ActionPoints -= cost;

        var pixel = target.ToPixel(32f);
        GlobalPosition = pixel;

        GD.Print($"[CombatUnit] {UnitName} moved to {target} (AP left: {ActionPoints})");
    }

    public void PerformAttack(CombatUnit target, CombatCalculator calc)
    {
        HasActed = true;
        var result = calc.CalculateMeleeHit(this, target);
        ActionPoints -= 4;
        Fatigue += 5;
    }
}
