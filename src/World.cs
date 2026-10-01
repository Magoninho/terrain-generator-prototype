using System.IO;
using System.Numerics;
using Raylib_cs;
using terrain_prototype_raylib.src;

namespace terrain_prototype_raylib;

public class World
{
    const int WIDTH = 200;
    const int HEIGHT = 200;
    public int Tilesize {get; set;} = 4;
    public Game Game;
    public Texture2D atlas;

    public TileMap TileMap;

    readonly Tile TileDeepWater = new(TileType.DeepWater, null, new Color(20, 50, 150, 255));
    readonly Tile TileWater = new(TileType.Water, null, new Color(40, 100, 200, 255));
    readonly Tile TileSand = new(TileType.Sand, new Rectangle(160f, 16f, 16f, 16f), new Color(230, 210, 130, 255));
    readonly Tile TileGrass = new(TileType.Grass, new Rectangle(16f, 16f, 16f, 16f), new Color(50, 160, 60, 255));
    readonly Tile TileForest = new(TileType.Forest, null, new Color(20, 110, 30, 255));
    readonly Tile TileMountain = new(TileType.Mountain, null, new Color(120, 120, 120, 255));
    readonly Tile TileSnow = new(TileType.Snow, new Rectangle(208f, 16f, 16f, 16f), Color.White);

    public World(Game game)
    {
        Game = game;
        string tilesetPath = File.Exists("assets/tileset_water.png") ? "assets/tileset_water.png" : "assets/tileset.png";
        atlas = Raylib.LoadTexture(tilesetPath);
        TileMap = new TileMap(WIDTH, HEIGHT, Tilesize, atlas);

        GenerateTerrain();
    }

    public void ApplyZoom(int factor)
    {
        int oldSize = TileMap.TileSize;
        int zoom = 4 * factor;
        int newSize = oldSize + zoom;

        Vector2 anchor = new(Game.WINDOW_WIDTH / 2f, Game.WINDOW_HEIGHT / 2f);

        // Optional: avoid zero/negative or absurd tile sizes
        if (newSize < 4 || newSize > 256) return;

        // World position under the anchor, measured in tiles (float)
        float tileX = (Game.cameraPos.X + anchor.X) / oldSize;
        float tileY = (Game.cameraPos.Y + anchor.Y) / oldSize;

        Tilesize = newSize;
        TileMap.TileSize = newSize;

        // Put that same tile position back under the anchor
        Game.cameraPos.X = tileX * newSize - anchor.X;
        Game.cameraPos.Y = tileY * newSize - anchor.Y;
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
        Random rand = new();
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

                if (elevation < 0.1f) TileMap.SetTile(x, y, TileDeepWater);
                else if (elevation < 0.25f) TileMap.SetTile(x, y, TileWater);
                else if (elevation < 0.35f) TileMap.SetTile(x, y, TileSand);
                else if (elevation < 0.60f) TileMap.SetTile(x, y, TileGrass);
                else if (elevation < 0.85f) TileMap.SetTile(x, y, TileForest);
                else if (elevation < 0.95f) TileMap.SetTile(x, y, TileMountain);
                else TileMap.SetTile(x, y, TileSnow);
            }
        }
    }

    public void Update()
    {

    }

    public void Render()
    {
        TileMap.Render();
    }
}
