using System;
using System.Collections.Generic;
using UnityEngine;
using R3;

[Serializable]
public class CellData
{
    public string PrefabId;
    public int Rotation;
    public int Height = 0;
}

public struct GridChange
{
    public Vector2Int Cell;
    public CellData Data;
    public GridChange(Vector2Int cell, CellData data)
    {
        Cell = cell;
        Data = data;
    }
}

[CreateAssetMenu(menuName = "Grid/GridState")]
public class GridState : ScriptableObject
{
    private readonly Subject<GridChange> _changes = new Subject<GridChange>();
    public Subject<GridChange> Changes => _changes;

    private readonly Dictionary<Vector2Int, CellData> _cells = new Dictionary<Vector2Int, CellData>();

    public bool IsOccupied(Vector2Int cell) => _cells.ContainsKey(cell);
    public CellData GetCell(Vector2Int cell) { _cells.TryGetValue(cell, out var d); return d; }

    public void SetCell(Vector2Int cell, CellData data)
    {
        _cells[cell] = data;
        _changes.OnNext(new GridChange(cell, data));
    }

    public void ClearCell(Vector2Int cell)
    {
        if (_cells.Remove(cell))
            _changes.OnNext(new GridChange(cell, null));
    }

    /// <summary>
    /// Enumerate all cells currently in the grid.
    /// </summary>
    public IEnumerable<KeyValuePair<Vector2Int, CellData>> AllCells()
    {
        return _cells;
    }

    /// <summary>
    /// Clear the entire grid.
    /// </summary>
    public void ClearAll()
    {
        var toRemove = new List<Vector2Int>(_cells.Keys);
        _cells.Clear();
        foreach (var cell in toRemove)
        {
            _changes.OnNext(new GridChange(cell, null));
        }
    }
}