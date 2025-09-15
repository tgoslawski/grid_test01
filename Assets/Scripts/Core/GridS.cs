using UnityEngine;

public class GridStateTester : MonoBehaviour
{
    public GridState gridState;

    void Start()
    {
        gridState.SetCell(Vector2Int.zero, new CellData { PrefabId = "floor", Rotation = 0 });
    }
}