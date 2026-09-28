using Raylib_cs;

namespace terrain_prototype_raylib;

internal static class Program
{
    [System.STAThread]
    public static void Main()
    {
        Game game = new Game();

        Raylib.InitWindow(Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT, "Terrain Prototype");
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            game.Update(Raylib.GetFrameTime());
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Blue);

            game.Render();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}