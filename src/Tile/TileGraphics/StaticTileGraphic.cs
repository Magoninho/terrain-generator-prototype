using System.Numerics;
using Raylib_cs;

namespace terrain_prototype_raylib.src.Tile;

public class StaticTileGraphic(Texture2D texture, Rectangle? sourceRect, Color fallbackColor) : ITileGraphic
{
    public Color FallbackColor = fallbackColor;
    public Texture2D Texture = texture;
    public Rectangle? SourceRect = sourceRect;

    public void Render(Rectangle dest)
    {
        Rectangle src = SourceRect ?? new Rectangle(0, 0, texture.Width, texture.Height);
        Raylib.DrawTexturePro(Texture, src, dest, Vector2.Zero, 0f, Color.White);
    }
}
