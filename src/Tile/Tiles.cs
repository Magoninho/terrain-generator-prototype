using Raylib_cs;

namespace terrain_prototype_raylib.src.Tile;

public static class Tiles
{
    private static Texture2D StaticTiles = Raylib.LoadTexture("assets/tileset.png");
    private static Texture2D GrassAutotile = Raylib.LoadTexture("assets/grass.png");
    private static Texture2D SandAutotile = Raylib.LoadTexture("assets/sand.png");
    private static Texture2D WaterTexture = Raylib.LoadTexture("assets/water.png");
    private static Texture2D DeepWaterTexture = Raylib.LoadTexture("assets/deepwater.png");
    public static readonly Tile DeepWater = new(TileType.DeepWater, new StaticTileGraphic(DeepWaterTexture, null, new Color(20, 50, 150, 255)), 0);
    public static readonly Tile Water = new(TileType.Water, new StaticTileGraphic(WaterTexture, null, new Color(40, 100, 200, 255)), 1);
    public static readonly Tile Sand = new(TileType.Sand, new AutoTileGraphic(SandAutotile, 16, Water, new Color(230, 210, 130, 255)), 2);
    public static readonly Tile Grass = new(TileType.Grass, new AutoTileGraphic(GrassAutotile, 16, Sand, new Color(50, 160, 60, 255)), 3);
    public static readonly Tile Forest = new(TileType.Forest, new ColorTileGraphic(new Color(20, 110, 30, 255)), 4);
    public static readonly Tile Mountain = new(TileType.Mountain, new ColorTileGraphic(new Color(120, 120, 120, 255)), 5);
    public static readonly Tile Snow = new(TileType.Snow, new ColorTileGraphic(Color.White), 6);

}
