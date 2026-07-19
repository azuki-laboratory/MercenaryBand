using Godot;

namespace MercenaryBand.UI;

public partial class HudDisplay : Control
{
    private Label _titleLabel = null!;
    private Label _infoLabel = null!;
    private VBoxContainer _logContainer = null!;

    public override void _Ready()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        _titleLabel = new Label
        {
            Text = "MercenaryBand",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        _titleLabel.AddThemeFontSizeOverride("font_size", 28);
        AddChild(_titleLabel);

        _infoLabel = new Label
        {
            Text = "Loading...",
            Position = new Vector2(0, 40),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        _infoLabel.AddThemeFontSizeOverride("font_size", 16);
        AddChild(_infoLabel);

        var scrollContainer = new ScrollContainer
        {
            Position = new Vector2(10, 70),
            Size = new Vector2(380, 180)
        };
        AddChild(scrollContainer);

        _logContainer = new VBoxContainer();
        scrollContainer.AddChild(_logContainer);
    }

    public void SetInfo(string text)
    {
        if (_infoLabel != null)
            _infoLabel.Text = text;
    }

    public void AddLog(string text)
    {
        if (_logContainer == null) return;
        var label = new Label { Text = text };
        label.AddThemeFontSizeOverride("font_size", 12);
        _logContainer.AddChild(label);
        if (_logContainer.GetChildCount() > 20)
            _logContainer.GetChild(0).QueueFree();
    }
}
