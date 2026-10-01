using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

namespace terrain_prototype_raylib.src;

public class TileMap
{
    public Tile[,] map;
    // public Dictionary<TileType, Tile> TileRegistry;
    public Texture2D atlas;
    public int TileSize {get; set;}
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

    public void SetTile(int x, int y, Tile tile)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            map[x, y] = tile;
    }

    private Rectangle? GetTileSourceRect(int x, int y)
    {
        Tile tile = map[x, y];

        // TODO: change this logic to something more general later...
        if (tile.Type == TileType.Sand)
        {
            bool isWater(int nx, int ny)
            {
                if (nx < 0 || nx >= Width || ny < 0 || ny >= Height) return true;
                Tile neighbor = map[nx, ny];
                return neighbor.Type == TileType.Water || neighbor.Type == TileType.DeepWater;
            }

            bool isGrass(int nx, int ny)
            {
                if (nx < 0 || nx >= Width || ny < 0 || ny >= Height) return false;
                Tile neighbor = map[nx, ny];
                return neighbor.Type == TileType.Grass || neighbor.Type == TileType.Forest;
            }

            bool hasWaterAround = isWater(x, y - 1) || isWater(x, y + 1) || isWater(x - 1, y) || isWater(x + 1, y) || 
                                  isWater(x - 1, y - 1) || isWater(x + 1, y - 1) || isWater(x - 1, y + 1) || isWater(x + 1, y + 1);
            bool hasGrassAround = isGrass(x, y - 1) || isGrass(x, y + 1) || isGrass(x - 1, y) || isGrass(x + 1, y) || 
                                  isGrass(x - 1, y - 1) || isGrass(x + 1, y - 1) || isGrass(x - 1, y + 1) || isGrass(x + 1, y + 1);

            if (hasWaterAround)
            {
                return GetSandToWaterTransitionRect(x, y);
            }
            if (hasGrassAround)
            {
                return GetSandTransitionRect(x, y);
            }

            return new Rectangle(10 * 16, 1 * 16, 16, 16);
        }

        return tile.SourceRect;
    }

    private Rectangle GetSandToWaterTransitionRect(int x, int y)
    {
        // Helper function to check if neighbor at (nx, ny) is Water or DeepWater
        bool isWater(int nx, int ny)
        {
            if (nx < 0 || nx >= Width || ny < 0 || ny >= Height) return true;
            Tile neighbor = map[nx, ny];
            return neighbor.Type == TileType.Water || neighbor.Type == TileType.DeepWater;
        }

        // Check 4 cardinal neighbors
        bool hasN = isWater(x, y - 1);     // North
        bool hasS = isWater(x, y + 1);     // South
        bool hasW = isWater(x - 1, y);     // West
        bool hasE = isWater(x + 1, y);     // East

        // Check 4 diagonal neighbors
        bool hasNW = isWater(x - 1, y - 1); // North-West
        bool hasNE = isWater(x + 1, y - 1); // North-East
        bool hasSW = isWater(x - 1, y + 1); // South-West
        bool hasSE = isWater(x + 1, y + 1); // South-East

        // --- 1. OUTER CORNERS (Water on two adjacent cardinal sides) ---
        if (hasN && hasW)
        {
            return new Rectangle(3 * 16, 0 * 16, 16, 16); // Top-Left Outer Corner
        }
        if (hasN && hasE)
        {
            return new Rectangle(5 * 16, 0 * 16, 16, 16); // Top-Right Outer Corner
        }
        if (hasS && hasW)
        {
            return new Rectangle(3 * 16, 2 * 16, 16, 16); // Bottom-Left Outer Corner
        }
        if (hasS && hasE)
        {
            return new Rectangle(5 * 16, 2 * 16, 16, 16); // Bottom-Right Outer Corner
        }

        // --- 2. EDGES (Water on one cardinal side) ---
        if (hasN)
        {
            return new Rectangle(4 * 16, 0 * 16, 16, 16); // Top Edge
        }
        if (hasS)
        {
            return new Rectangle(4 * 16, 2 * 16, 16, 16); // Bottom Edge
        }
        if (hasW)
        {
            return new Rectangle(3 * 16, 1 * 16, 16, 16); // Left Edge
        }
        if (hasE)
        {
            return new Rectangle(5 * 16, 1 * 16, 16, 16); // Right Edge
        }

        // --- 3. INNER CORNERS (Water on a diagonal side, surrounded by sand on cardinal sides) ---
        if (hasNW)
        {
            return new Rectangle(3 * 16, 3 * 16, 16, 16); // Inner Top-Left Corner
        }
        if (hasNE)
        {
            return new Rectangle(4 * 16, 3 * 16, 16, 16); // Inner Top-Right Corner
        }
        if (hasSW)
        {
            return new Rectangle(3 * 16, 4 * 16, 16, 16); // Inner Bottom-Left Corner
        }
        if (hasSE)
        {
            return new Rectangle(4 * 16, 4 * 16, 16, 16); // Inner Bottom-Right Corner
        }

        // --- 4. DEFAULT SAND (Pure Sand) ---
        return new Rectangle(10 * 16, 1 * 16, 16, 16);
    }

    private Rectangle GetSandTransitionRect(int x, int y)
    {
        // Helper function to check if neighbor at (nx, ny) is Grass or Forest
        bool isGrass(int nx, int ny)
        {
            if (nx < 0 || nx >= Width || ny < 0 || ny >= Height) return false;
            Tile neighbor = map[nx, ny];
            return neighbor.Type == TileType.Grass || neighbor.Type == TileType.Forest;
        }

        // Check 4 cardinal neighbors
        bool hasN = isGrass(x, y - 1);     // North
        bool hasS = isGrass(x, y + 1);     // South
        bool hasW = isGrass(x - 1, y);     // West
        bool hasE = isGrass(x + 1, y);     // East

        // Check 4 diagonal neighbors
        bool hasNW = isGrass(x - 1, y - 1); // North-West
        bool hasNE = isGrass(x + 1, y - 1); // North-East
        bool hasSW = isGrass(x - 1, y + 1); // South-West
        bool hasSE = isGrass(x + 1, y + 1); // South-East

        // --- 1. OUTER CORNERS (Grass on two adjacent cardinal sides) ---
        if (hasN && hasW)
        {
            return new Rectangle(9 * 16, 0 * 16, 16, 16); // Top-Left Outer Corner
        }
        if (hasN && hasE)
        {
            return new Rectangle(11 * 16, 0 * 16, 16, 16); // Top-Right Outer Corner
        }
        if (hasS && hasW)
        {
            return new Rectangle(9 * 16, 2 * 16, 16, 16); // Bottom-Left Outer Corner
        }
        if (hasS && hasE)
        {
            return new Rectangle(11 * 16, 2 * 16, 16, 16); // Bottom-Right Outer Corner
        }

        // --- 2. EDGES (Grass on one cardinal side) ---
        if (hasN)
        {
            return new Rectangle(10 * 16, 0 * 16, 16, 16); // Top Edge
        }
        if (hasS)
        {
            return new Rectangle(10 * 16, 2 * 16, 16, 16); // Bottom Edge
        }
        if (hasW)
        {
            return new Rectangle(9 * 16, 1 * 16, 16, 16); // Left Edge
        }
        if (hasE)
        {
            return new Rectangle(11 * 16, 1 * 16, 16, 16); // Right Edge
        }

        // --- 3. INNER CORNERS (Grass on a diagonal side, surrounded by sand on cardinal sides) ---
        if (hasNW)
        {
            return new Rectangle(9 * 16, 3 * 16, 16, 16); // Inner Top-Left Corner
        }
        if (hasNE)
        {
            return new Rectangle(10 * 16, 3 * 16, 16, 16); // Inner Top-Right Corner
        }
        if (hasSW)
        {
            return new Rectangle(9 * 16, 4 * 16, 16, 16); // Inner Bottom-Left Corner
        }
        if (hasSE)
        {
            return new Rectangle(10 * 16, 4 * 16, 16, 16); // Inner Bottom-Right Corner
        }

        // --- 4. DEFAULT SAND (Pure Sand) ---
        return new Rectangle(10 * 16, 1 * 16, 16, 16);
    }

    public void Render()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile currentTile = map[x, y];
                Vector2 position = new Vector2(x * TileSize - Game.cameraPos.X, y * TileSize - Game.cameraPos.Y);

                Rectangle? sourceRect = GetTileSourceRect(x, y);

                if (sourceRect.HasValue)
                {
                    if (currentTile.Type == TileType.Sand)
                    {
                        // Background water tone in case the tileset has transparent water pixels in sand-to-water transition
                        Raylib.DrawRectangle((int)position.X, (int)position.Y, TileSize, TileSize, new Color(40, 100, 200, 255));
                    }
                    Raylib.DrawTexturePro(atlas, (Rectangle)sourceRect, new Rectangle((int)position.X, (int)position.Y, TileSize, TileSize), new Vector2(0f, 0f), 0.0f, Color.White);
                }
                else
                {
                    Raylib.DrawRectangle((int)position.X, (int)position.Y, TileSize, TileSize, currentTile.FallbackColor);
                }
            }
        }
    }
}
