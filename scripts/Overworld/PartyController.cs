using Godot;

namespace MercenaryBand.Overworld;

public partial class PartyController : Node2D
{
    private Vector2 _target;
    private bool _isMoving;
    private float _progress;

    [Signal] public delegate void DestinationReachedEventHandler();

    public float Speed { get; set; } = 60f;
    public bool IsMoving => _isMoving;

    public override void _Ready()
    {
        var rect = new ColorRect
        {
            Size = new Vector2(14, 14),
            Color = Colors.Gold,
            Position = new Vector2(-7, -7)
        };
        AddChild(rect);

        var label = new Label
        {
            Text = "P",
            Position = new Vector2(-3, -4)
        };
        label.AddThemeFontSizeOverride("font_size", 10);
        AddChild(label);
    }

    public void MoveTo(Vector2 target)
    {
        _target = target;
        _isMoving = true;
        _progress = 0f;
    }

    public override void _Process(double delta)
    {
        if (!_isMoving) return;

        var start = GlobalPosition;
        float dist = start.DistanceTo(_target);

        if (dist < 4f)
        {
            GlobalPosition = _target;
            _isMoving = false;
            EmitSignal(SignalName.DestinationReached);
            return;
        }

        _progress += (float)delta * Speed / dist;
        GlobalPosition = start.Lerp(_target, Mathf.Min(_progress, 1f));
    }
}
