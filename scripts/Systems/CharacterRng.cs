using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MercenaryBand.Systems;

public static class CharacterRng
{
    private static readonly Random _rng = new();

    public static int Range(int min, int max) => _rng.Next(min, max + 1);
    public static bool Chance(float probability) => _rng.NextDouble() < probability;

    public static T Pick<T>(IReadOnlyList<T> list) =>
        list[_rng.Next(list.Count)];

    public static List<T> PickWeighted<T>(IReadOnlyList<T> items, Func<T, int> weightSelector, int count)
    {
        var result = new List<T>();
        var available = items.ToList();

        for (int i = 0; i < count && available.Count > 0; i++)
        {
            var selected = WeightedPick(available, weightSelector);
            result.Add(selected);
            available.Remove(selected);
        }

        return result;
    }

    private static T WeightedPick<T>(IReadOnlyList<T> items, Func<T, int> weightSelector)
    {
        int totalWeight = items.Sum(weightSelector);
        int roll = _rng.Next(totalWeight);
        int cumulative = 0;

        foreach (var item in items)
        {
            cumulative += weightSelector(item);
            if (roll < cumulative)
                return item;
        }

        return items[^1];
    }
}
