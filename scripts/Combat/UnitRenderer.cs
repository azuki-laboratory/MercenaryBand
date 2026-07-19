using Godot;

namespace MercenaryBand.Combat;

public partial class UnitRenderer : Node2D
{
    private Sprite2D? _sprite;
    private Label? _nameLabel;
    private Label? _hpLabel;
    private float _hexSize = 30f;

    public override void _Ready()
    {
        SetupVisuals();
    }

    private void SetupVisuals()
    {
        _sprite = new Sprite2D();
        AddChild(_sprite);

        var rect = new ColorRect
        {
            Size = new Vector2(20, 20),
            Color = Colors.White
        };
        AddChild(rect);

        _nameLabel = new Label { Position = new Vector2(-30, -30) };
        _nameLabel.AddThemeFontSizeOverride("font_size", 10);
        AddChild(_nameLabel);

        _hpLabel = new Label { Position = new Vector2(-30, -18) };
        _hpLabel.AddThemeFontSizeOverride("font_size", 9);
        AddChild(_hpLabel);
    }

    public void UpdateFromUnit(CombatUnit unit, float hexSize)
    {
        _hexSize = hexSize;
        var pixel = unit.HexPosition.ToPixel(hexSize);
        GlobalPosition = pixel;

        if (_nameLabel != null)
            _nameLabel.Text = unit.UnitName;

        var teamColor = unit.Team == Team.Player ? Colors.DodgerBlue : Colors.Crimson;
        var rect = GetNodeOrNull<ColorRect>("ColorRect");
        if (rect != null) rect.Color = unit.IsDead ? Colors.Gray : teamColor;

        if (_hpLabel != null)
            _hpLabel.Text = $"{unit.Hitpoints}/{unit.MaxHitpoints}";
    }
}
