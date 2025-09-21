using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/MenuLibrary")]
public class MenuLibrary : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public string Id;
        public MenuItem Prefab;
    }

    [SerializeField] private List<Entry> entries = new();

    private Dictionary<string, MenuItem> lookup;

    public MenuItem Resolve(string id)
    {
        if (lookup == null)
        {
            lookup = new Dictionary<string, MenuItem>();
            foreach (var e in entries)
                if (!lookup.ContainsKey(e.Id)) lookup.Add(e.Id, e.Prefab);
        }

        lookup.TryGetValue(id, out var prefab);
        return prefab;
    }
}
