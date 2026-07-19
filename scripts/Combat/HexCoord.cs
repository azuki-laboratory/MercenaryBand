using System;
using System.Collections.Generic;
using Godot;

namespace MercenaryBand.Combat;

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public readonly struct HexCoord : IEquatable<HexCoord>
{
    public int Q { get; }
    public int R { get; }

    public int S => -Q - R;

    public HexCoord(int q, int r)
    {
        Q = q;
        R = r;
    }

    public static HexCoord Zero => new(0, 0);

    private static readonly HexCoord[] Directions = {
        new(1, -1),  // NE
        new(1, 0),   // E
        new(0, 1),   // SE
        new(-1, 1),  // SW
        new(-1, 0),  // W
        new(0, -1)   // NW
    };

    public HexCoord Neighbor(HexDirection dir) => this + Directions[(int)dir];

    public int DistanceTo(HexCoord other)
    {
        var dq = Math.Abs(Q - other.Q);
        var dr = Math.Abs(R - other.R);
        var ds = Math.Abs(S - other.S);
        return Math.Max(dq, Math.Max(dr, ds));
    }

    public IEnumerable<HexCoord> Neighbors()
    {
        for (int i = 0; i < 6; i++)
            yield return Neighbor((HexDirection)i);
    }

    public IEnumerable<HexCoord> Range(int distance)
    {
        for (int dq = -distance; dq <= distance; dq++)
        {
            for (int dr = Math.Max(-distance, -dq - distance);
                 dr <= Math.Min(distance, -dq + distance);
                 dr++)
            {
                yield return new HexCoord(Q + dq, R + dr);
            }
        }
    }

    public IEnumerable<HexCoord> LineTo(HexCoord target)
    {
        int dist = DistanceTo(target);
        if (dist == 0)
        {
            yield return this;
            yield break;
        }

        for (int i = 0; i <= dist; i++)
        {
            float t = dist == 0 ? 0f : (float)i / dist;
            float q = Q + (target.Q - Q) * t;
            float r = R + (target.R - R) * t;
            yield return Round(q, r);
        }
    }

    public static HexCoord Round(float q, float r)
    {
        float s = -q - r;
        int rq = (int)Math.Round(q);
        int rr = (int)Math.Round(r);
        int rs = (int)Math.Round(s);

        float dq = Math.Abs(rq - q);
        float dr = Math.Abs(rr - r);
        float ds = Math.Abs(rs - s);

        if (dq > dr && dq > ds)
            rq = -rr - rs;
        else if (dr > ds)
            rr = -rq - rs;

        return new HexCoord(rq, rr);
    }

    public Vector2 ToPixel(float size)
    {
        float x = size * (3f / 2f * Q);
        float y = size * (Mathf.Sqrt(3f) / 2f * Q + Mathf.Sqrt(3f) * R);
        return new Vector2(x, y);
    }

    public static HexCoord FromPixel(Vector2 pixel, float size)
    {
        float q = 2f / 3f * pixel.X / size;
        float r = (-1f / 3f * pixel.X + Mathf.Sqrt(3f) / 3f * pixel.Y) / size;
        return Round(q, r);
    }

    public static HexCoord operator +(HexCoord a, HexCoord b) => new(a.Q + b.Q, a.R + b.R);
    public static HexCoord operator -(HexCoord a, HexCoord b) => new(a.Q - b.Q, a.R - b.R);
    public static bool operator ==(HexCoord a, HexCoord b) => a.Q == b.Q && a.R == b.R;
    public static bool operator !=(HexCoord a, HexCoord b) => !(a == b);

    public bool Equals(HexCoord other) => Q == other.Q && R == other.R;
    public override bool Equals(object? obj) => obj is HexCoord other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Q, R);
    public override string ToString() => $"({Q}, {R})";
}
