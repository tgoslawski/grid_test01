// MenuItem.cs
using UnityEngine;
using UnityEngine.UI;
using R3;
using TMPro;

[RequireComponent(typeof(Button))]
public class MenuItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText; // or TextMeshProUGUI if you prefer
    // If you use TMP, change type above and SetLabel accordingly.

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
    public void Initialize(Subject<int> selectionSubject, int index, string id, string label)
    {
        this.selectionSubject = selectionSubject;
        cachedIndex = index;
        cachedId = id;
        SetLabel(label);

        // Ensure button listener is set only once per instance to avoid duplicate listeners
        var btn = GetComponent<Button>();
        btn.onClick.RemoveListener(clickAction);
        btn.onClick.AddListener(clickAction);
    }

    private void OnClickInternal()
    {
        // Publish index to pool-level subject; avoid allocating a wrapper object here
        if (selectionSubject != null)
        {
            selectionSubject.OnNext(cachedIndex);
        }
    }

    public string GetId() => cachedId;

    public void SetLabel(string text)
    {
        if (labelText != null) labelText.text = text;
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
