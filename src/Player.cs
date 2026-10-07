using System.Numerics;
using Raylib_cs;

namespace terrain_prototype_raylib.src;

public class Player(Game game, Vector2 position)
{
    public Game Game = game;
    private Texture2D Spritesheet = Raylib.LoadTexture("assets/player.png");
    public Vector2 Position = position;
    public float Width = 16;
    public float Height = 18;
    private Rectangle SourceRect = new();
    private int SourceWidth = 16;
    private int SourceHeight = 18;
    private static float Speed = 1.4f;
    private int dx = 0;
    private int dy = 0;
    private int AnimationFrame = 0;
    private int Row = 0;
    

    public void Update(float dt)
    {
        dx = 0;
        dy = 0;

        // if (Raylib)
    }
    public void Render()
    {
        SourceRect.X = AnimationFrame * SourceWidth;
        SourceRect.Y = Row * SourceHeight;
        SourceRect.Width = SourceWidth;
        SourceRect.Height = SourceHeight;


        Raylib.DrawTexturePro(Spritesheet, 
                              SourceRect, 
                              new Rectangle(Position * Game.world.Tilesize - Game.cameraPos, Width * (Game.world.Tilesize / 16f), Height * (Game.world.Tilesize / 16f)), 
                              Vector2.Zero, 
                              0f, 
                              Color.White);
    }
}
