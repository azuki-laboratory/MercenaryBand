using Godot;
using static MercenaryBand.UI.Theme;

namespace MercenaryBand.Overworld;

public partial class PartyController : Node2D
{
    private Vector2 _target;
    private bool _isMoving;
    private float _animationTime;

    [Signal] public delegate void DestinationReachedEventHandler();

    public float Speed { get; set; } = 100f;
    public bool IsMoving => _isMoving;

    public override void _Ready()
    {
        var bannerPole = new ColorRect
        {
            Size = new Vector2(2, 16),
            Color = new Color(0.5f, 0.4f, 0.3f),
            Position = new Vector2(4, -16)
        };
        AddChild(bannerPole);

        var banner = new ColorRect
        {
            Size = new Vector2(14, 10),
            Color = new Color(Gold, 0.85f),
            Position = new Vector2(-5, -26)
        };
        AddChild(banner);

        var body = new ColorRect
        {
            Size = new Vector2(10, 10),
            Color = Gold,
            Position = new Vector2(-5, -5)
        };
        AddChild(body);

        var label = new Label
        {
            Text = "MB",
            Position = new Vector2(-3, -3)
        };
        label.AddThemeColorOverride("font_color", Colors.Black);
        label.AddThemeFontSizeOverride("font_size", 8);
        AddChild(label);
    }

    public void MoveTo(Vector2 target)
    {
        _target = target;
        _isMoving = true;
        _animationTime = 0f;
    }

    public void UpdatePosition(float delta)
    {
        if (!_isMoving) return;

        var start = GlobalPosition;
        float dist = start.DistanceTo(_target);

        if (dist < 3f)
        {
            GlobalPosition = _target;
            _isMoving = false;
            EmitSignal(SignalName.DestinationReached);
            return;
        }

        _animationTime += delta * Speed / dist;
        GlobalPosition = start.Lerp(_target, Mathf.Min(_animationTime, 0.98f));
    }
}
