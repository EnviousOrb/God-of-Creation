using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [Header("Battle UI")]
     public TextMeshProUGUI BattleText;
     public Slider heroHealth;
     public Slider heroHeat;
     public Slider opponentHealth;
     public Button[] playerChoices;

    [Header("Battle Info")]
    public TextMeshProUGUI heroName;
     public TextMeshProUGUI opponentName;
    public TextMeshProUGUI heroLevel;
     public TextMeshProUGUI opponentLevel;

    [Header("Battle Icons")]
    public Image heroSprite;
    public Image opponentSprite;

    [Header("Inventory UI")]
    public GameObject InventoryPanel;
    public GameObject[] InventoryButtons;
    private bool isActive;

    public void SetBattleUI(HeroStats heroStats, NPC opponent)
    {
        heroName.text = heroStats.heroName;
        heroHealth.maxValue = heroStats.maxHealth;
        heroHeat.maxValue = heroStats.maxHeat;
        heroLevel.text = heroStats.Level.ToString();
        heroSprite.sprite = heroStats.battleIcon;

        opponentName.text = opponent.npcName;
        opponentHealth.maxValue = opponent.maxHealth;
        opponent.currentHealth = opponent.maxHealth;
        opponentLevel.text = opponent.opponentLevel.ToString();
        opponentSprite.sprite = opponent.opponentIcon;

        UpdateBattleUI(heroStats, opponent);
    }

    public void UpdateBattleUI(HeroStats heroStats, NPC opponent)
    {
        heroHealth.value = heroStats.currentHealth;
        heroHeat.value = heroStats.currentHeat;
        opponentHealth.value = opponent.currentHealth;
    }

    public void Start()
    {
        InventoryPanel.SetActive(false);
    }

    public void ToggleInventory()
    {
        isActive = !isActive;
        InventoryPanel.SetActive(isActive);
    }
}