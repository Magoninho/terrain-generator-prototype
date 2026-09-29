using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;
using terrain_prototype_raylib.src;

namespace terrain_prototype_raylib;

public class World
{
    const int WIDTH = 200;
    const int HEIGHT = 200;
    const int TILESIZE = 4;
    public Game Game;
    public Texture2D atlas;

    public TileMap TileMap;

    public World(Game game)
    {
        this.Game = game;
        atlas = Raylib.LoadTexture("assets/tileset.png");
        TileMap = new TileMap(WIDTH, HEIGHT, TILESIZE, atlas);

        TileMap.DefineTile(TileType.DeepWater, null, false, new Color(20, 50, 150, 255));
        TileMap.DefineTile(TileType.Water, null, false, new Color(40, 100, 200, 255));
        TileMap.DefineTile(TileType.Sand, new Rectangle(160f, 16f, 16f, 16f), false, new Color(230, 210, 130, 255));
        TileMap.DefineTile(TileType.Grass, new Rectangle(16f, 16f, 16f, 16f), false, new Color(50, 160, 60, 255));
        TileMap.DefineTile(TileType.Forest, new Rectangle(64f, 176f, 16f, 16f), false, new Color(20, 110, 30, 255));
        TileMap.DefineTile(TileType.Mountain, null, false, new Color(120, 120, 120, 255));
        TileMap.DefineTile(TileType.Snow, new Rectangle(208f, 16f, 16f, 16f), false, Color.White);

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

                if (elevation < 0.1f) TileMap.SetTile(x, y, TileType.DeepWater);
                else if (elevation < 0.25f) TileMap.SetTile(x, y, TileType.Water);
                else if (elevation < 0.35f) TileMap.SetTile(x, y, TileType.Sand);
                else if (elevation < 0.60f) TileMap.SetTile(x, y, TileType.Grass);
                else if (elevation < 0.75f) TileMap.SetTile(x, y, TileType.Forest);
                else if (elevation < 0.85f) TileMap.SetTile(x, y, TileType.Mountain);
                else TileMap.SetTile(x, y, TileType.Snow);
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
        if (TileMap.map.Length == 0) return false;

        for (int row = 0; row < TileMap.map.GetLength(0); row++)
        {
            for (int col = 0; col < TileMap.map.GetLength(1); col++)
            {
                if (TileMap.map[row, col] == TileType.Snow)
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
        TileMap.Render();
    }
}
