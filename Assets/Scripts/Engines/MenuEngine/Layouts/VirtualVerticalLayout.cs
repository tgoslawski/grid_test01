using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class VirtualVerticalLayout : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private float itemHeight = 40f;
    [SerializeField] private int buffer = 2;

    private int totalCount;
    private Func<int, GameObject> itemProvider;
    private readonly Dictionary<int, GameObject> active = new();
    private readonly Stack<GameObject> pool = new();
    private int firstVisibleIndex = -1;
    
    private void Awake()
    {
        if (content == null)
            content = GetComponent<RectTransform>();

        if (viewport == null && transform.parent != null)
            viewport = transform.parent.GetComponent<RectTransform>();
    }

    public void Initialize(int count, Func<int, GameObject> provider)
    {
        totalCount = count;
        itemProvider = provider;
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalCount * itemHeight);
        Refresh();
    }

    private void Update() => Refresh();

    private void Refresh()
    {
        if (itemProvider == null || viewport == null) return;

        float scrollY = content.anchoredPosition.y;
        int newFirstIndex = Mathf.Max(0, Mathf.FloorToInt(scrollY / itemHeight) - buffer);

        if (newFirstIndex == firstVisibleIndex) return; // no change

        firstVisibleIndex = newFirstIndex;

        // return all to pool
        foreach (var kv in active)
        {
            kv.Value.SetActive(false);
            pool.Push(kv.Value);
        }
        active.Clear();

        int visibleCount = Mathf.CeilToInt(viewport.rect.height / itemHeight) + buffer * 2;
        for (int i = 0; i < visibleCount && (firstVisibleIndex + i) < totalCount; i++)
        {
            int dataIndex = firstVisibleIndex + i;
            GameObject go = pool.Count > 0 ? pool.Pop() : itemProvider(dataIndex);
            go.transform.SetParent(content, false);
            go.SetActive(true);

            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(Mathf.Abs(viewport.rect.x), -dataIndex * itemHeight);

            active[dataIndex] = go;
        }
    }
}

