using Raylib_cs;

namespace terrain_prototype_raylib.src.Tile;

public static class Tiles
{
    private static Texture2D staticTiles = Raylib.LoadTexture("assets/tileset.png");
    public static readonly Tile DeepWater = new(TileType.DeepWater, new ColorTileGraphic(new Color(20, 50, 150, 255)), false);
    public static readonly Tile Water = new(TileType.Water, new ColorTileGraphic(new Color(40, 100, 200, 255)), false);
    public static readonly Tile Sand = new(TileType.Sand, new StaticTileGraphic(staticTiles, new Rectangle(160f, 16f, 16f, 16f), new Color(230, 210, 130, 255)), false);
    public static readonly Tile Grass = new(TileType.Grass, new StaticTileGraphic(staticTiles, new Rectangle(16f, 16f, 16f, 16f), new Color(50, 160, 60, 255)), false);
    public static readonly Tile Forest = new(TileType.Forest, new ColorTileGraphic(new Color(20, 110, 30, 255)), false);
    public static readonly Tile Mountain = new(TileType.Mountain, new ColorTileGraphic(new Color(120, 120, 120, 255)), false);
    public static readonly Tile Snow = new(TileType.Snow, new ColorTileGraphic(Color.White), false);

}
