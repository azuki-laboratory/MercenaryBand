using System;
using System.Collections.Generic;
using Godot;

namespace MercenaryBand.Combat;

public partial class HexRenderer : Node2D
{
    private readonly List<(Vector2[] poly, Color color)> _drawQueue = new();
    private float _hexSize = 30f;
    private Vector2 _offset = Vector2.Zero;

    private static readonly Color[] TileColors = {
        new(0.4f, 0.7f, 0.3f),  // Plains
        new(0.2f, 0.5f, 0.2f),  // Forest
        new(0.6f, 0.5f, 0.3f),  // Hill
        new(0.3f, 0.4f, 0.2f),  // Swamp
        new(0.2f, 0.3f, 0.7f),  // Water
        new(0.5f, 0.4f, 0.3f)   // Mountain
    };

    public override void _Ready()
    {
        _offset = GetViewportRect().Size / 2f;
    }

    public void SetOffset(Vector2 offset) { _offset = offset; }

    public void Clear()
    {
        _drawQueue.Clear();
        QueueRedraw();
    }

    public void QueueHex(HexCoord coord, TileType type = TileType.Plains)
    {
        var points = HexPoints(coord, _hexSize);
        var color = TileColors[(int)type];
        _drawQueue.Add((points, color));
    }

    public void QueueAll(IEnumerable<HexTile> tiles)
    {
        foreach (var tile in tiles)
        {
            QueueHex(tile.Coord, tile.Type);
        }
        QueueRedraw();
    }

    public void Flush()
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        foreach (var (points, color) in _drawQueue)
        {
            DrawPolygon(points, [color with { A = 0.3f }]);
            DrawPolyline(points, color with { A = 0.6f });
        }
    }

    public static Vector2[] HexPoints(HexCoord coord, float size)
    {
        var center = coord.ToPixel(size);
        var points = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.DegToRad(60f * i);
            points[i] = center + new Vector2(
                size * Mathf.Cos(angle),
                size * Mathf.Sin(angle)
            );
        }
        return points;
    }

    public void SetHexSize(float size) { _hexSize = size; }
}
