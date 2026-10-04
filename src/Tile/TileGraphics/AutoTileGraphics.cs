using System.Numerics;
using Raylib_cs;

namespace terrain_prototype_raylib.src.Tile;

// A tile with autotiling
// The given texture must be a 47 blob tileset in the Godot 3x3 pattern
public class AutoTileGraphic(Texture2D texture, int textureTileSize, Tile? underlayTile, Color fallbackColor) : ITileGraphic
{

    // this is the bitmask layout for the Godot 47 tiles pattern
    static readonly int[,] Layout =
    {
        // col: 0    1    2    3    4     5  6  7  8  9 10 11
        {      144, 176, 184, 152,  187,  440, 248, 190, 432, 506, 504, 216 },  // row 0
        {      146, 178, 186, 154,  434, 510, 507, 218, 438, 254, 0, 251 },  // row 1
        {       18,  50,  58,  26,  182, 447, 255, 155, 446, 511, 443, 219 },  // row 2
        {       16,  48,  56,  24,  250, 62, 59, 442, 54, 63, 191, 27 },  // row 3
    };
    static readonly Dictionary<int, (int col, int row)> Table = BuildTable();

    static Dictionary<int, (int col, int row)> BuildTable()
    {
        var table = new Dictionary<int, (int, int)>();
        for (int row = 0; row < Layout.GetLength(0); row++)
            for (int col = 0; col < Layout.GetLength(1); col++)
                if (Layout[row, col] != 0)
                    table[Layout[row, col]] = (col, row);
        return table;
    }

    public readonly Texture2D Texture = texture;
    public readonly int TextureTileSize = textureTileSize;
    public readonly Tile? UnderlayTile = underlayTile;
    public readonly Color FallbackColor = fallbackColor;

    // Godot's autotiling engine
    // this method calculates the bitmask value for a tile based on its neighbours, given the tilemap
    public static int ComputeBitmask(TileMap tileMap, int x, int y)
    {
        Tile tile = tileMap.map[x, y];
        
        bool t = tileMap.Connects(tile, x, y - 1);
        bool b = tileMap.Connects(tile, x, y + 1);
        bool l = tileMap.Connects(tile, x - 1, y);
        bool r = tileMap.Connects(tile, x + 1, y);

        bool tl = tileMap.Connects(tile, x - 1, y - 1);
        bool tr = tileMap.Connects(tile, x + 1, y - 1);
        bool bl = tileMap.Connects(tile, x - 1, y + 1);
        bool br = tileMap.Connects(tile, x + 1, y + 1);

        /*
         1   2   4        TL  T  TR
         8  16  32   =     L  C   R
         64 128 256        BL  B  BR
        */

        int mask = 16; // starts with 16 which is the no neighbours tile bitmask

        // cardinal directions first
        if (t) mask |= 2;
        if (b) mask |= 128;
        if (r) mask |= 32;
        if (l) mask |= 8;

        // the diagonal neighbour only makes difference to the tile if both cardinal exist first
        if (t && l && tl) mask |= 1;
        if (t && r && tr) mask |= 4;
        if (b && l && bl) mask |= 64;
        if (b && r && br) mask |= 256;

        return mask;
    }

    public void Render(Rectangle dest, TileMap tileMap, int x, int y)
    {
        int mask = ComputeBitmask(tileMap, x, y);

        if (!Table.TryGetValue(mask, out var _))
            Raylib.DrawRectangle((int)dest.X, (int)dest.Y, (int)dest.Width, (int)dest.Height, FallbackColor);

        Tile currentTile = tileMap.map[x, y];

        // if the tile has transparent edges (is not mask 511, which is all filled up), 
        // and has an underlay tile, then render that first
        if (mask != 511 && UnderlayTile != null)
        {
            // TODO: refactor this
            // pass the Tile to the ComputeBitmask function instead of setting it
            // this will require changing the Render method too
            tileMap.map[x, y] = UnderlayTile;
            UnderlayTile.Render(dest, tileMap, x, y);
            tileMap.map[x, y] = currentTile;
        }

        Rectangle src = new(Table[mask].col * TextureTileSize, Table[mask].row * TextureTileSize, TextureTileSize, TextureTileSize);
        Raylib.DrawTexturePro(Texture, src, dest, Vector2.Zero, 0f, Color.White);
    }
}
