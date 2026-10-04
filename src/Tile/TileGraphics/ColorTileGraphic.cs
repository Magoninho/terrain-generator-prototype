using System.Numerics;
using Raylib_cs;

namespace terrain_prototype_raylib.src.Tile;

// A tile with only color
public class ColorTileGraphic(Color color) : ITileGraphic
{
    public Color Color = color;

    public void Render(Rectangle dest, TileMap tileMap, int x, int y)
    {
        Raylib.DrawRectangle((int)dest.X, (int)dest.Y, (int)dest.Width, (int)dest.Height, Color);
    }
}
