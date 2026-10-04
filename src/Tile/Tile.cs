
using Raylib_cs;

namespace terrain_prototype_raylib.src.Tile;

public class Tile(TileType type, ITileGraphic graphic, int zIndex = 0, bool isSolid = false)
{
    public TileType Type { get; set; } = type;
    public ITileGraphic Graphic { get; } = graphic;
    public bool IsSolid = isSolid;
    public int ZIndex = zIndex; // to control wether the tile will go over the others and stuff

    public void Render(Rectangle dest, TileMap tileMap, int x, int y)
    {
        // if (Type == TileType.Grass) return;
        // before rendering this tile,
        // if there's a tile around that has lower z index
        // render that under the tile
        if (x >= 0 && x < tileMap.Width && y >= 0 && y < tileMap.Height)
        {
            Graphic.Render(dest, tileMap, x, y);
        }
    }
}
