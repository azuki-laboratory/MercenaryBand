namespace MercenaryBand.Combat;

public enum TileType
{
    Plains,
    Forest,
    Hill,
    Swamp,
    Water,
    Mountain
}

public class HexTile
{
    public HexCoord Coord { get; }
    public TileType Type { get; set; }
    public int MoveCost { get; set; }
    public float Height { get; set; }

    public HexTile(HexCoord coord, TileType type = TileType.Plains)
    {
        Coord = coord;
        Type = type;
        MoveCost = GetBaseMoveCost(type);
    }

    private static int GetBaseMoveCost(TileType type) => type switch
    {
        TileType.Plains => 1,
        TileType.Forest => 2,
        TileType.Hill => 3,
        TileType.Swamp => 3,
        TileType.Water => 99,
        TileType.Mountain => 99,
        _ => 1
    };
}
