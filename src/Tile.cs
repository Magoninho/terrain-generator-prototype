using System;
using Raylib_cs;

namespace terrain_prototype_raylib.src;

public class Tile
{
    public Rectangle SourceRect;
    public bool IsSolid;
    public Color TintColor = Color.DarkGray;

    public Tile(Rectangle sourceRec, bool isSolid)
    {
        SourceRect = sourceRec;
        IsSolid = isSolid;
    }
}
