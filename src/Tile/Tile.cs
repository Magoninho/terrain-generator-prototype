
using Raylib_cs;

namespace terrain_prototype_raylib.src.Tile;

public class Tile
{
    public TileType Type {get; set;}
    public bool IsSolid;
    public ITileGraphic Graphic {get;}

    public Tile(TileType type, ITileGraphic graphic, bool isSolid = false)
    {
        Type = type;
        IsSolid = isSolid;
        Graphic = graphic;
    }

    public void Render(Rectangle dest)
    {
        Graphic.Render(dest);
    }
}
