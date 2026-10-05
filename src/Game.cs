using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;
using terrain_prototype_raylib.src.Audio;

namespace terrain_prototype_raylib;

public class Game
{
    public const int WINDOW_WIDTH = 640;
    public const int WINDOW_HEIGHT = 480;

    public static Vector2 cameraPos;
    public World world;

    public MusicManager MusicManager;

    public void Setup()
    {
        cameraPos = new(0, 0);
        world = new(this);
        MusicManager = new MusicManager();

        MusicManager.CreatePlaylist("Teste", [
            "assets/audio/music/MainTheme.mp3",
            "assets/audio/music/DistantHarp.mp3"
        ]);

        MusicManager.Play("Teste");
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

    public void Update(float dt)
    {
        MusicManager.Update();
        if (Raylib.IsKeyDown(KeyboardKey.Right)) cameraPos.X += 550.0f * dt;
        if (Raylib.IsKeyDown(KeyboardKey.Left)) cameraPos.X -= 550.0f * dt;
        if (Raylib.IsKeyDown(KeyboardKey.Up)) cameraPos.Y -= 550.0f * dt;
        if (Raylib.IsKeyDown(KeyboardKey.Down)) cameraPos.Y += 550.0f * dt;
        if (Raylib.IsKeyPressed(KeyboardKey.Space)) world.GenerateTerrain();
        if (Raylib.IsKeyPressed(KeyboardKey.E)) world.ApplyZoom(1);
        if (Raylib.IsKeyPressed(KeyboardKey.Q)) world.ApplyZoom(-1);
        if (Raylib.IsKeyPressed(KeyboardKey.F)) world.SpawnPoint = world.FindSpawnPoint();
    }

    public void Render()
    {
        world.Render();
    }

}
