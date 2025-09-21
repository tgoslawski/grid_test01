using UnityEngine.EventSystems;
using UnityEngine;
using R3;

public class TilePlacer : MonoBehaviour
{
    [SerializeField] private InputService inputService;
    [SerializeField] private GridState gridState;
    [SerializeField] private string prefabId = "floor";
    [SerializeField] private int rotation = 0;
    [SerializeField] private GameObject ghostPrefab;

    private TileGhost ghostInstance;
    private CompositeDisposable disposables;
    
    [SerializeField] private float placementInterval = 0.2f;
    private float placementTimer;
    
    private Vector3 GetMouseWorldOnGround()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Ground plane at Y = 0
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero; // fallback
    }
    
    void Start()
    {
        if (ghostPrefab != null)
        {
            GameObject ghost = Instantiate(ghostPrefab);
            ghost.transform.SetParent(this.transform); // keep hierarchy tidy
            ghostInstance = ghost.GetComponent<TileGhost>();
        }
    }

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
    
    void Update()
    {
        if (ghostInstance is null) return;
        
        if (EventSystem.current is not null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Project mouse onto ground plane at Y=0
        Vector3 worldPos = GetMouseWorldOnGround();

        // Snap to nearest cell in grid space
        Vector2Int cell = new Vector2Int(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.z)
        );

        // Update ghost position
        ghostInstance.SetPosition(new Vector3(cell.x + 0.5f, 0, cell.y + 0.5f));
        ghostInstance.SetVisible(true);
        
        // Handle LMB hold
        if (Input.GetMouseButton(0))
        {
            placementTimer -= Time.deltaTime;
            if (placementTimer <= 0f)
            {
                TryPlaceTile(cell);
                placementTimer = placementInterval;
            }
        }
        else
        {
            placementTimer = 0f; // reset timer when button released
        }
    }
    
    private void TryPlaceTile(Vector2Int cell)
    {
        if (!gridState.IsOccupied(cell))
        {
            gridState.SetCell(cell, new CellData
            {
                PrefabId = prefabId,
                Rotation = 0
            });
        }
    }

    private void OnPointerDown(PointerEvent evt)
    {
        Vector3 worldPos = GetMouseWorldOnGround();
        
        // Round to nearest cell
        Vector2Int cell = new Vector2Int(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.z) // Z because our ground plane is XZ
        );
        
        if (!gridState.IsOccupied(cell))
        {
            gridState.SetCell(cell, new CellData
            {
                PrefabId = prefabId,
                Rotation = rotation
            });
        }
    }
}