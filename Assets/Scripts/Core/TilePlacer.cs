using UnityEngine;
using R3;

public class TilePlacer : MonoBehaviour
{
    [SerializeField] private InputService inputService;
    [SerializeField] private GridState gridState;
    [SerializeField] private string prefabId = "floor";
    [SerializeField] private int rotation = 0;

    private CompositeDisposable disposables;

    void OnEnable()
    {
        disposables = new CompositeDisposable();
        // Subscribe to the R3 Subject
        inputService.PointerDown.Subscribe(OnPointerDown).AddTo(disposables);
    }

    void OnDisable()
    {
        // Dispose subscription
        disposables.Dispose();
    }

    private void OnPointerDown(PointerEvent evt)
    {
        if (!gridState.IsOccupied(evt.Cell))
        {
            gridState.SetCell(evt.Cell, new CellData
            {
                PrefabId = prefabId,
                Rotation = rotation
            });
        }
    }
}