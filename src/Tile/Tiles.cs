using Raylib_cs;

namespace terrain_prototype_raylib.src;

public static class Tiles
{
    public static readonly Tile DeepWater = new(TileType.DeepWater, null, new Color(20, 50, 150, 255));
    public static readonly Tile Water = new(TileType.Water, null, new Color(40, 100, 200, 255));
    public static readonly Tile Sand = new(TileType.Sand, new Rectangle(160f, 16f, 16f, 16f), new Color(230, 210, 130, 255));
    public static readonly Tile Grass = new(TileType.Grass, new Rectangle(16f, 16f, 16f, 16f), new Color(50, 160, 60, 255));
    public static readonly Tile Forest = new(TileType.Forest, null, new Color(20, 110, 30, 255));
    public static readonly Tile Mountain = new(TileType.Mountain, null, new Color(120, 120, 120, 255));
    public static readonly Tile Snow = new(TileType.Snow, new Rectangle(208f, 16f, 16f, 16f), Color.White);
}
