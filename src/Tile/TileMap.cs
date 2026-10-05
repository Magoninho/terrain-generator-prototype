using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

namespace terrain_prototype_raylib.src.Tile;

public class TileMap
{
    public Tile[,] map;
    public int TileSize { get; set; }
    public readonly int Width;
    public readonly int Height;

    public TileMap(int width, int height, int tilesize)
    {
        Width = width;
        Height = height;
        TileSize = tilesize;
        map = new Tile[width, height];
    }

    public bool Connects(Tile tile, int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return false;
        Tile neighbour = map[x, y];
        return tile.Type == neighbour.Type || neighbour.ZIndex >= tile.ZIndex;
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            map[x, y] = tile;
    }

    public void Render()
    {
        int startX = Math.Max(0, (int)(Game.cameraPos.X / TileSize) - 1);
        int endX = Math.Min(Width - 1, (int)((Game.cameraPos.X + Game.WINDOW_WIDTH) / TileSize) + 1);
        int startY = Math.Max(0, (int)(Game.cameraPos.Y / TileSize) - 1);
        int endY = Math.Min(Height - 1, (int)((Game.cameraPos.Y + Game.WINDOW_HEIGHT) / TileSize) + 1);
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Tile currentTile = map[x, y];
                Vector2 position = new Vector2(x * TileSize - Game.cameraPos.X, y * TileSize - Game.cameraPos.Y);

                currentTile.Render(new Rectangle((int)position.X, (int)position.Y, TileSize, TileSize), this, x, y);
            }
        }
    }
}
