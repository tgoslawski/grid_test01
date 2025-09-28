using UnityEngine;
using TMPro; // if you use TextMeshPro

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private RectTransform background;   // panel image
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private Vector2 padding = new Vector2(8, 4);

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private MenuItem currentSource;
    private RectTransform boundary;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        Hide();
    }

    public void Show(string message, MenuItem source = null, RectTransform boundary = null)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        tooltipText.text = message;
        currentSource = source;
        this.boundary = boundary;

        // Resize background
        Vector2 textSize = tooltipText.GetPreferredValues(message);
        background.sizeDelta = textSize + padding;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
            out localPos
        );

        RectTransform targetBounds = boundary is not null ? boundary : parentCanvas.transform as RectTransform;

        // Tooltip’s half size
        float halfW = rectTransform.rect.width / 2f;
        float halfH = rectTransform.rect.height / 2f;

        // Get target bounds
        float minX = targetBounds.rect.xMin + halfW;
        float maxX = targetBounds.rect.xMax - halfW;
        float minY = targetBounds.rect.yMin + halfH;
        float maxY = targetBounds.rect.yMax - halfH;

        // Clamp inside the chosen bounds
        float clampedX = Mathf.Clamp(targetBounds.rect.xMin, minX, maxX);
        float clampedY = Mathf.Clamp(targetBounds.rect.yMin , minY, maxY);

        rectTransform.anchoredPosition = new Vector2(clampedX, clampedY);
    }
    
    private Vector2 ClampToParentBounds(Vector2 desiredPos)
    {
        var parentRect = parentCanvas.transform as RectTransform;

        // Get half sizes
        float halfWidth = rectTransform.rect.width / 2f;
        float halfHeight = rectTransform.rect.height / 2f;

        // Get parent bounds in local space
        float minX = parentRect.rect.xMin + halfWidth;
        float maxX = parentRect.rect.xMax - halfWidth;
        float minY = parentRect.rect.yMin + halfHeight;
        float maxY = parentRect.rect.yMax - halfHeight;

        // Clamp
        float clampedX = Mathf.Clamp(desiredPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(desiredPos.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }
    
    public MenuItem GetSource()
    {
        return currentSource;
    }
}