using Raylib_cs;

namespace terrain_prototype_raylib;

internal static class Program
{
    [System.STAThread]
    public static void Main()
    {
        
        Game game = new Game();
        game.Start();

    }
}