using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MercenaryBand.Combat;

public partial class HexGrid : Node2D
{
    private readonly Dictionary<HexCoord, HexTile> _tiles = new();
    private float _hexSize = 32f;
    private int _radius = 8;

    [Export]
    public float HexSize
    {
        get => _hexSize;
        set { _hexSize = value; }
    }

    [Export]
    public int Radius
    {
        get => _radius;
        set { _radius = value; }
    }

    public override void _Ready()
    {
        GD.Print($"[HexGrid] Initialized (radius={_radius}, hexSize={_hexSize})");
    }

    public void GenerateCircularMap(int radius)
    {
        _radius = radius;
        _tiles.Clear();

        for (int q = -radius; q <= radius; q++)
        {
            for (int r = Math.Max(-radius, -q - radius);
                 r <= Math.Min(radius, -q + radius);
                 r++)
            {
                var coord = new HexCoord(q, r);
                _tiles[coord] = new HexTile(coord, TileType.Plains);
            }
        }

        GD.Print($"[HexGrid] Generated circular map with {_tiles.Count} tiles");
    }

    public HexTile? GetTile(HexCoord coord) =>
        _tiles.TryGetValue(coord, out var tile) ? tile : null;

    public bool IsPassable(HexCoord coord)
    {
        var tile = GetTile(coord);
        return tile != null && tile.MoveCost < 99;
    }

    public int GetMoveCost(HexCoord coord)
    {
        var tile = GetTile(coord);
        return tile?.MoveCost ?? 99;
    }

    public IEnumerable<HexTile> AllTiles => _tiles.Values;
}
