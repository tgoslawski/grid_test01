using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class GridTilemapAdapter : MonoBehaviour
{
    public Tilemap tilemap;

    void Reset()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public Vector3 CellToWorld(Vector2Int cell) {
        var pos = tilemap.CellToWorld(new Vector3Int(cell.x, 0, cell.y));
        // return center of cell (tile anchor might vary)
        var cellSize = tilemap.cellSize;
        return pos + new Vector3(cellSize.x * 0.5f, 0f, cellSize.y * 0.5f);
    }

    public Vector2Int WorldToCell(Vector3 world)
    {
        Vector3Int cell = tilemap.WorldToCell(world);
        return new Vector2Int(cell.x, cell.y);
    }

    
    public Vector2Int ScreenToIsoCell(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
    
        // Assume tile width and height in world units
        float tileWidth = tilemap.cellSize.x;
        float tileHeight = tilemap.cellSize.y;
    
        // Convert world X/Y to isometric coordinates
        float isoX = worldPos.x / tileWidth;
        float isoY = worldPos.y / tileHeight;
    
        // Compute grid cell using isometric math
        int cellX = Mathf.FloorToInt(isoY + isoX);
        int cellY = Mathf.FloorToInt(isoY - isoX);
    
        return new Vector2Int(cellX, cellY);
    }
}