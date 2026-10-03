
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
        // before rendering this tile,
        // if there's a tile around that has lower z index
        // render that under the tile
        if (x >= 0 && x < tileMap.Width && y >= 0 && y < tileMap.Height)
        {
            List<Tile> tilesAround = new List<Tile>();

            // Check each neighbor with bounds validation
            if (y - 1 >= 0) tilesAround.Add(tileMap.map[x, y - 1]);
            if (y + 1 < tileMap.Height) tilesAround.Add(tileMap.map[x, y + 1]);
            if (x - 1 >= 0) tilesAround.Add(tileMap.map[x - 1, y]);
            if (x + 1 < tileMap.Width) tilesAround.Add(tileMap.map[x + 1, y]);

            // Corners
            if (x - 1 >= 0 && y - 1 >= 0) tilesAround.Add(tileMap.map[x - 1, y - 1]);
            if (x + 1 < tileMap.Width && y - 1 >= 0) tilesAround.Add(tileMap.map[x + 1, y - 1]);
            if (x - 1 >= 0 && y + 1 < tileMap.Height) tilesAround.Add(tileMap.map[x - 1, y + 1]);
            if (x + 1 < tileMap.Width && y + 1 < tileMap.Height) tilesAround.Add(tileMap.map[x + 1, y + 1]);

            foreach (Tile tile in tilesAround)
            {
                if (tile.ZIndex < ZIndex)
                {
                    tileMap.map[x, y] = tile;
                    tile.Render(dest, tileMap, x, y);
                }
            }
            tileMap.map[x, y] = this;

            Graphic.Render(dest, tileMap, x, y);
        }
    }
}
