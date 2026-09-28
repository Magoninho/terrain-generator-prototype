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

    // Fractal Brownian motion: stacks several noise layers, each finer and weaker
    private float Fbm(float x, float y, int octaves, float lacunarity = 2f, float gain = 0.5f)
    {
        float sum = 0f, amp = 1f, freq = 1f, norm = 0f;
        for (int i = 0; i < octaves; i++)
        {
            sum += Perlin.Noise(x * freq, y * freq) * amp;
            norm += amp;
            amp *= gain;
            freq *= lacunarity;
        }
        return sum / norm; // keeps result in the same range as a single noise sample
    }

    // Ridged noise: sharp crests, good for mountain ranges (returns 0..1)
    private float Ridged(float x, float y, int octaves)
    {
        float sum = 0f, amp = 1f, freq = 1f, norm = 0f;
        for (int i = 0; i < octaves; i++)
        {
            float n = 1f - MathF.Abs(Perlin.Noise(x * freq, y * freq));
            sum += n * n * amp;
            norm += amp;
            amp *= 0.5f;
            freq *= 2f;
        }
        return sum / norm;
    }

    public void GenerateTerrain()
    {
        Random rand = new Random();
        // separate offsets so the layers don't line up with each other
        float offX1 = (float)rand.NextDouble() * 10000f, offY1 = (float)rand.NextDouble() * 10000f;
        float offX2 = (float)rand.NextDouble() * 10000f, offY2 = (float)rand.NextDouble() * 10000f;
        float offX3 = (float)rand.NextDouble() * 10000f, offY3 = (float)rand.NextDouble() * 10000f;

        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                float nx = 2f * x / (WIDTH - 1) - 1f;
                float ny = 2f * y / (HEIGHT - 1) - 1f;
                float distance = MathF.Sqrt(nx * nx + ny * ny);
                float mask = 1.0f - distance;

                // Layer 1: base shape, now with octaves and a higher frequency
                float baseNoise = Fbm(x * 0.03f + offX1, y * 0.03f + offY1, 4);
                float elevation = mask + baseNoise * 0.35f;

                // Layer 2: mountains, only on inland areas so they don't poke out of the sea
                float landFactor = Math.Clamp((mask - 0.25f) / 0.3f, 0f, 1f);
                float ridge = Ridged(x * 0.04f + offX2, y * 0.04f + offY2, 3);
                elevation += ridge * 0.35f * landFactor;

                // Layer 3: lakes, carve basins where this noise is high, inland only
                float lakeNoise = Fbm(x * 0.05f + offX3, y * 0.05f + offY3, 3);
                float lakeFactor = Math.Clamp((mask - 0.2f) / 0.2f, 0f, 1f);
                elevation -= MathF.Max(0f, lakeNoise - 0.15f) * 2.5f * lakeFactor;

                if (elevation < 0.1f) map[x, y] = TileType.DeepWater;
                else if (elevation < 0.25f) map[x, y] = TileType.Water;
                else if (elevation < 0.35f) map[x, y] = TileType.Sand;
                else if (elevation < 0.60f) map[x, y] = TileType.Grass;
                else if (elevation < 0.75f) map[x, y] = TileType.Forest;
                else if (elevation < 0.85f) map[x, y] = TileType.Mountain;
                else map[x, y] = TileType.Snow;
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
