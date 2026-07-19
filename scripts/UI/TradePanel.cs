using Godot;
using MercenaryBand.Core;

namespace MercenaryBand.UI;

public partial class TradePanel : VBoxContainer
{
    [Signal] public delegate void ItemPurchasedEventHandler(string itemName, int cost);

    public void ShowItems(DataLoader data)
    {
        var title = new Label { Text = "--- 장비 상점 ---" };
        title.AddThemeFontSizeOverride("font_size", 16);
        AddChild(title);

        foreach (var weapon in data.AllWeapons)
        {
            var row = new HBoxContainer();
            row.AddChild(new Label
            {
                Text = $"{weapon.Name} ({weapon.DamageMin}-{weapon.DamageMax} dmg)",
                CustomMinimumSize = new Vector2(180, 30)
            });

            var btn = new Button
            {
                Text = $"구매 ({weapon.Value}g)",
                CustomMinimumSize = new Vector2(100, 30)
            };
            var wName = weapon.Name;
            var wCost = weapon.Value;
            btn.Pressed += () =>
            {
                EmitSignal(SignalName.ItemPurchased, wName, wCost);
                btn.Disabled = true;
                btn.Text = "구매됨";
            };
            row.AddChild(btn);
            AddChild(row);
        }

        var divider = new Label { Text = "--- 방어구 ---" };
        divider.AddThemeFontSizeOverride("font_size", 13);
        AddChild(divider);

        foreach (var armor in data.AllArmors)
        {
            var row = new HBoxContainer();
            row.AddChild(new Label
            {
                Text = $"{armor.Name} ({armor.Armor} def)",
                CustomMinimumSize = new Vector2(180, 30)
            });

            var btn = new Button
            {
                Text = $"구매 ({armor.Value}g)",
                CustomMinimumSize = new Vector2(100, 30)
            };
            var aName = armor.Name;
            var aCost = armor.Value;
            btn.Pressed += () =>
            {
                EmitSignal(SignalName.ItemPurchased, aName, aCost);
                btn.Disabled = true;
                btn.Text = "구매됨";
            };
            row.AddChild(btn);
            AddChild(row);
        }
    }
}
