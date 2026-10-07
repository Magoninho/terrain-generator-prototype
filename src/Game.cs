using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;
using terrain_prototype_raylib.src.Audio;

namespace terrain_prototype_raylib;

public class Game
{
    public const int WINDOW_WIDTH = 640;
    public const int WINDOW_HEIGHT = 480;
    public bool MusicEnabled = false;
    public bool FreeCam = false;
    public static Vector2 cameraPos;
    public World world;
    public MusicManager MusicManager = new();

    public void Setup()
    {
        cameraPos = new(0, 0);
        world = new(this);

        if (MusicEnabled)
        {
            MusicManager.CreatePlaylist("Overworld", [
                "assets/audio/music/MainTheme.mp3",
                "assets/audio/music/DistantHarp.mp3"
            ]);

            MusicManager.Play("Overworld");
        }
    }

    public void Start()
    {
        Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "Terrain Prototype");
        Raylib.InitAudioDevice();
        Raylib.SetTargetFPS(60);

        Setup();

        while (!Raylib.WindowShouldClose())
        {
            Update(Raylib.GetFrameTime());

            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(40, 200, 250, 255));

            Render();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    public void ToggleFreeCam()
    {
        FreeCam = !FreeCam;
        // world.ApplyZoom(-1);
    }

    public void Update(float dt)
    {

        world.Update(dt);
        if (MusicEnabled)
            MusicManager.Update();

        if (Raylib.IsKeyPressed(KeyboardKey.C))
        {
            Console.WriteLine("vivo");
            ToggleFreeCam();
        }


        if (FreeCam)
        {
            if (Raylib.IsKeyDown(KeyboardKey.Right)) cameraPos.X += 250.0f * dt;
            if (Raylib.IsKeyDown(KeyboardKey.Left)) cameraPos.X -= 250.0f * dt;
            if (Raylib.IsKeyDown(KeyboardKey.Up)) cameraPos.Y -= 250.0f * dt;
            if (Raylib.IsKeyDown(KeyboardKey.Down)) cameraPos.Y += 250.0f * dt;
            if (Raylib.IsKeyPressed(KeyboardKey.Space)) world.GenerateTerrain();
            if (Raylib.IsKeyPressed(KeyboardKey.E)) world.ApplyZoom(1);
            if (Raylib.IsKeyPressed(KeyboardKey.Q)) world.ApplyZoom(-1);
            // if (Raylib.IsKeyPressed(KeyboardKey.F)) world.SpawnPoint = world.FindSpawnPoint();
        }
        else
        {
            // TEMP: create world scale later
            world.SetScale(2f);
            cameraPos = world.Player.Position * world.ScaledTileSize;
            cameraPos.X -= (WINDOW_WIDTH / 2f) - world.Player.Width / 2f;
            cameraPos.Y -= (WINDOW_HEIGHT / 2f) - world.Player.Height / 2f;
        }

    }

    public void Render()
    {
        world.Render();
    }

}
