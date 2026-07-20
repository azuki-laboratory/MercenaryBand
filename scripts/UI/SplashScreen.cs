using Godot;
using static MercenaryBand.UI.Theme;

namespace MercenaryBand.UI;

public partial class SplashScreen : Control
{
    [Signal] public delegate void LoadingCompleteEventHandler();

    private float _elapsed;
    private float _fadeIn;
    private bool _completed;

    public override void _Ready()
    {
        var bg = MakeBackground();
        AddChild(bg);

        var glowRect = new ColorRect
        {
            Size = new Vector2(800, 200),
            Position = new Vector2(112, 180),
            Color = new Color(Gold, 0.08f),
            MouseFilter = MouseFilterEnum.Ignore
        };
        AddChild(glowRect);

        var title = new Label
        {
            Text = "MERCENARY",
            Position = new Vector2(200, 140),
            Size = new Vector2(624, 50),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        title.AddThemeColorOverride("font_color", Gold);
        title.AddThemeFontSizeOverride("font_size", 48);
        AddChild(title);

        var subtitle = new Label
        {
            Text = "BAND",
            Position = new Vector2(200, 200),
            Size = new Vector2(624, 60),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        subtitle.AddThemeColorOverride("font_color", GoldDim);
        subtitle.AddThemeFontSizeOverride("font_size", 56);
        AddChild(subtitle);

        var tagline = new Label
        {
            Text = "A Mercenary Company Simulator",
            Position = new Vector2(200, 280),
            Size = new Vector2(624, 30),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        tagline.AddThemeColorOverride("font_color", TextDim);
        tagline.AddThemeFontSizeOverride("font_size", 14);
        AddChild(tagline);

        var loaderBg = new ColorRect
        {
            Size = new Vector2(400, 8),
            Position = new Vector2(312, 360),
            Color = new Color(GoldDim, 0.2f)
        };
        AddChild(loaderBg);

        var loaderFill = new ColorRect
        {
            Name = "LoaderFill",
            Size = new Vector2(0, 8),
            Position = new Vector2(312, 360),
            Color = Gold
        };
        AddChild(loaderFill);

        var loadingLabel = new Label
        {
            Text = "Loading...",
            Position = new Vector2(312, 378),
            Size = new Vector2(400, 20),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        loadingLabel.AddThemeColorOverride("font_color", TextDim);
        loadingLabel.AddThemeFontSizeOverride("font_size", 11);
        AddChild(loadingLabel);
    }

    public override void _Process(double delta)
    {
        _elapsed += (float)delta;
        _fadeIn = Mathf.Min(_fadeIn + (float)delta * 0.5f, 1f);

        var fill = GetNode<ColorRect>("LoaderFill");
        float progress = Mathf.Min(_elapsed / 2.5f, 1f);
        fill.Size = new Vector2(400 * progress, 8);

        if (_elapsed > 2.5f && !_completed)
        {
            _completed = true;
            EmitSignal(SignalName.LoadingComplete);
        }
    }
}
