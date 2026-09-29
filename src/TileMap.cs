using System.Numerics;
using Raylib_cs;

namespace terrain_prototype_raylib.src;

public class TileMap
{
    public TileType[,] map;
    public Dictionary<TileType, Tile> TileRegistry;
    public Texture2D atlas;
    public int TileSize;
    public readonly int Width;
    public readonly int Height;

    public TileMap(int width, int height, int tilesize, Texture2D atlas)
    {
        Width = width;
        Height = height;
        TileSize = tilesize;
        TileRegistry = new Dictionary<TileType, Tile>();
        map = new TileType[width, height];
        this.atlas = atlas;

    }

    public void DefineTile(TileType tileType, Rectangle sourceRect, bool isSolid)
    {
        TileRegistry[tileType] = new Tile(sourceRect, isSolid);
    }

    public void SetTile(int x, int y, TileType tileType)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            map[x, y] = tileType;
    }

    public void Render()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                TileType currentTile = map[x, y];
                Console.WriteLine($"{currentTile}");
                // if (currentTile == 0 || !TileRegistry.TryGetValue(currentTile, out _))
                //     continue;

                Tile data = TileRegistry[currentTile];
                Vector2 position = new Vector2(x * TileSize - Game.cameraPos.X, y * TileSize - Game.cameraPos.Y);
                Raylib.DrawTextureRec(atlas, data.SourceRect, position, Color.White);
            }
        }
    }
}
