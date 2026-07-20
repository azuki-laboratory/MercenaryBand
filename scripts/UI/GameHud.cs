using Godot;
using static MercenaryBand.UI.Theme;

namespace MercenaryBand.UI;

public partial class GameHud : Control
{
    private Label _goldLabel = null!;
    private Label _foodLabel = null!;
    private Label _timeLabel = null!;
    private Label _partyLabel = null!;
    private Label _infoLabel = null!;

    public int Gold { get; set; } = 500;
    public int Food { get; set; } = 30;
    public int PartySize { get; set; } = 3;
    public string TimeDisplay { get; set; } = "Day 1, 06:00";
    public string InfoText { get; set; } = "";

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;

        var bar = new ColorRect
        {
            Size = new Vector2(1024, 28),
            Color = new Color(0.05f, 0.05f, 0.08f, 0.85f)
        };
        AddChild(bar);

        var barBottom = new ColorRect
        {
            Size = new Vector2(1024, 1),
            Position = new Vector2(0, 28),
            Color = new Color(MercenaryBand.UI.Theme.Gold, 0.3f)
        };
        AddChild(barBottom);

        _goldLabel = new Label
        {
            Position = new Vector2(12, 4)
        };
        _goldLabel.AddThemeColorOverride("font_color", MercenaryBand.UI.Theme.Gold);
        _goldLabel.AddThemeFontSizeOverride("font_size", 13);
        AddChild(_goldLabel);

        _foodLabel = new Label
        {
            Position = new Vector2(152, 4)
        };
        _foodLabel.AddThemeColorOverride("font_color", TextPrimary);
        _foodLabel.AddThemeFontSizeOverride("font_size", 13);
        AddChild(_foodLabel);

        _timeLabel = new Label
        {
            Position = new Vector2(292, 4)
        };
        _timeLabel.AddThemeColorOverride("font_color", TextPrimary);
        _timeLabel.AddThemeFontSizeOverride("font_size", 13);
        AddChild(_timeLabel);

        _partyLabel = new Label
        {
            Position = new Vector2(472, 4)
        };
        _partyLabel.AddThemeColorOverride("font_color", TextPrimary);
        _partyLabel.AddThemeFontSizeOverride("font_size", 13);
        AddChild(_partyLabel);

        _infoLabel = new Label
        {
            Position = new Vector2(824, 4),
            Size = new Vector2(190, 20),
            HorizontalAlignment = HorizontalAlignment.Right,
            ClipText = true
        };
        _infoLabel.AddThemeColorOverride("font_color", TextDim);
        _infoLabel.AddThemeFontSizeOverride("font_size", 11);
        AddChild(_infoLabel);

        Refresh();
    }

    public void Refresh()
    {
        _goldLabel.Text = $"  Gold: {Gold}";
        _foodLabel.Text = $"  Food: {Food}";
        _timeLabel.Text = $"  {TimeDisplay}";
        _partyLabel.Text = $"  Party: {PartySize}";
        _infoLabel.Text = InfoText;
    }

    public void SetInfo(string text) { InfoText = text; Refresh(); }
    public void SetGold(int g) { Gold = g; Refresh(); }
    public void SetFood(int f) { Food = f; Refresh(); }
    public void SetParty(int p) { PartySize = p; Refresh(); }
    public void SetTime(string t) { TimeDisplay = t; Refresh(); }
}
