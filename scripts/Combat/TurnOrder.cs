using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MercenaryBand.Combat;

public class TurnOrder
{
    private readonly List<TurnEntry> _entries = new();
    private int _currentIndex;
    private int _turnNumber;

    public int TurnNumber => _turnNumber;

    public class TurnEntry
    {
        public CombatUnit Unit { get; }
        public int Initiative { get; }

        public TurnEntry(CombatUnit unit, int initiative)
        {
            Unit = unit;
            Initiative = initiative;
        }
    }

    public void Initialize(IEnumerable<CombatUnit> units)
    {
        _entries.Clear();
        _currentIndex = 0;
        _turnNumber = 1;

        foreach (var unit in units.OrderByDescending(u => u.Initiative))
        {
            _entries.Add(new TurnEntry(unit, unit.Initiative));
        }

        GD.Print($"[TurnOrder] Initialized with {_entries.Count} units. Turn {_turnNumber}");
    }

    public CombatUnit? CurrentUnit
    {
        get
        {
            if (_entries.Count == 0)
                return null;
            return _entries[_currentIndex].Unit;
        }
    }

    public bool AdvanceTurn()
    {
        if (_entries.Count == 0)
            return false;

        _currentIndex++;
        if (_currentIndex >= _entries.Count)
        {
            _currentIndex = 0;
            _turnNumber++;
            GD.Print($"[TurnOrder] Round {_turnNumber}");
        }

        return CurrentUnit != null && !CurrentUnit.IsDead;
    }

    public bool SkipToNextAlive()
    {
        int startIndex = _currentIndex;
        do
        {
            if (CurrentUnit != null && !CurrentUnit.IsDead)
                return true;

            _currentIndex++;
            if (_currentIndex >= _entries.Count)
            {
                _currentIndex = 0;
                _turnNumber++;
            }
        } while (_currentIndex != startIndex);

        return false;
    }

    public int AliveCount => _entries.Count(e => !e.Unit.IsDead);

    public bool IsCombatOver()
    {
        var aliveTeams = _entries
            .Where(e => !e.Unit.IsDead)
            .Select(e => e.Unit.Team)
            .Distinct()
            .Count();

        return aliveTeams <= 1;
    }
}
