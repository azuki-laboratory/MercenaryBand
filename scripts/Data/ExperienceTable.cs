using System.Collections.Generic;
using System.Linq;
using Godot;
using MercenaryBand.Data;

namespace MercenaryBand.Systems;

public static class ExperienceTable
{
    public static readonly int[] XpToNextLevel =
    {
        0,      // Lv.1
        200,    // Lv.2
        500,    // Lv.3
        1000,   // Lv.4
        1800,   // Lv.5
        2800,   // Lv.6
        4000,   // Lv.7
        5500,   // Lv.8
        7500,   // Lv.9
        10000,  // Lv.10
        14000,  // Lv.11
    };

    public static int MaxLevel => XpToNextLevel.Length;

    public static int GetXpForLevel(int level)
    {
        if (level < 2) return 0;
        if (level > MaxLevel) return int.MaxValue;
        return XpToNextLevel[level - 1];
    }
}
