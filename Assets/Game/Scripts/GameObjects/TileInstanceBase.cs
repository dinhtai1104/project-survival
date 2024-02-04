using UnityEngine;
using UnityEngine.Tilemaps;

public delegate void OnTileDisactive();

public class TileInstanceBase : MonoBehaviour
{
    protected OnTileDisactive onTileDisactive;
    protected Vector3Int positionTile;
    private Tilemap tilemap;
    protected Tilemap TileMap
    {
        get
        {
            if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();
            return tilemap;
        }
    }
    public void SetTileData(Vector3Int position)
    {
        this.positionTile = position;
    }
    public void Subscribe(OnTileDisactive subcribe)
    {
        onTileDisactive += subcribe;
    }
    public void Unscribe(OnTileDisactive subcribe)
    {
        onTileDisactive -= subcribe;
    }
    protected virtual void OnDisable()
    {
        onTileDisactive?.Invoke();
       // RemoveTile();
        onTileDisactive = null;
    }

    public void RemoveTile()
    {
        try
        {
            var pos = TileMap.WorldToCell(transform.position);
            TileMap?.SetTile(positionTile, null);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
}