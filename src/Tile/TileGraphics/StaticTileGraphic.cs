using System.Numerics;
using Raylib_cs;

namespace terrain_prototype_raylib.src.Tile;

// A tile with texture
// it can be just a tile image or an atlas with multiple tiles
// in case of atlas, you can specify the source rect where the tile is located in the atlas
public class StaticTileGraphic(Texture2D texture, Rectangle? sourceRect, Color fallbackColor) : ITileGraphic
{
    public readonly Color FallbackColor = fallbackColor;
    public readonly Texture2D Texture = texture;
    public readonly Rectangle? SourceRect = sourceRect;

    public void Render(Rectangle dest, TileMap tileMap, int x, int y)
    {
        Rectangle src = SourceRect ?? new Rectangle(0, 0, Texture.Width, Texture.Height);
        Raylib.DrawTexturePro(Texture, src, dest, Vector2.Zero, 0f, Color.White);
    }

}
