using System;
using System.Collections.Generic;
using Godot;
using MercenaryBand.Core;
using MercenaryBand.Systems;

namespace MercenaryBand.UI;

public partial class RecruitPanel : VBoxContainer
{
    [Signal] public delegate void RecruitCompletedEventHandler(string name);

    public void ShowRecruits(DataLoader data, int count)
    {
        var factory = new CharacterFactory(data);

        var title = new Label { Text = "--- 용병 모집 ---" };
        title.AddThemeFontSizeOverride("font_size", 16);
        AddChild(title);

        for (int i = 0; i < count; i++)
        {
            var bgId = i switch
            {
                0 => "farmhand",
                1 => "apprentice",
                _ => "beggar"
            };
            var merc = factory.Create(bgId, 1);

            var row = new HBoxContainer();
            var label = new Label
            {
                Text = $"{merc.Name}  Lv.{merc.Level}",
                CustomMinimumSize = new Vector2(120, 30)
            };
            row.AddChild(label);

            var statsLabel = new Label { Text = $"HP:{merc.Stats["Hitpoints"]:F0} MS:{merc.Stats["MeleeSkill"]:F0}" };
            statsLabel.AddThemeFontSizeOverride("font_size", 10);
            row.AddChild(statsLabel);

            var btn = new Button
            {
                Text = $"고용 ({CharacterRng.Range(50, 200)}g)",
                CustomMinimumSize = new Vector2(100, 30)
            };
            var mercName = merc.Name;
            btn.Pressed += () =>
            {
                GD.Print($"[Recruit] Hired {mercName}");
                EmitSignal(SignalName.RecruitCompleted, mercName);
                btn.Disabled = true;
                btn.Text = "고용됨";
            };
            row.AddChild(btn);

            AddChild(row);
        }
    }
}
