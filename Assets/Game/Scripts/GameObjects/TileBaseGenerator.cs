using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBaseGenerator : Tile
{
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        var resul = base.StartUp(position, tilemap, go);
        return resul;
    }
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
    }
}