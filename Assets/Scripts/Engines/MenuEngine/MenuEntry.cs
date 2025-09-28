// MenuEntry.cs
using System;
using System.Collections.Generic;
using Engines.MenuEngine;
using UnityEngine;

[Serializable]
public struct MenuEntry
{
    public string id;
    public string label;
    public MenuEntryType type; 
    public string iconKey;
    public string tooltip;
    public Action onSelect;
    public Action onBack;
    public List<MenuEntry> children;
    
    //public float sliderMin;
    //public float sliderMax;
    //public float sliderValue;
    //
    //public List<string> dropdownOptions;
    //public int dropdownIndex;
    public MenuEntry(string id,
                     string label,
                     MenuEntryType type,
                     string iconKey = null,
                     string tooltip = null,
                     Action onSelect = null,
                     Action onBack = null,
                     List<MenuEntry> children = null)
    {
        this.id = id;
        this.label = label;
        this.type = type;
        this.iconKey = iconKey;
        this.tooltip = tooltip;
        this.onSelect = onSelect;
        this.onBack = onBack;
        this.children = children ?? new List<MenuEntry>();
    }
    
    public bool HasChildren => children != null && children.Count > 0;
}