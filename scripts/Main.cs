using Godot;
using MercenaryBand.Data;
using MercenaryBand.Core;

namespace MercenaryBand.Scripts;

public partial class Main : Node2D
{
    public override void _Ready()
    {
        GD.Print("========================================");
        GD.Print("  MercenaryBand - Battle Brothers Clone");
        GD.Print("  Godot 4.7.1 / C#                      ");
        GD.Print("========================================");

        var dataManager = GetNode<DataLoader>("/root/DataManager");
        GD.Print($"  Backgrounds loaded: {System.Linq.Enumerable.Count(dataManager.AllBackgrounds)}");
        GD.Print($"  Perks loaded:       {System.Linq.Enumerable.Count(dataManager.AllPerks)}");
        GD.Print($"  Traits loaded:      {System.Linq.Enumerable.Count(dataManager.AllTraits)}");
        GD.Print($"  Injuries loaded:    {System.Linq.Enumerable.Count(dataManager.AllInjuries)}");
        GD.Print($"  Weapons loaded:     {System.Linq.Enumerable.Count(dataManager.AllWeapons)}");
        GD.Print($"  Armors loaded:      {System.Linq.Enumerable.Count(dataManager.AllArmors)}");
        GD.Print("========================================");
    }
}
