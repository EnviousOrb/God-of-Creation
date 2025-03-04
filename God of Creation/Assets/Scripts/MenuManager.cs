using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject statsMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject hatsMenu;
    [SerializeField] GameObject skillTreeMenu;
    [SerializeField] Button settingsButton;
    [SerializeField] Button hatsButton;
    [SerializeField] Button statsButton;
    [SerializeField] Button skillTreeButton;
    [SerializeField] Button closeButton;
    private Stats stats;
    private SkillTree skillTree;
    private readonly Stack<GameObject> menuStack = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = statsMenu.GetComponent<Stats>();
        skillTree = skillTreeMenu.GetComponent<SkillTree>();
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

        statsButton.onClick.AddListener(() =>
        {
            // Open the stats menu and set the stats to be that of the current hero at play
            OpenMenu(statsMenu);
            stats.SetStats(GameManager.Instance.Currenthero);
        });

        settingsButton.onClick.AddListener(() =>
        {
            // Open the settings menu
            OpenMenu(settingsMenu);
        });

        hatsButton.onClick.AddListener(() =>
        {
            // Open the hats menu
            OpenMenu(hatsMenu);
        });

        skillTreeButton.onClick.AddListener(() =>
        {
            // Open the skill tree menu
            OpenMenu(skillTreeMenu);
            skillTree.DisplaySkillTreeName(GameManager.Instance.Currenthero);
            skillTree.DisplaySkillIcons(GameManager.Instance.Currenthero);
        });

        closeButton.onClick.AddListener(() =>
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
} 