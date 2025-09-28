using System.Collections.Generic;
using UnityEngine;
using R3;

public class CardMenu : MonoBehaviour
{
    [SerializeField] private MenuBuilder menuBuilder;
    [SerializeField] private PrefabLibrary prefabLibrary;
    // Optional: store entries so you can reuse
    private readonly List<MenuEntry> list = new List<MenuEntry> {
        new MenuEntry { id = "attack", label = "Attack", tooltip = null, iconKey = null, children = null},
        new MenuEntry { id = "defend", label = "Defend", tooltip = null, iconKey = null, children = null},
        new MenuEntry { id = "heal", label = "Heal", tooltip = null, iconKey = null, children = null}
    };
    
    // Track whether the menu is currently open
    private bool menuOpen = false;

    private CompositeDisposable disposables;

    void OnEnable()
    {
        disposables = new CompositeDisposable();
        menuBuilder.OnSelection
            .Subscribe(sel => {
                Debug.Log($"Selected menu id={sel.id} index={sel.index}");
                // handle selection (no heavy allocations here)
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