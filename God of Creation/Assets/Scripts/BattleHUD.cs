using UnityEngine;
using TMPro;
using UnityEngine.UI;

    public class BattleHUD : MonoBehaviour
    {
        [Header("Battle UI")]
         public TextMeshProUGUI BattleText; //The location that the battle text will be displayed on the main UI
         public Slider heroHealth; //The location that the hero's health will be displayed on the main UI
         public Slider heroHeat; //The location that the hero's heat will be displayed on the main UI
         public Slider opponentHealth; //The location that the opponent's health will be displayed on the main UI
         public Button[] playerChoices; //The location that the player's choices will be displayed on the main UI

        [Header("Battle Info")]
         public TextMeshProUGUI heroName; //The location that the hero's name will be displayed on the main UI
         public TextMeshProUGUI opponentName; //The location that the opponent's name will be displayed on the main UI
         public TextMeshProUGUI heroLevel; //The location that the hero's level will be displayed on the main UI
         public TextMeshProUGUI opponentLevel; //The location that the opponent's level will be displayed on the main UI

        [Header("Battle Icons")]
        [SerializeField] public Image heroSprite; //The location that the hero's sprite will be displayed on the main UI
        [SerializeField] public Image opponentSprite; //The location that the opponent's sprite will be displayed on the main UI

        [Header("Inventory UI")]
        [SerializeField] private GameObject InventoryPanel; //The location that the inventory panel will be displayed on the main UI
        [SerializeField] public Button[] InventoryButtons; //The location that the inventory buttons will be displayed on the main UI
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

        public void UpdateInventoryUI(Inventory inventory)
        {
            for (int i = 0; i < InventoryButtons.Length; i++)
            {
                if (i < inventory.items.Count)
                {
                    var item = inventory.items[i];
                    InventoryButtons[i].gameObject.SetActive(true);
                    InventoryButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{item.ItemName} x{item.ItemCount}";
                }
                else
                {
                    InventoryButtons[i].gameObject.SetActive(false);
                }
            }
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