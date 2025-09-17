using System.Collections.Generic;
using UnityEngine;
using R3;

public class GridRenderer : MonoBehaviour
{
    [SerializeField] private GridTilemapAdapter gridAdapter;
    [SerializeField] private GridState gridState;
    [SerializeField] private PrefabLibrary prefabLibrary;

    private readonly Dictionary<Vector2Int, GameObject> spawned = new();
    private CompositeDisposable disposables;

    void OnEnable()
    {
        if (gridState == null) return;

        disposables = new CompositeDisposable();

        // Subscribe once
        gridState.Changes.Subscribe(OnGridChange).AddTo(disposables);

        // Render existing cells
        foreach (var kv in gridState.AllCells())
            Place(kv.Key, kv.Value);
    }

    void OnDisable()
    {
        // Dispose all subscriptions
        disposables?.Dispose();

        // Destroy all spawned tiles
        foreach (var go in spawned.Values)
            if (go != null) Destroy(go);

        spawned.Clear();
    }

    private void OnGridChange(GridChange change)
    {
        if (change.Data == null)
            Remove(change.Cell);
        else
            Place(change.Cell, change.Data);
    }

    public void Place(Vector2Int cell, CellData data)
    {
        if (prefabLibrary == null) return;
        Remove(cell);

        var prefab = prefabLibrary.Resolve(data.PrefabId);
        if (prefab == null) return;

        var worldPos = gridAdapter.CellToWorld(cell);
        var go = Instantiate(prefab, worldPos, Quaternion.Euler(0, 0, data.Rotation), transform);
        spawned[cell] = go;
    }

    public void Remove(Vector2Int cell)
    {
        if (spawned.TryGetValue(cell, out var go))
        {
            if (go != null) Destroy(go);
            spawned.Remove(cell);
        }
    }
}
