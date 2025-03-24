using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Elements")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject statsMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject hatsMenu;
    public GameObject skillTreeMenu;

    [Header("Menu Buttons")]
    [SerializeField] Button settingsButton;
    [SerializeField] Button hatsButton;
    [SerializeField] Button statsButton;
    [SerializeField] Button skillTreeButton;
    [SerializeField] Button closeButton;

    private Stats stats;
    private SkillTree skillTree;
    public HeroUI heroUI;
    private readonly Stack<GameObject> menuStack = new();

    void Start()
    {
        InitalizeMenus();
        heroUI = FindFirstObjectByType<HeroUI>();
        settingsMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && menuStack.Count <= 0)
        {
            if (SceneManager.GetActiveScene().name == "ShopScene")
                return;
               
            TogglePauseMenu();
        }

        if(skillTreeMenu)
            closeButton.gameObject.SetActive(!skillTree.skillTreePathMenu.activeSelf && menuStack.Count > 0);
    }

    private void InitalizeMenus()
    {
        if (statsMenu)
            stats = statsMenu.GetComponent<Stats>();
        if (skillTreeMenu)
            skillTree = skillTreeMenu.GetComponent<SkillTree>(); 
    }

    private void TogglePauseMenu()
    {
       pauseMenu.SetActive(!pauseMenu.activeSelf);
       Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
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
        skillTree.UpdateSoulButton(GameManager.Instance.Currenthero);
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