using Godot;

namespace MercenaryBand.Overworld;

public class TimeData
{
    public int Day { get; set; } = 1;
    public int Hour { get; set; } = 6;
    public float TimeScale { get; set; } = 1f;

    public string TimeString => $"Day {Day}, {Hour:D2}:00";
    public bool IsDaytime => Hour >= 6 && Hour < 20;

    public void Advance(int hours)
    {
        Hour += hours;
        while (Hour >= 24)
        {
            Hour -= 24;
            Day++;
        }
    }
}

public partial class TimeSystem : Node
{
    public TimeData Time { get; } = new();

    private float _accumulator;

    public override void _Process(double delta)
    {
        _accumulator += (float)delta * Time.TimeScale;
        if (_accumulator >= 1f)
        {
            _accumulator -= 1f;
            Time.Advance(1);
        }
    }
}
