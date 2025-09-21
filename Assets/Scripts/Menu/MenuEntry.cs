// MenuEntry.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MenuEntry
{
    public string id;
    public string label;
    public string iconKey;
    public string tooltip;
    public List<MenuEntry> children;
    public MenuEntry(string id, string label, string iconKey = null, string tooltip = null,  List<MenuEntry> children = null)
    {
        this.id = id;
        this.label = label;
        this.iconKey = iconKey;
        this.tooltip = tooltip;
        this.children = children;

    }
}