using System;
using System.Collections.Generic;
using Engines.MenuEngine;
using UnityEngine;
using R3;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private MenuBuilder menuBuilder;
    [SerializeField] private PrefabLibrary prefabLibrary;
    [SerializeField] private RandomMapGenerator mapGenerator;

    private readonly List<MenuEntry> rootEntries = new List<MenuEntry> {
        new MenuEntry("exit","Exit",MenuEntryType.Button,null, "exit"),
        new MenuEntry("settings","Settings",MenuEntryType.Button,null, null, null, null, new List<MenuEntry> {
            new MenuEntry("audio","Audio",MenuEntryType.Button,null,null),
            new MenuEntry("video","Video",MenuEntryType.Button,null,null),
            new MenuEntry("controls","Controls",MenuEntryType.Button,null,null),
            new MenuEntry("back","< Back",MenuEntryType.Button, null, null)
        }),
        new MenuEntry("load_game","Load",MenuEntryType.Button,null, null),
        //new MenuEntry("dd","Load",MenuEntryType.Dropdown,null, null),
        //new MenuEntry("tog","Load",MenuEntryType.Toggle,null, null),
        new MenuEntry("continue_game","Continue",MenuEntryType.Button,null, null),
        new MenuEntry("start_game","Start",MenuEntryType.Button,null, null),
    };

    private bool menuOpen = false;
    private CompositeDisposable disposables;
    private Stack<List<MenuEntry>> menuStack = new Stack<List<MenuEntry>>();

    void OnEnable()
    {
        disposables = new CompositeDisposable();
        menuBuilder.OnSelection
            .Subscribe(sel => HandleSelection(sel))
            .AddTo(disposables);
    }
    void OnDisable()
    {
        disposables.Dispose();
    }

    private void HandleSelection(MenuSelection sel)
    {
        if (sel.HasChildren)
        {
            menuStack.Push(sel.children);
            menuBuilder.BuildMenu(sel.children);
            return;
        }

        switch (sel.id)
        {
            case "start_game":
                mapGenerator.Generate();
                ToggleMenu();
                break;
            case "continue_game":
                Debug.Log("Continue game...");
                break;
            case "load_game":
                Debug.Log("Load game...");
                break;
            case "settings":
                Debug.Log("Settings opened...");
                break;
            case "exit":
                Debug.Log("Exit game...");
                break;
            case "back":
                Debug.Log("Back");
                GoBack();
                break;
                
        }
    }

    public void ToggleMenu()
    {
        if (menuOpen)
        {
            menuBuilder.ClearMenu();
            menuStack.Clear();
            menuOpen = false;
        }
        else
        {
            menuStack.Push(rootEntries);
            menuBuilder.BuildMenu(rootEntries);
            menuOpen = true;
        }
    }

    public void GoBack()
    {
        if (menuStack.Count > 1)
        {
            menuStack.Pop(); // discard current
            var previous = menuStack.Peek();
            menuBuilder.BuildMenu(previous);
        }
        else
        {
            ToggleMenu(); // close menu if at root
        }
    }
}
