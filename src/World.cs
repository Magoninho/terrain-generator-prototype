using System.IO;
using System.Numerics;
using Raylib_cs;
using terrain_prototype_raylib.src;
using terrain_prototype_raylib.src.Tile;
using terrain_prototype_raylib.src.WorldGen;

namespace terrain_prototype_raylib;

public class World
{
    public readonly int WIDTH = 400;
    public readonly int HEIGHT = 400;
    public readonly int TileSize = 16;   // base tile size in pixels, never changes
    public float Scale { get; set; } = 1f;

    // Tile size on screen after scaling
    public int ScaledTileSize => (int)MathF.Round(TileSize * Scale);

    private const float MinScale = 0.25f;
    private const float MaxScale = 16f;
    private const float ZoomStep = 0.25f;
    public Game Game;
    public TileMap TileMap;

    public Vector2 SpawnPoint;

    public Player Player;

    public World(Game game)
    {
        Game = game;
        TileMap = new TileMap(WIDTH, HEIGHT, TileSize);
        GenerateTerrain();

        Player = new(Game, FindSpawnPoint());

    }

    public void ApplyZoom(int factor)
    {
        SetScale(Scale + ZoomStep * factor);
    }

    public void ResetZoom()
    {
        SetScale(1f);
    }

    public void SetScale(float newScale)
    {
        if (newScale < MinScale || newScale > MaxScale) return;

        int oldSize = ScaledTileSize;
        Scale = newScale;
        int newSize = ScaledTileSize;

        if (newSize == oldSize) return;

        Vector2 anchor = new(Game.WINDOW_WIDTH / 2f, Game.WINDOW_HEIGHT / 2f);

        // World position under the anchor, measured in tiles (float)
        float tileX = (Game.cameraPos.X + anchor.X) / oldSize;
        float tileY = (Game.cameraPos.Y + anchor.Y) / oldSize;

        TileMap.TileSize = newSize; // TODO: remove once TileMap reads the scale directly

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

                if (elevation < 0.1f) TileMap.SetTile(x, y, Tiles.DeepWater);
                else if (elevation < 0.25f) TileMap.SetTile(x, y, Tiles.Water);
                else if (elevation < 0.35f) TileMap.SetTile(x, y, Tiles.Sand);
                else if (elevation < 0.60f) TileMap.SetTile(x, y, Tiles.Grass);
                else if (elevation < 0.85f) TileMap.SetTile(x, y, Tiles.Forest);
                else if (elevation < 0.95f) TileMap.SetTile(x, y, Tiles.Mountain);
                else TileMap.SetTile(x, y, Tiles.Snow);
            }
        }
    }

    // this function will go from bottom on the map to top to find the first sand tile
    public Vector2 FindSpawnPoint()
    {
        if (TileMap.map[0, 0] == null) return new Vector2(0, 0);

        int centerTileX = WIDTH / 2;
        int bottomTileY = HEIGHT - 1;

        for (int Y = bottomTileY; Y > 0; Y--)
        {
            Tile tile = TileMap.map[centerTileX, Y];
            if (tile.Type == TileType.Sand)
                return new Vector2(centerTileX, Y);
        }
        return new Vector2(HEIGHT / 2, centerTileX);
    }

    public void Update(float dt)
    {
        Player.Update(dt);
    }

    public void Render()
    {
        TileMap.Render();

        // TEMP
        // Raylib.DrawRectangle((int)(SpawnPoint.X * Tilesize - Game.cameraPos.X), (int)(SpawnPoint.Y * Tilesize - Game.cameraPos.Y), Tilesize, Tilesize, Color.Red);

        Player.Render();

    }
}
