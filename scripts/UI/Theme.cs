using Godot;

namespace MercenaryBand.UI;

public static class Theme
{
    public static readonly Color BgDark      = new(0.08f, 0.08f, 0.12f, 1f);
    public static readonly Color BgPanel     = new(0.10f, 0.10f, 0.16f, 0.92f);
    public static readonly Color BgButton    = new(0.18f, 0.14f, 0.10f, 1f);
    public static readonly Color BgHover     = new(0.25f, 0.18f, 0.12f, 1f);

    public static readonly Color Gold        = new(0.79f, 0.66f, 0.29f, 1f);
    public static readonly Color GoldDim     = new(0.55f, 0.45f, 0.20f, 1f);
    public static readonly Color TextPrimary = new(0.91f, 0.84f, 0.72f, 1f);
    public static readonly Color TextDim     = new(0.60f, 0.55f, 0.48f, 1f);

    public static readonly Color AccentRed   = new(0.65f, 0.15f, 0.15f, 1f);
    public static readonly Color AccentGreen = new(0.20f, 0.55f, 0.20f, 1f);
    public static readonly Color AccentBlue  = new(0.15f, 0.30f, 0.55f, 1f);

    public static readonly Color MapLand     = new(0.18f, 0.22f, 0.14f, 1f);
    public static readonly Color MapWater    = new(0.08f, 0.12f, 0.22f, 1f);
    public static readonly Color MapRoad     = new(0.35f, 0.30f, 0.20f, 0.4f);

    public static readonly Color HealthBar   = new(0.75f, 0.15f, 0.15f, 1f);
    public static readonly Color ArmorBar    = new(0.55f, 0.55f, 0.60f, 1f);
    public static readonly Color FatigueBar  = new(0.25f, 0.55f, 0.70f, 1f);
    public static readonly Color XpBar       = new(0.22f, 0.65f, 0.22f, 1f);

    public static readonly Color HexPlains   = new(0.28f, 0.38f, 0.18f, 1f);
    public static readonly Color HexForest   = new(0.15f, 0.25f, 0.10f, 1f);
    public static readonly Color HexHill     = new(0.40f, 0.35f, 0.22f, 1f);
    public static readonly Color HexSwamp    = new(0.20f, 0.28f, 0.15f, 1f);
    public static readonly Color HexWater    = new(0.12f, 0.20f, 0.42f, 1f);
    public static readonly Color HexMountain = new(0.38f, 0.32f, 0.28f, 1f);

    public static StyleBoxFlat MakePanelStyle(float radius = 6f)
    {
        return new StyleBoxFlat
        {
            BgColor = BgPanel,
            CornerRadiusTopLeft = (int)radius,
            CornerRadiusTopRight = (int)radius,
            CornerRadiusBottomLeft = (int)radius,
            CornerRadiusBottomRight = (int)radius,
            BorderWidthLeft = 1,
            BorderWidthRight = 1,
            BorderWidthTop = 1,
            BorderWidthBottom = 1,
            BorderColor = new Color(GoldDim, 0.4f),
            ContentMarginLeft = 12,
            ContentMarginRight = 12,
            ContentMarginTop = 8,
            ContentMarginBottom = 8
        };
    }

    public static StyleBoxFlat MakeButtonStyle()
    {
        return new StyleBoxFlat
        {
            BgColor = BgButton,
            CornerRadiusTopLeft = 4,
            CornerRadiusTopRight = 4,
            CornerRadiusBottomLeft = 4,
            CornerRadiusBottomRight = 4,
            BorderWidthLeft = 1,
            BorderWidthRight = 1,
            BorderWidthTop = 1,
            BorderWidthBottom = 1,
            BorderColor = new Color(Gold, 0.5f),
            ContentMarginLeft = 16,
            ContentMarginRight = 16,
            ContentMarginTop = 6,
            ContentMarginBottom = 6
        };
    }

    public static Button MakeButton(string text)
    {
        var btn = new Button
        {
            Text = text,
            CustomMinimumSize = new Vector2(220, 42)
        };
        btn.AddThemeColorOverride("font_color", TextPrimary);
        btn.AddThemeFontSizeOverride("font_size", 16);
        btn.AddThemeStyleboxOverride("normal", MakeButtonStyle());
        btn.AddThemeStyleboxOverride("hover", new StyleBoxFlat
        {
            BgColor = BgHover,
            CornerRadiusTopLeft = 4,
            CornerRadiusTopRight = 4,
            CornerRadiusBottomLeft = 4,
            CornerRadiusBottomRight = 4,
            BorderWidthLeft = 1,
            BorderWidthRight = 1,
            BorderWidthTop = 1,
            BorderWidthBottom = 1,
            BorderColor = new Color(Gold, 0.8f),
            ContentMarginLeft = 16,
            ContentMarginRight = 16,
            ContentMarginTop = 6,
            ContentMarginBottom = 6
        });
        return btn;
    }

    public static Label MakeTitle(string text, int fontSize = 36)
    {
        var label = new Label
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        label.AddThemeColorOverride("font_color", Gold);
        label.AddThemeFontSizeOverride("font_size", fontSize);
        return label;
    }

    public static Label MakeLabel(string text, int fontSize = 14)
    {
        var label = new Label
        {
            Text = text
        };
        label.AddThemeColorOverride("font_color", TextPrimary);
        label.AddThemeFontSizeOverride("font_size", fontSize);
        return label;
    }

    public static Panel MakePanel(Vector2 size)
    {
        var panel = new Panel { Size = size, CustomMinimumSize = size };
        panel.AddThemeStyleboxOverride("panel", MakePanelStyle());
        return panel;
    }

    public static ColorRect MakeBackground()
    {
        return new ColorRect
        {
            Size = new Vector2(1024, 600),
            Color = BgDark,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
    }
}
