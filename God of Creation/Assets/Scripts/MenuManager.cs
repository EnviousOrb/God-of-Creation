using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject StatsMenu;
    [SerializeField] GameObject SettingsMenu;
    [SerializeField] GameObject HatsMenu;
    [SerializeField] Button settingsButton;
    [SerializeField] Button HatsButton;
    [SerializeField] Button StatsButton;
    [SerializeField] Button CloseButton;
    private Stats stats;
    private readonly Stack<GameObject> menuStack = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = StatsMenu.GetComponent<Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && menuStack.Count <= 0)
        {
            if (pauseMenu.activeSelf)
            {
                // If the pause menu is active, deactivate it and set the time scale to 1
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                // If the pause menu is not active, activate it and set the time scale to 0
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
        }

        StatsButton.onClick.AddListener(() =>
        {
            // Open the stats menu
            OpenMenu(StatsMenu);
            stats.SetStats(GameManager.Instance.Currenthero);
        });

        settingsButton.onClick.AddListener(() =>
        {
            // Open the settings menu
            OpenMenu(SettingsMenu);
        });

        HatsButton.onClick.AddListener(() =>
        {
            // Open the hats menu
            OpenMenu(HatsMenu);
        });

        CloseButton.onClick.AddListener(() =>
        {
            // Close the current menu
            CloseMenu();
        });
    }

    public void OpenMenu(GameObject menu)
    {
        if(menuStack.Count > 0) 
           menuStack.Peek().SetActive(false); // If there are menus in the stack, hide the top menu

        menuStack.Push(menu); // Add the new menu to the stack
        menu.SetActive(true); // Show the new menu
        CloseButton.gameObject.SetActive(true); // Show the close button
    }

    public void CloseMenu()
    {
        if (menuStack.Count > 0) // If there are menus in the stack
        {
            menuStack.Pop().SetActive(false);
            if (menuStack.Count > 0) // If there are still menus in the stack after popping
                menuStack.Peek().SetActive(true);

            CloseButton.gameObject.SetActive(menuStack.Count > 0); // If there are still menus in the stack after popping, show the close button
        }
    }
}