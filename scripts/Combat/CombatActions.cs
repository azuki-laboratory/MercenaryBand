using System.Collections.Generic;

namespace MercenaryBand.Combat;

public enum ActionType
{
    Move,
    AttackMelee,
    AttackRanged,
    UseSkill,
    Wait,
    Defend,
    Pass
}

public class CombatAction
{
    public ActionType Type { get; set; }
    public HexCoord? TargetCoord { get; set; }
    public CombatUnit? TargetUnit { get; set; }
    public int CostAP { get; set; }
    public int CostFatigue { get; set; }

    public static CombatAction Move(HexCoord target, int apCost) => new()
    {
        Type = ActionType.Move,
        TargetCoord = target,
        CostAP = apCost
    };

    public static CombatAction Attack(CombatUnit target, int apCost, int fatigueCost) => new()
    {
        Type = ActionType.AttackMelee,
        TargetUnit = target,
        CostAP = apCost,
        CostFatigue = fatigueCost
    };

    public static CombatAction Pass() => new()
    {
        Type = ActionType.Pass,
        CostAP = 0
    };
}

public class ActionResolver
{
    private readonly Pathfinding _pathfinding;
    private readonly CombatCalculator _calculator;

    public ActionResolver(Pathfinding pathfinding, CombatCalculator calculator)
    {
        _pathfinding = pathfinding;
        _calculator = calculator;
    }

    public List<CombatAction> GetAvailableActions(CombatUnit unit, HexGrid grid)
    {
        var actions = new List<CombatAction>();

        var reachable = _pathfinding.GetReachableTiles(unit.HexPosition, unit.ActionPoints);
        foreach (var tile in reachable)
        {
            if (tile == unit.HexPosition)
                continue;
            actions.Add(CombatAction.Move(tile, grid.GetMoveCost(tile)));
        }

        foreach (var neighbor in unit.HexPosition.Neighbors())
        {
            var other = unit.CombatScene?.GetUnitAt(neighbor);
            if (other != null && other.Team != unit.Team && !other.IsDead)
            {
                int apCost = 4;
                actions.Add(CombatAction.Attack(other, apCost, 5));
            }
        }

        actions.Add(CombatAction.Pass());
        return actions;
    }
}
