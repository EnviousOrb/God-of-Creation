using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject statsMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject hatsMenu;
    [SerializeField] public GameObject skillTreeMenu;
    [SerializeField] Button settingsButton;
    [SerializeField] Button hatsButton;
    [SerializeField] Button statsButton;
    [SerializeField] Button skillTreeButton;
    [SerializeField] Button closeButton;
    private Stats stats;
    private SkillTree skillTree;
    public HeroUI heroUI;
    private readonly Stack<GameObject> menuStack = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = statsMenu.GetComponent<Stats>();
        skillTree = skillTreeMenu.GetComponent<SkillTree>();
        heroUI = FindFirstObjectByType<HeroUI>();
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

        if (skillTree.skillTreePathMenu.activeSelf)
            closeButton.gameObject.SetActive(false);
        else
            closeButton.gameObject.SetActive(menuStack.Count > 0);
    }

    public void OpenMenu(GameObject menu)
    {
        if(menuStack.Count > 0) 
           menuStack.Peek().SetActive(false); // If there are menus in the stack, hide the top menu

        menuStack.Push(menu); // Add the new menu to the stack
        menu.SetActive(true); // Show the new menu
        closeButton.gameObject.SetActive(true); // Show the close button
    }

    public void CloseMenu()
    {
        if (menuStack.Count > 0) // If there are menus in the stack
        {
            menuStack.Pop().SetActive(false);
            if (menuStack.Count > 0) // If there are still menus in the stack after popping
                menuStack.Peek().SetActive(true);

            closeButton.gameObject.SetActive(menuStack.Count > 0); // If there are still menus in the stack after popping, show the close button
        }
    }

    public void OnStatsButton()
    {
        // Open the stats menu and set the stats to be that of the current hero at play
        OpenMenu(statsMenu);
        stats.SetStats(GameManager.Instance.Currenthero);
    }

    public void OnSkillTreeButton()
    {
        // Open the skill tree menu and set the skill tree to be that of the current hero at play
        OpenMenu(skillTreeMenu);
        skillTree.DisplaySkillTreeName(GameManager.Instance.Currenthero);
        skillTree.DisplaySkillIcons(GameManager.Instance.Currenthero);
    }

    public void OnSettingsButton()
    {
        // Open the settings menu
        OpenMenu(settingsMenu);
    }

    public void OnHatsButton()
    {
        // Open the hats menu
        OpenMenu(hatsMenu);
    }
} 