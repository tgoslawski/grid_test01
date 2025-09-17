using UnityEngine;

public class RandomMapGenerator : MonoBehaviour
{
    [SerializeField] private GridState gridState;            // holds placed tiles
    [SerializeField] private GridRenderer gridRenderer;      // renders tiles
    [SerializeField] private PrefabLibrary prefabLibrary;    // same one used in TilePlacer
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private int seed = 0;

    [Header("Noise Settings")]
    [SerializeField, Range(0f, 1f)] private float waterThreshold = 0.3f;
    [SerializeField, Range(0f, 1f)] private float hillThreshold = 0.7f;
    [SerializeField] private float noiseScale = 0.1f;

    private System.Random rng;

    public void Generate()
    {
        if (gridState == null || gridRenderer == null || prefabLibrary == null)
        {
            Debug.LogError("RandomMapGenerator not configured properly.");
            return;
        }

        rng = seed == 0 ? new System.Random() : new System.Random(seed);

        // Clear existing tiles
        //gridRenderer.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Use Perlin noise for smoother maps
                float noiseValue = Mathf.PerlinNoise((x + seed) * noiseScale, (y + seed) * noiseScale);

                string prefabId;
                if (noiseValue < waterThreshold)
                    prefabId = "water";
                else if (noiseValue > hillThreshold)
                    prefabId = "mountain";
                else
                    prefabId = "grass";

                Vector2Int cell = new Vector2Int(x, y);

                // Update state
                gridState.SetCell(cell, new CellData
                {
                    PrefabId = prefabId,
                    Rotation = 0
                });

                // Render
                var prefab = prefabLibrary.Resolve(prefabId);
                if (prefab != null)
                    gridState.SetCell(cell, new CellData
                    {
                        PrefabId = prefabId,
                        Rotation = 0
                    });
            }
        }
    }
}