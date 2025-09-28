using System;
using System.Collections.Generic;
using Engines.MenuEngine;
using UnityEngine;
using R3;

public class MenuGenerator : MonoBehaviour
{
    [SerializeField] private MenuBuilder menuBuilder;
    [SerializeField] private PrefabLibrary prefabLibrary;
    [SerializeField] private RandomMapGenerator mapGenerator;
    // Optional: store entries so you can reuse
    private readonly List<MenuEntry> list = new List<MenuEntry> {
        new MenuEntry("exit","Exit", MenuEntryType.Button,null, null),
        new MenuEntry("settings","Settings", MenuEntryType.Button,null, null),
        new MenuEntry("load_game","Load", MenuEntryType.Button,null, null),
        new MenuEntry("continue_game","Continue", MenuEntryType.Button,null, null),
        new MenuEntry("start_game","Start", MenuEntryType.Button,null, null),
    };
    
    // Track whether the menu is currently open
    private bool menuOpen = false;

    private CompositeDisposable disposables;

    void OnEnable()
    {
        disposables = new CompositeDisposable();
        menuBuilder.OnSelection
            .Subscribe(sel => {
                switch (true)
                {
                    case bool _ when sel.id == "start_game":
                        Debug.Log($"Selected menu id={sel.id} index={sel.index}");
                        mapGenerator.Generate();
                        ToggleMenu();
                        break;
                    case bool _ when sel.id == "continue_game":
                        Debug.Log($"Selected menu id={sel.id} index={sel.index}");
                        break;
                    case bool _ when sel.id == "load_game":
                        Debug.Log($"Selected menu id={sel.id} index={sel.index}");
                        break;
                    case bool _ when sel.id == "settings":
                        Debug.Log($"Selected menu id={sel.id} index={sel.index}");
                        break;
                    case bool _ when sel.id == "exit":
                        Debug.Log($"Selected menu id={sel.id} index={sel.index}");
                        break;
                }
            })
            .AddTo(disposables);

        // example build
        //menuBuilder.BuildMenu(list);
    }

    void OnDisable()
    {
        disposables.Dispose();
    }

    public void ToggleMenu()
    {
        if (menuOpen)
        {
            menuBuilder.ClearMenu();
            menuOpen = false;
        }
        else
        {
            menuBuilder.BuildMenu(list);
            menuOpen = true;
        }
    }
}