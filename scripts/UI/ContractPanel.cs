using System;
using Godot;
using MercenaryBand.Data;
using MercenaryBand.Core;

namespace MercenaryBand.UI;

public partial class ContractPanel : VBoxContainer
{
    [Signal] public delegate void ContractAcceptedEventHandler(string contractName, int reward);

    public void ShowContracts(DataLoader data, string settlementId)
    {
        var title = new Label { Text = "--- 계약 의뢰 ---" };
        title.AddThemeFontSizeOverride("font_size", 16);
        AddChild(title);

        foreach (var contract in data.AllContracts)
        {
            var row = new HBoxContainer();
            row.AddChild(new Label
            {
                Text = $"{contract.Name} [{contract.Difficulty}★]",
                CustomMinimumSize = new Vector2(160, 30)
            });

            var desc = new Label
            {
                Text = contract.Description,
                CustomMinimumSize = new Vector2(100, 30)
            };
            desc.AddThemeFontSizeOverride("font_size", 10);
            row.AddChild(desc);

            var btn = new Button
            {
                Text = $"수락 ({contract.BaseReward}g)",
                CustomMinimumSize = new Vector2(100, 30)
            };
            var cName = contract.Name;
            var cReward = contract.BaseReward;
            var cId = contract.Id;
            btn.Pressed += () =>
            {
                GD.Print($"[ContractPanel] Accepted: {cId}");
                EmitSignal(SignalName.ContractAccepted, cName, cReward);
                btn.Disabled = true;
                btn.Text = "수락됨";
            };
            row.AddChild(btn);
            AddChild(row);
        }
    }
}
