using System;
using System.Collections.Generic;
using Engines.MenuEngine;
using UnityEngine;

[Serializable]
public class PrefabEntry
{
    public string key;
    public GameObject prefab;
}

[CreateAssetMenu(menuName = "Menu/MenuLibrary")]
public class MenuLibrary : ScriptableObject
{
    [SerializeField] private List<PrefabEntry> entries = new List<PrefabEntry>();

    private Dictionary<string, GameObject> prefabDict;

    private void OnEnable()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        prefabDict = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
        foreach (var e in entries)
        {
            if (e == null || string.IsNullOrEmpty(e.key) || e.prefab == null)
                continue;

            if (!prefabDict.ContainsKey(e.key))
                prefabDict.Add(e.key, e.prefab);
        }
    }

    public GameObject Resolve(string key)
    {
        if (prefabDict == null || prefabDict.Count == 0)
            BuildDictionary();

        prefabDict.TryGetValue(key, out var prefab);
        return prefab;
    }

    public GameObject Resolve(MenuEntryType type)
    {
        return Resolve(KeyForType(type));
    }

    private string KeyForType(MenuEntryType type)
    {
        switch (type)
        {
            case MenuEntryType.Button: return "button";
            case MenuEntryType.Slider: return "slider";
            case MenuEntryType.Dropdown: return "dropdown";
            case MenuEntryType.Toggle: return "toggle";
            case MenuEntryType.Graph: return "graph";
            default: return "button";
        }
    }
}