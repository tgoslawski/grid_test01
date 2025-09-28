// MenuBuilder.cs
using System.Collections.Generic;
using UnityEngine;
using R3;
using System;
using Engines.MenuEngine;
using UnityEngine.UI;
using TMPro;

public enum MenuLayout { VerticalList, HorizontalList, Grid, CardHand, Deck }

public struct MenuSelection
{
    public readonly string id;
    public readonly int index;
    public readonly bool HasChildren;
    public readonly List<MenuEntry> children;

    public MenuSelection(string id, int index, bool hasChildren, List<MenuEntry> children)
    {
        this.id = id;
        this.index = index;
        this.HasChildren = hasChildren;
        this.children = children;
    }
}

public class MenuBuilder : MonoBehaviour
{
    [Header("Prefab + container")]
    [SerializeField] private MenuLayout layout;
    [SerializeField] private Transform contentParent;
    [SerializeField] private MenuLibrary menuLibrary;
    [SerializeField] private MenuIconLibrary menuIconLibrary;
    [SerializeField] private bool enableLabels = true;
    [SerializeField] private bool enableTooltips = false;
    [SerializeField] private bool enableIcons = false;
    [SerializeField] private int gridSpacing = 3;
    [SerializeField] private int initialPoolSize = 16;
    [SerializeField] private GameObject menuParent;

    // Pools: one stack per MenuEntryType
    private readonly Dictionary<MenuEntryType, Stack<GameObject>> pools =
        new Dictionary<MenuEntryType, Stack<GameObject>>();

    // Active items and current entries
    private readonly List<GameObject> activeItems = new List<GameObject>(32);
    private readonly List<MenuEntry> currentEntries = new List<MenuEntry>(32);

    // Events
    private readonly Subject<MenuSelection> selectionSubject = new Subject<MenuSelection>();
    public Subject<MenuSelection> OnSelection => selectionSubject;

    private CompositeDisposable disposables;

    void Awake()
    {
        ApplyLayout();
        disposables = new CompositeDisposable();

        // Prefill each pool with some items (buttons are most common)
        Prefill(MenuEntryType.Button, initialPoolSize);
    }

    void OnDestroy()
    {
        disposables?.Dispose();
        selectionSubject.OnCompleted();
    }

    // --- Pool helpers ---
    private void Prefill(MenuEntryType type, int count)
    {
        if (!pools.ContainsKey(type))
            pools[type] = new Stack<GameObject>();

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(menuLibrary.Resolve(type), transform, false);
            go.SetActive(false);
            pools[type].Push(go);
        }
    }

    private GameObject Rent(MenuEntryType type)
    {
        if (!pools.ContainsKey(type))
            pools[type] = new Stack<GameObject>();

        if (pools[type].Count > 0)
            return pools[type].Pop();

        // No available prefab in pool → instantiate a new one
        return Instantiate(menuLibrary.Resolve(type), contentParent, false);
    }

    private void Return(MenuEntryType type, GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(transform, false);
        pools[type].Push(go);
    }

    // --- Menu building ---
    public void BuildMenu(IReadOnlyList<MenuEntry> entries)
    {
        menuParent.SetActive(true);
        ClearActiveItems();

        currentEntries.Clear();
        for (int i = 0; i < entries.Count; i++)
            currentEntries.Add(entries[i]);

        for (int i = 0; i < currentEntries.Count; i++)
        {
            var e = currentEntries[i];
            var go = Rent(e.type);
            go.transform.SetParent(contentParent, false);
            go.SetActive(true);

            SetupElement(go, e, i);

            activeItems.Add(go);
        }
    }

    public void ClearMenu()
    {
        ClearActiveItems();
        currentEntries.Clear();
        menuParent.SetActive(false);
    }

    private void ClearActiveItems()
    {
        if (activeItems.Count == 0) return;

        for (int i = activeItems.Count - 1; i >= 0; i--)
        {
            var go = activeItems[i];
            activeItems.RemoveAt(i);

            var entryType = currentEntries[i].type;
            Return(entryType, go);
        }
    }

    // --- Element setup per type ---
    private void SetupElement(GameObject go, MenuEntry e, int index)
    {
        // Label
        if (enableLabels)
        {
            var label = go.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = e.label;
        }

        switch (e.type)
        {
            case MenuEntryType.Button:
                var btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                        selectionSubject.OnNext(new MenuSelection(e.id, index, e.HasChildren, e.children)));
                }
                break;

            case MenuEntryType.Slider:
                var slider = go.GetComponent<Slider>();
                if (slider != null)
                {
                    slider.minValue = 0;
                    slider.maxValue = 20;
                    slider.value = 1;
                    slider.onValueChanged.RemoveAllListeners();
                    // Resize the handle rect
                    if (slider.handleRect != null)
                    {
                        var rt = slider.handleRect.GetComponent<RectTransform>();
                        if (rt != null)
                        {
                            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 20f); // make the handle taller
                        }
                    }
                    // Resize the handle rect
                    if (slider.fillRect != null)
                    {
                        var rt = slider.fillRect.GetComponent<RectTransform>();
                        if (rt != null)
                        {
                            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 5f); // make the handle taller
                        }
                    }
                    
                }
                break;

            case MenuEntryType.Dropdown:
                var dd = go.GetComponent<TMP_Dropdown>();
                if (dd != null)
                {
                    //dd.ClearOptions();
                    //dd.AddOptions(e.dropdownOptions);
                    //dd.value = e.dropdownIndex;
                    //dd.onValueChanged.RemoveAllListeners();
                    //dd.onValueChanged.AddListener(idx => e.dropdownIndex = idx);
                }
                break;

            case MenuEntryType.Toggle:
                var toggle = go.GetComponent<Toggle>();
                if (toggle != null)
                {
                    //toggle.isOn = e.dropdownIndex == 1;
                    //toggle.onValueChanged.RemoveAllListeners();
                    //toggle.onValueChanged.AddListener(state => e.dropdownIndex = state ? 1 : 0);
                }
                break;

            case MenuEntryType.Graph:
                //var graph = go.GetComponent<GraphComponent>(); // example
                //if (graph != null)
                //{
                //    graph.BindData(e); // implement however your graph works
                //}
                break;
        }
    }

    // --- Layout ---
    private void ApplyLayout()
    {
        foreach (var comp in contentParent.GetComponents<LayoutGroup>())
            DestroyImmediate(comp);

        switch (layout)
        {
            case MenuLayout.VerticalList:
                var vlg = contentParent.gameObject.AddComponent<VerticalLayoutGroup>();
                vlg.spacing = 9f;
                vlg.padding = new RectOffset(120, 120, 120, 120);
                vlg.childAlignment = TextAnchor.MiddleCenter;
                vlg.childControlHeight = true;
                vlg.childControlWidth = true;
                LayoutRebuilder.ForceRebuildLayoutImmediate(vlg.GetComponent<RectTransform>());
                break;
            case MenuLayout.HorizontalList:
                var hlg = contentParent.gameObject.AddComponent<HorizontalLayoutGroup>();
                hlg.spacing = 9f;
                hlg.padding = new RectOffset(120, 120, 120, 120);
                hlg.childAlignment = TextAnchor.MiddleCenter;
                hlg.childControlHeight = true;
                hlg.childControlWidth = true;
                LayoutRebuilder.ForceRebuildLayoutImmediate(hlg.GetComponent<RectTransform>());
                break;
            case MenuLayout.CardHand:
                contentParent.gameObject.AddComponent<HandLayoutGroup>();
                break;
            case MenuLayout.Deck:
                var deck = contentParent.gameObject.AddComponent<HorizontalLayoutGroup>();
                deck.spacing = 10;
                deck.childAlignment = TextAnchor.MiddleCenter;
                break;
            case MenuLayout.Grid:
                var grid = contentParent.gameObject.AddComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(100, 100);
                grid.spacing = new Vector2(7f, 7f);
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = gridSpacing;
                break;
        }
    }
}
