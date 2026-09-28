using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

namespace terrain_prototype_raylib;

public class World
{
    const int WIDTH = 200;
    const int HEIGHT = 200;
    const int TILESIZE = 4;
    public Game game;

    public TileType[,] map;

    public World(Game game)
    {
        this.game = game;
        map = new TileType[WIDTH, HEIGHT];
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        Random rand = new Random();
        float offsetX = (float)rand.NextDouble() * 10000f;
        float offsetY = (float)rand.NextDouble() * 10000f;

        float centerX = WIDTH / 2f;
        float centerY = HEIGHT / 2f;
        float radius = MathF.Min(centerX, centerY);

        float scale = 0.01f;
        float islandSize = 1f;

        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                // 1. Normalize coordinates from -1.0 to 1.0 (where 0,0 is the center of the map)
                float nx = 2f * x / (WIDTH - 1) - 1f;
                float ny = 2f * y / (HEIGHT - 1) - 1f;

                // 2. Calculate distance from the center
                float distance = MathF.Sqrt(nx * nx + ny * ny);

                // 3. Create a dome mask (1.0 at center, 0.0 at the edges)
                float mask = 1.0f - distance;

                // 4. Sample noise
                float rawNoise = Perlin.Noise((x * scale) + offsetX, (y * scale) + offsetY);

                // 5. Base the primary elevation on the mask, and use noise just for variation
                float elevation = mask + (rawNoise * 0.5f);

                // 6. Map to tiles using the new elevation logic
                if (elevation < 0.1f) map[x, y] = TileType.DeepWater;
                else if (elevation < 0.25f) map[x, y] = TileType.Water;
                else if (elevation < 0.35f) map[x, y] = TileType.Sand;
                else if (elevation < 0.60f) map[x, y] = TileType.Grass;
                else if (elevation < 0.75f) map[x, y] = TileType.Forest;
                else if (elevation < 0.80f) map[x, y] = TileType.Mountain;
                else map[x, y] = TileType.Snow; // Forces the highest central points to be snow
            }
        }

        if (!IsValidTerrain())
        {
            GenerateTerrain();
        }
    }

    // checks if map contains snow
    private bool IsValidTerrain()
    {
        if (map.Length == 0) return false;

        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
                if (map[row, col] == TileType.Snow)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void Update()
    {

    }

    public void Render()
    {
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < HEIGHT; j++)
            {
                int drawX = i * TILESIZE - (int)game.cameraPos.X;
                int drawY = j * TILESIZE - (int)game.cameraPos.Y;

                Color tileColor = map[i, j] switch
                {
                    TileType.DeepWater => new Color(20, 50, 150, 255),
                    TileType.Water => new Color(40, 100, 200, 255),
                    TileType.Sand => new Color(230, 210, 130, 255),
                    TileType.Grass => new Color(50, 160, 60, 255),
                    TileType.Forest => new Color(20, 110, 30, 255),
                    TileType.Mountain => new Color(120, 120, 120, 255),
                    TileType.Snow => Color.White,
                    _ => Color.Magenta
                };
                Raylib.DrawRectangle(drawX, drawY, TILESIZE, TILESIZE, tileColor);
            }
        }
    }
}
