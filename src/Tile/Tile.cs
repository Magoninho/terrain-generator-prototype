using System;
using Raylib_cs;

namespace terrain_prototype_raylib.src;

public class Tile
{
    public TileType Type {get; set;}
    public bool IsSolid;
    public Rectangle? SourceRect { get; set; }
    public Color FallbackColor { get; set; }

    public Tile(TileType type, Rectangle? sourceRect, Color fallbackColor, bool isSolid = false)
    {
        Type = type;
        IsSolid = isSolid;
        SourceRect = sourceRect;
        FallbackColor = fallbackColor;
    }
}
