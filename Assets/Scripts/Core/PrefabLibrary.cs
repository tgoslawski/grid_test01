using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grid/PrefabLibrary")]
public class PrefabLibrary : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public string Id;
        public GameObject Prefab;
    }

    [SerializeField] private List<Entry> entries = new();

    private Dictionary<string, GameObject> lookup;

    public GameObject Resolve(string id)
    {
        if (lookup == null)
        {
            lookup = new Dictionary<string, GameObject>();
            foreach (var e in entries)
                if (!lookup.ContainsKey(e.Id)) lookup.Add(e.Id, e.Prefab);
        }

        lookup.TryGetValue(id, out var prefab);
        return prefab;
    }
}
