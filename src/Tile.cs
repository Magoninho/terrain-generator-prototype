using System;
using Raylib_cs;

namespace terrain_prototype_raylib.src;

public class Tile
{
    public bool IsSolid;
    public Rectangle? SourceRect {get; set;}
    public Color TintColor {get; set;}

    public Tile(bool isSolid = false)
    {
        IsSolid = isSolid;
    }
}
