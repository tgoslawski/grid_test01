// MenuEntry.cs
using System;
[Serializable]
public struct MenuEntry
{
    public string id;      // stable id (useful for saves)
    public string label;   // visible text
    public MenuEntry(string id, string label)
    {
        this.id = id;
        this.label = label;
    }
}