// MenuBuilder.cs
using System.Collections.Generic;
using UnityEngine;
using R3;
using System;
using UnityEngine.UI;


public enum MenuLayout { VerticalList, HorizontalList, Grid, CardHand, Deck}
public struct MenuSelection
{
    public readonly string id;
    public readonly int index;

    public MenuSelection(string id, int index)
    {
        this.id = id;
        this.index = index;
    }
}

public class MenuBuilder : MonoBehaviour
{
    [Header("Prefab + container")]
    [SerializeField] private MenuLayout layout;
    [SerializeField] private Transform contentParent; // UI container (VerticalLayoutGroup etc.)
    [SerializeField] private MenuLibrary menuLibrary;
    [SerializeField] private MenuIconLibrary menuIconLibrary;
    [SerializeField] private bool enableLabels = true;
    [SerializeField] private bool enableTooltips = false;
    [SerializeField] private bool enableIcons = false;
    [SerializeField] private int gridSpacing = 3;
    [SerializeField] private int initialPoolSize = 16;

    // Pool internals
    private readonly Stack<MenuItem> pool = new Stack<MenuItem>();
    private readonly List<MenuItem> activeItems = new List<MenuItem>(32);

    // Data currently showing
    private readonly List<MenuEntry> currentEntries = new List<MenuEntry>(32);

    // R3 subjects
    private readonly Subject<MenuSelection> selectionSubject = new Subject<MenuSelection>();
    private readonly Subject<int> internalIndexSubject = new Subject<int>(); // index-based for pooled items

    // Expose observable to outside
    public Subject<MenuSelection> OnSelection => selectionSubject;

    private CompositeDisposable disposables;

    void Awake()
    {
   
        ApplyLayout();
        // prefill pool
        for (int i = 0; i < initialPoolSize; i++)
            pool.Push(CreateNewItem());

        // subscribe to internal index subject; map to MenuSelection and publish
        disposables = new CompositeDisposable();
        internalIndexSubject
            .Subscribe(OnIndexSelected)
            .AddTo(disposables);
    }

    void OnDestroy()
    {
        disposables?.Dispose();

        // cleanup subjects to avoid dangling subscribers
        internalIndexSubject.OnCompleted();
        selectionSubject.OnCompleted();
    }

    private MenuItem CreateNewItem()
    {
        var prefab = menuLibrary.Resolve("main_menu");
        var go = Instantiate(prefab, contentParent, false);
        go.gameObject.SetActive(false);
        return go;
    }

    // Build menu from entries - reuses pooled items
    public void BuildMenu(IReadOnlyList<MenuEntry> entries)
    {
        // reuse list, clear active items
        ClearActiveItems();

        // copy entries locally (avoid user list mutation)
        currentEntries.Clear();
        for (int i = 0; i < entries.Count; i++)
            currentEntries.Add(entries[i]);

        // allocate and initialize items
        for (int i = 0; i < currentEntries.Count; i++)
        {
            var e = currentEntries[i];
            MenuItem mi = RentItem();
            mi.gameObject.SetActive(true);

            Sprite iconSprite = null;
            if (e.iconKey is not null && enableIcons)
                iconSprite = menuIconLibrary.Resolve(e.iconKey);
            if (!enableLabels)
                e.label = null;
            if (!enableTooltips)
                e.tooltip = null;

            // Pass the shared internalIndexSubject to the MenuItem as Subject<int>
            mi.Initialize(internalIndexSubject, i, e.id, e.label, iconSprite, e.tooltip);

            activeItems.Add(mi);
        }
    }

    public void ClearMenu()
    {
        ClearActiveItems();
        currentEntries.Clear();
    }

    private void ClearActiveItems()
    {
        if (activeItems.Count == 0) return;
        for (int i = activeItems.Count - 1; i >= 0; i--)
        {
            var mi = activeItems[i];
            activeItems.RemoveAt(i);
            mi.ResetForPool();
            mi.gameObject.SetActive(false);
            mi.transform.SetParent(transform, false); // keep pool objects under builder or hidden parent
            pool.Push(mi);
        }
    }

    private MenuItem RentItem()
    {
        if (pool.Count > 0)
            return pool.Pop();

        return CreateNewItem();
    }

    private void OnIndexSelected(int index)
    {
        if (index < 0 || index >= currentEntries.Count) return;
        var e = currentEntries[index];
        selectionSubject.OnNext(new MenuSelection(e.id, index));
    }
    
    private void ApplyLayout()
    {
        foreach (var comp in contentParent.GetComponents<LayoutGroup>())
            DestroyImmediate(comp);
        
        switch (layout)
        {
            case MenuLayout.VerticalList:
                contentParent.gameObject.AddComponent<VerticalLayoutGroup>();
                break;
            case MenuLayout.HorizontalList:
                contentParent.gameObject.AddComponent<HorizontalLayoutGroup>();
                break;
            case MenuLayout.CardHand:
                contentParent.gameObject.AddComponent<HandLayoutGroup>();
                break;
            case MenuLayout.Deck:
                var hlg = contentParent.gameObject.AddComponent<HorizontalLayoutGroup>();
                hlg.spacing = 10; // adjust card spacing
                hlg.childAlignment = TextAnchor.MiddleCenter;
                break;
            case MenuLayout.Grid:
                var grid = contentParent.gameObject.AddComponent<GridLayoutGroup>();
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = gridSpacing;
                break;
        }
    }
}
