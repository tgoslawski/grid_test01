// MenuItem.cs
using UnityEngine;
using UnityEngine.UI;
using R3;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class MenuItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Image icon;
    [SerializeField] private TooltipUI tooltipUI;
    
    private string tooltip;

    // Index & id for this item
    private int cachedIndex;
    private string cachedId;

    // A subject reference to publish selection (set by MenuBuilder when creating / pooling)
    private Subject<int> selectionSubject;

    // Cache UnityAction to avoid closure allocation per assignment
    private UnityEngine.Events.UnityAction clickAction;

    void Awake()
    {
        // Create a bound action once; it will use current cachedIndex when invoked.
        clickAction = OnClickInternal;
    }

    // called by MenuBuilder to initialize or recycle
    public void Initialize(Subject<int> selectionSubject, int index, string id, string label, Sprite iconSprite = null, string tooltip = null)
    {
        this.selectionSubject = selectionSubject;
        cachedIndex = index;
        cachedId = id;
        SetLabel(label);
        SetIcon(iconSprite);
        SetTooltip(tooltip);

        // Ensure button listener is set only once per instance to avoid duplicate listeners
        var btn = GetComponent<Button>();
        btn.onClick.RemoveListener(clickAction);
        btn.onClick.AddListener(clickAction);
        // hook events
        var trigger = btn.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entry.callback.AddListener((_) => ShowTooltip());
        trigger.triggers.Add(entry);

        var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener((_) => HideTooltip());
        trigger.triggers.Add(exit);
    }

    private void OnClickInternal()
    {
        // Publish index to pool-level subject; avoid allocating a wrapper object here
        if (selectionSubject is not null)
        {
            selectionSubject.OnNext(cachedIndex);
        }
    }

    public string GetId() => cachedId;

    public void SetLabel(string text)
    {
        if (labelText is not null) labelText.text = text;
    }
    
    public void SetIcon(Sprite iconSprite)
    {
        if (iconSprite is not null)
        {
            icon.sprite = iconSprite;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }

    }
    
    public void SetTooltip(string tooltip)
    {
        if (tooltip is not null) this.tooltip = tooltip;
    }
    
    private void ShowTooltip()
    {
        if (!string.IsNullOrEmpty(tooltip))
        {
            RectTransform parentBounds = transform.parent as RectTransform;
            tooltipUI.Show(tooltip, this, parentBounds);
        }

        
    }

    private void HideTooltip()
    {
        tooltipUI.Hide();
    }

    // Called by pool when deactivating item
    public void ResetForPool()
    {
        var btn = GetComponent<Button>();
        btn.onClick.RemoveListener(clickAction);
        cachedId = null;
        // optionally clear labelText.text = string.Empty; (assigning string empty is tiny alloc)
    }
}
