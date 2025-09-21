using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Menu Icon Library")]
public class MenuIconLibrary : ScriptableObject
{
    [System.Serializable] public struct Entry { public string key; public Sprite sprite; }
    public Entry[] icons;

    private Dictionary<string, Sprite> lookup;

    void OnEnable()
    {
        lookup = new Dictionary<string, Sprite>();
        foreach (var e in icons) lookup[e.key] = e.sprite;
    }

    public Sprite Resolve(string key)
    {
        return lookup.TryGetValue(key, out var sprite) ? sprite : null;
    }
}
