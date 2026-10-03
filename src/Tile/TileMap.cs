using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

namespace terrain_prototype_raylib.src.Tile;

public class TileMap
{
    public Tile[,] map;
    public Texture2D atlas;
    public int TileSize { get; set; }
    public readonly int Width;
    public readonly int Height;

    public TileMap(int width, int height, int tilesize, Texture2D atlas)
    {
        Width = width;
        Height = height;
        TileSize = tilesize;
        map = new Tile[width, height];
        this.atlas = atlas;
    }

    public bool Connects(Tile tile, int x, int y)
    {
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
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile currentTile = map[x, y];
                Vector2 position = new Vector2(x * TileSize - Game.cameraPos.X, y * TileSize - Game.cameraPos.Y);

                // TODO: Make use of the z-order attribute from tile
                currentTile.Render(new Rectangle((int)position.X, (int)position.Y, TileSize, TileSize), this, x, y);

                
            }
        }
    }
}
