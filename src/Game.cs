using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;

namespace terrain_prototype_raylib;

public class Game
{
    public const int WINDOW_WIDTH = 640;
    public const int WINDOW_HEIGHT = 480;

    public static Vector2 cameraPos;
    public World world;

    public Game()
    {
        cameraPos = new(0, 0);
        world = new(this);
        
    }

    public void Start()
    {
        
    }

    public void Update(float dt)
    {
        if (Raylib.IsKeyDown(KeyboardKey.Right)) cameraPos.X += 550.0f * dt;
        if (Raylib.IsKeyDown(KeyboardKey.Left)) cameraPos.X -= 550.0f * dt;
        if (Raylib.IsKeyDown(KeyboardKey.Up)) cameraPos.Y -= 550.0f * dt;
        if (Raylib.IsKeyDown(KeyboardKey.Down)) cameraPos.Y += 550.0f * dt;
        if (Raylib.IsKeyPressed(KeyboardKey.Space)) world.GenerateTerrain();
    }

    public void Render()
    {
        world.Render();
    }

}
