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
    private static float Speed = 5f;
    private int dx = 0;
    private int dy = 0;
    private float AnimationFrame = 0f;
    private int Row = 0;


    public void Update(float dt)
    {
        Vector2 oldPosition = Position;
        dx = 0;
        dy = 0;

        if (Raylib.IsKeyDown(KeyboardKey.A))
        {
            dx = -1;
            Row = 2;
        }
        if (Raylib.IsKeyDown(KeyboardKey.W))
        {
            dy = -1;
            Row = 1;
        }
        if (Raylib.IsKeyDown(KeyboardKey.D))
        {
            dx = 1;
            Row = 3;
        }
        if (Raylib.IsKeyDown(KeyboardKey.S))
        {
            dy = 1;
            Row = 0;
        }

        Move(dx, dy, dt);

        Vector2 newPosition = Position;

        // TODO: idle animation
        if (newPosition - oldPosition == Vector2.Zero)
        {
            AnimationFrame = 0; // sets the frame to the first of the selected row animation
        }

    }

    private void Move(int dx, int dy, float dt)
    {
        AnimationFrame = (AnimationFrame + 0.1f) % 3;
        float vx = dx * Speed;
        float vy = dy * Speed;

        if (dx != 0 && dy != 0)
        {
            vx /= 1.414f;
            vy /= 1.414f;
        }

        // TODO: collisions
        Position.X += vx * dt;
        Position.Y += vy * dt;
    }

    public void Render()
    {
        int flooredFrame = (int)AnimationFrame;
        SourceRect.X = flooredFrame * SourceWidth;
        SourceRect.Y = Row * SourceHeight;
        SourceRect.Width = SourceWidth;
        SourceRect.Height = SourceHeight;


        Raylib.DrawTexturePro(Spritesheet,
                              SourceRect,
                              new Rectangle(Position * Game.world.ScaledTileSize - Game.cameraPos, Width * (Game.world.ScaledTileSize / (float)Game.world.TileSize), Height * (Game.world.ScaledTileSize / (float)Game.world.TileSize)),
                              Vector2.Zero,
                              0f,
                              Color.White);
    }
}
