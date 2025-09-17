using UnityEngine;

public class TileGhost : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetVisible(bool visible)
    {
        if (meshRenderer is not null)
            meshRenderer.enabled = visible;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}