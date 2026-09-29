using System.Numerics;
using Raylib_cs;

namespace terrain_prototype_raylib.src;

public class TileMap
{
    public Tile[,] map;
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
        map = new Tile[width, height];
        this.atlas = atlas;
    }

    public void DefineTile(TileType tileType, Tile tile)
    {
        TileRegistry[tileType] = tile;
    }

    public void SetTile(int x, int y, TileType tileType)
    {
        // if the tiletype was not defined earlier
        // assign a pink tile then
        if (!TileRegistry.TryGetValue(tileType, out _))
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                map[x, y] = new Tile(null, Color.Pink, false);
            return;
        }
        
        Tile tile = TileRegistry[tileType];
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            map[x, y] = tile;
    }

    public void Render()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile currentTile = map[x, y];
                Vector2 position = new Vector2(x * TileSize - Game.cameraPos.X, y * TileSize - Game.cameraPos.Y);

                // TODO: implement sand to grass transition
                // create a method to return a custom sourcerect based on neighbours

                if (currentTile.SourceRect.HasValue)
                    Raylib.DrawTexturePro(atlas, (Rectangle)currentTile.SourceRect, new Rectangle((int)position.X, (int)position.Y, TileSize, TileSize), new Vector2(0f, 0f), 0.0f, Color.White);
                else
                    Raylib.DrawRectangle((int)position.X, (int)position.Y, TileSize, TileSize, currentTile.FallbackColor);

            }
        }
    }
}
