using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleStates
{
    START,
    PLAYERCHOICE,
    OPPONENTCHOICE,
    BATTLE,
    WIN,
    LOSE
}
public class BattleSystem : MonoBehaviour
{
    #region Script Variables
    private BattleStates currentState;

    [SerializeField] GameObject NextMoveContainer;

    [SerializeField] HeroUI heroUI;

    [SerializeField] List<Item> itemsToCreate;

    private HeroStats hero;
    private NPC opponent;

    private BattleHUD battleHUD;

    [SerializeField] private float typingSpeed;

    private Coroutine textCoroutine;

    private int choice;
    private int cooldown;
    private int playerSkipTurns;
    private int opponentSkipTurns;

    private int originalHealth;
    private int originalHeat;
    private int originalAttack;
    private int originalDefense;
    private float originalSpeed;

    private List<Item> generatedItems = new();

    public bool IsBattleTextFinished { get { return battleHUD.BattleText.text == string.Empty; } }
    #endregion

    #region Unity Functions
    void Start()
    {
        InitalizeBattle();
    }

    void Update()
    {
        UpdateBattleState();
    }
    #endregion

    #region Public Methods
    public void OnAttack()
    {
        if (currentState != BattleStates.PLAYERCHOICE)
            return;

        if (battleHUD.InventoryPanel.activeSelf)
        {
            battleHUD.InventoryPanel.SetActive(false);
        }

        TurnOffControls();
        StartCoroutine(PlayerAttack());
    }

    public void OnInventory()
    {
        if (currentState != BattleStates.PLAYERCHOICE)
            return;

        battleHUD.ToggleInventory();
    }

    public void UseItem(Item item)
    {
        if (item.ItemCount <= 0)
            return;

        if (item.ApplyEffect == null)
            return;

        if(generatedItems.Contains(item))
        {
            generatedItems.Remove(item);
        }

        item.ApplyEffect(hero);
        hero.inventory.RemoveItem(item);
        battleHUD.UpdateBattleUI(hero, opponent);
        LoadInventory();
        battleHUD.InventoryPanel.SetActive(false);

        StartCoroutine(TypeText(hero.heroName + " Used a " + item.ItemName + "!"));
        currentState = BattleStates.OPPONENTCHOICE;
        OpponentTurn();
    }

    public void OnSpecial()
    {
        if (currentState != BattleStates.PLAYERCHOICE)
            return;

        if (hero.currentHeat < hero.maxHeat)
        {
            battleHUD.BattleText.colorGradientPreset = hero.heroTextColor;
            StartCoroutine(TypeText("Gotta build up more heat first..."));
            battleHUD.BattleText.colorGradientPreset = null;
            return;
        }

        if (battleHUD.InventoryPanel.activeSelf)
        {
            battleHUD.InventoryPanel.SetActive(false);
        }

        TurnOffControls();
        StartCoroutine(PlayerSpecial());
    }

    public void OnRun()
    {
        if (currentState != BattleStates.PLAYERCHOICE)
            return;

        if (battleHUD.InventoryPanel.activeSelf)
        {
            battleHUD.InventoryPanel.SetActive(false);
        }

        TurnOffControls();

        if (hero.speed >= opponent.opponentSpeed)
        {
            StartCoroutine(TypeText("You got away safely!"));
            StartCoroutine(EndingToBattleSequence());
        }

        else
        {
            StartCoroutine(TypeText("You couldn't get away!"));
            currentState = BattleStates.OPPONENTCHOICE;
            OpponentTurn();
        }
    }
    #endregion

    #region Private Methods
    private string GetOpponentNextMove()
    {
        return choice == 0 ? "Attack" : "Heal";
    }

    private void InitalizeBattle()
    {
        hero = GameManager.Instance.Currenthero;
        opponent = GameManager.Instance.CurrentOpponent;
        battleHUD = FindObjectsByType<BattleHUD>(FindObjectsSortMode.None)[0];

        originalHealth = hero.maxHealth;
        originalHeat = hero.maxHeat;
        originalAttack = hero.attack;
        originalDefense = hero.defense;
        originalSpeed = hero.speed;

        battleHUD.SetBattleUI(hero, opponent);
        currentState = BattleStates.START;
        StartCoroutine(TypeText(opponent.opponentFlavorTexts[0]));
        StartCoroutine(SetupBattle());
    }

    private void UpdateBattleState()
    {
        choice = UnityEngine.Random.Range(0, 2);

        if (hero.currentHealth <= 0)
        {
            currentState = BattleStates.LOSE;
            EndBattle();
        }
        else if (opponent.currentHealth <= 0)
        {
            currentState = BattleStates.WIN;
            EndBattle();
        }
        battleHUD.UpdateBattleUI(hero, opponent);
    }

    IEnumerator SetupBattle()
    {
        TurnOffControls();
        LoadItemEffects();
        LoadInventory();

        yield return new WaitForSeconds(2f);
        if (hero.speed >= opponent.opponentSpeed)
        {
            currentState = BattleStates.PLAYERCHOICE;
            PlayerTurn();
        }
        else
        {
            currentState = BattleStates.OPPONENTCHOICE;
            OpponentTurn();
        }
    }

    IEnumerator TypeText(string text, float delayAfterText = 2f)
    {
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }
        textCoroutine = StartCoroutine(TypeTextCoroutine(text, delayAfterText));
        yield return textCoroutine;
    }

    IEnumerator TypeTextCoroutine(string text, float delayAfterText)
    {
        battleHUD.BattleText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            battleHUD.BattleText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(delayAfterText);
}

    //YandereDev-like code, don't touch
    private void LoadItemEffects()
    {
        foreach (var item in hero.inventory.items)
        {
            switch (item.ItemName)
            {
                case "Chibi-Bean© Ms.Slime Bandage":
                    item.ApplyEffect = (hero) => { hero.currentHealth += 3; };
                    break;
                case "Captain Spark's \"All-In-One\" Buffet Stew":
                    item.ApplyEffect = (hero) => { GameManager.Instance.heroParty.ForEach(hero => hero.currentHealth = hero.maxHealth); };
                    break;
                case "Chibi-Bean© \"Friends Forever!\" Care Package":
                    item.ApplyEffect = (hero) => { hero.currentHealth = hero.maxHealth; playerSkipTurns += 1; };
                    break;
                case "First-Aid Kit":
                    item.ApplyEffect = (hero) => { hero.currentHealth = hero.maxHealth / 2; };
                    break;
                case "SOMA© XJ9-Runman":
                    item.ApplyEffect = (hero) => { ApplyHeatEffect(hero, hero.maxHeat / 2, true); };
                    break;
                case "SOMA© XJ9-Runman Pro Max":
                    item.ApplyEffect = (hero) => { ApplyHeatEffect(hero, hero.maxHeat, true); };
                    break;
                case "Revitanix":
                    item.ApplyEffect = (hero) => { ApplyHeatEffect(hero, 3, false); };
                    break;
                case "Revitanix X":
                    item.ApplyEffect = (hero) => { ApplyHeatEffect(hero, 9, false); };
                break;
                case "Revitanix MAX":
                    item.ApplyEffect = (hero) => { ApplyHeatEffect(hero, 21, false); };
                break;
                case "Staminfuel":
                    item.ApplyEffect = (hero) => { ApplyHealthEffect(hero, 3, false); ApplyHeatEffect(hero, 3, false); };
                break;
                case "Staminfuel X":
                    item.ApplyEffect = (hero) => { ApplyHealthEffect(hero, 9, false); ApplyHeatEffect(hero, 9, false); };
                break;
                case "Staminfuel MAX":
                    item.ApplyEffect = (hero) => { ApplyHealthEffect(hero, 21, false); ApplyHeatEffect(hero, 21, false); };
                break;
            }
        }
    }

    private void LoadRandomItemEffects()
    {
        foreach (var item in hero.inventory.items)
        {
            switch (item.ItemName)
            {
                case "Fireworks":
                    item.ApplyEffect = (hero) => 
                    { 
                        int chance = UnityEngine.Random.Range(0, 100);
                        if(chance > 50)
                        {
                            opponentSkipTurns += 1;
                        }
                    };
                    break;
                case "Bucket of Lava":
                    item.ApplyEffect = (hero) => { opponent.currentHealth = UnityEngine.Random.Range(opponent.currentHealth / 3, opponent.currentHealth / 2); };
                    break;
                case "Minecart":
                    item.ApplyEffect = (hero) => 
                    {
                        int chance = UnityEngine.Random.Range(0, 100);
                        if (chance > 50)
                        {
                            StartCoroutine(EndingToBattleSequence());
                        }
                        else
                        {
                            opponent.currentHealth = opponent.currentHealth / 2;
                        }
                    };
                    break;
                case "TNT":
                    item.ApplyEffect = (hero) => 
                    { 
                        hero.currentHealth = Random.Range(2, hero.currentHealth / 2);
                        opponent.currentHealth = Random.Range(2, opponent.currentHealth / 2);
                    };
                    break;
                case "Potion of Healing":
                    item.ApplyEffect = (hero) => { hero.currentHealth = Random.Range(2, hero.currentHealth / 3); };
                    break;
            }
        }
    }

    private void ApplyHeatEffect(HeroStats hero, int heatAmount, bool isParty)
    {
        if (isParty)
            foreach (var currentHero in GameManager.Instance.heroParty)
            {
                currentHero.currentHeat = Mathf.Min(currentHero.maxHeat, heatAmount);
            }
        else
            hero.currentHeat = Mathf.Min(hero.maxHeat, heatAmount);
    }

    private void ApplyHealthEffect(HeroStats hero, int healthAmount, bool isParty)
    {
        if (isParty)
            foreach (var currentHero in GameManager.Instance.heroParty)
            {
                currentHero.currentHealth = Mathf.Min(currentHero.maxHealth, healthAmount);
            }
        else
            hero.currentHealth = Mathf.Min(hero.maxHealth, healthAmount);
    }

    private void LoadInventory()
    {
        for(int i = 0; i < battleHUD.InventoryButtons.Length; i++)
        {
            if (i < hero.inventory.items.Count)
            {
                var item = hero.inventory.items[i];
                if (item != null)
                {
                    battleHUD.InventoryButtons[i].GetComponent<Image>().sprite = item.ItemIcon;
                    battleHUD.InventoryButtons[i].GetComponent<Button>().interactable = true;
                    battleHUD.InventoryButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    battleHUD.InventoryButtons[i].GetComponent<Button>().onClick.AddListener(() => UseItem(item));
                }
            }
            else
            {
                battleHUD.InventoryButtons[i].GetComponent<Image>().sprite = null;
                battleHUD.InventoryButtons[i].GetComponent<Button>().interactable = false;
                battleHUD.InventoryButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }

    private void CallBuffs()
    {
        if (cooldown == 0)
        {
            if (hero.heroName == "William")
            {
                for (int i = 1; i < hero.heroSkills.Count; i++)
                {
                    hero.UseSkill(hero.heroSkills[i], opponent);
                }
            }
            else if (hero.heroName == "EnviousOrb")
            {
                SpecificSkillsUse(new int[] {0,1,2,3, 5, 7 });
            }
            else if (hero.heroName == "Dr.Creeper")
            {
                SpecifcSkillExeception(1);
            }

            cooldown = 5;
        }
    }

    private void SpecificSkillsUse(int[] skillIndices)
    {
        foreach (var index in skillIndices)
        {
            hero.UseSkill(hero.heroSkills[index], opponent);
        }
    }

    private void SpecifcSkillExeception(int index)
    {
        for (int i = 0; i < hero.heroSkills.Count; i++)
        {
            if (i == index)
                continue;
            hero.UseSkill(hero.heroSkills[i], opponent);
        }
    }

    private void GenerateRandomItem()
    {
        if (itemsToCreate.Count > 0)
        {
            var mcSkill = hero.heroSkills.Find(skill => skill.SkillName == "Mastery Craftsmanship");
            if (mcSkill != null && mcSkill.IsUnlocked)
            {
                int randomItemIndex = UnityEngine.Random.Range(1, itemsToCreate.Count-1);
                hero.inventory.AddItem(itemsToCreate[randomItemIndex], 1);
                generatedItems.Add(itemsToCreate[randomItemIndex]);
            }
            else
            {
                int randomItemIndex = UnityEngine.Random.Range(1, itemsToCreate.Count-2);
                hero.inventory.AddItem(itemsToCreate[randomItemIndex], 1);
                generatedItems.Add(itemsToCreate[randomItemIndex]);
            }
        }
    }

    void PlayerTurn()
    {
        if (playerSkipTurns > 0)
        {
            playerSkipTurns--;
            currentState = BattleStates.OPPONENTCHOICE;
            OpponentTurn();
            return;
        }

        CallBuffs();
        if (cooldown > 0)
        {
            cooldown--;
        }

        var smSkill = hero.heroSkills.Find(skill => skill.SkillName == "Scientfic Method");
        if (smSkill != null && smSkill.IsUnlocked)
        {
            NextMoveContainer.SetActive(true);
            NextMoveContainer.GetComponentInChildren<TextMeshProUGUI>().colorGradientPreset = hero.heroTextColor;
            NextMoveContainer.GetComponentInChildren<TextMeshProUGUI>().text = "Their next move is... " + GetOpponentNextMove();
        }

        var bsSkill = hero.heroSkills.Find(skill => skill.SkillName == "Built-in Brewing Station");
        if (bsSkill != null && bsSkill.IsUnlocked)
        {
            hero.inventory.AddItem(itemsToCreate[0], 1);
        }

        var cSkill = hero.heroSkills.Find(skill => skill.SkillName == "Craftsmanship");
        if (cSkill != null && cSkill.IsUnlocked)
        {
            GenerateRandomItem();
            LoadRandomItemEffects();
            LoadInventory();
        }

        GameManager.Instance.Currenthero.SaveHeroData();
        TurnOnControls();
        battleHUD.BattleText.colorGradientPreset = null;

        for (int i = 0; i < battleHUD.playerChoices.Length; i++)
        {
            battleHUD.playerChoices[i].interactable = true;
        }

        StartCoroutine(TypeText("Time to..."));
    }

    void OpponentTurn()
    {
        if (opponentSkipTurns > 0)
        {
            opponentSkipTurns--;
            currentState = BattleStates.PLAYERCHOICE;
            PlayerTurn();
            return;
        }

        battleHUD.BattleText.colorGradientPreset = null;
        NextMoveContainer.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
        TurnOffControls();
        StartCoroutine(TypeText("Opponent's turn..."));

        if (hero.speed >= opponent.opponentSpeed)
        {
            if (hero.DoesOpponentFlee(opponent))
            {
                battleHUD.BattleText.colorGradientPreset = opponent.npcTextColor;
                StartCoroutine(TypeText("Woah! That's scary! I'm outta here!"));
                StartCoroutine(EndingToBattleSequence());
            }
        }

        if (choice == 0)
        {
            StartCoroutine(OpponentAttack());
        }
        else if (choice == 1)
        {
            int healAmount = UnityEngine.Random.Range(1, opponent.maxHealth / 2);
            HealOpponent(healAmount);
        }
    }

    private void HealOpponent(int healAmount)
    {
        opponent.currentHealth += Mathf.Min(opponent.maxHealth / 2, healAmount);

        if (opponent.currentHealth > opponent.maxHealth)
        {
            opponent.currentHealth = opponent.maxHealth;
        }

        GameObject heal = Instantiate(opponent.opponentHeal, battleHUD.opponentSprite.transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(opponent.opponentHealSound);

        Destroy(heal, 0.3f);

        StartCoroutine(TypeText("Opponent healed for " + healAmount + " health!"));
        battleHUD.UpdateBattleUI(hero, opponent);
        currentState = BattleStates.PLAYERCHOICE;
        PlayerTurn();
    }

    void EndBattle()
    {
        TurnOffControls();

        if (generatedItems.Count > 0)
        {
            foreach (var item in generatedItems)
            {
                hero.inventory.RemoveItem(item);
            }
        }

        if (currentState == BattleStates.WIN)
            WinState();
        else if (currentState == BattleStates.LOSE)
            LoseState();

        heroUI.UpdateOverworldUI(hero);
    }

    private void WinState()
    {
        battleHUD.BattleText.colorGradientPreset = opponent.npcTextColor;
        StartCoroutine(TypeText(opponent.opponentFlavorTexts[1]));

        int credixEarned = UnityEngine.Random.Range(1, 100);
        hero.credixAmount += credixEarned;

        StartCoroutine(TypeText("You earned " + credixEarned + " credix!"));

        hero.AddXP(opponent.opponentLevel * 20);

        //Start the ending to battle sequence here
        battleHUD.BattleText.text = string.Empty;
        GameManager.Instance.MarkOpponentAsDefeated(opponent);
        StartCoroutine(EndingToBattleSequence());
    }

    private void LoseState()
    {
        StartCoroutine(TypeText("You lose!"));
        battleHUD.BattleText.colorGradientPreset = opponent.npcTextColor;
        StartCoroutine(TypeText(opponent.opponentFlavorTexts[2]));
        //Go to game over screen here, for now exit the editor
        battleHUD.BattleText.text = string.Empty;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    IEnumerator PlayerAttack()
    {
        //Deal damage based on factors (level and attack)

        GameObject normalAttack = Instantiate(hero.normalAttack, battleHUD.opponentSprite.transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(hero.heroNormalAttackSound);
        Destroy(normalAttack, 0.3f);

        if (hero.heroName == "William")
        {
            hero.UseSkill(hero.heroSkills[0], opponent);
        }
        else if (hero.heroName == "EnviousOrb")
        {
            hero.UseSkill(hero.heroSkills[6], opponent);
        }
        opponent.TakeDamage(hero);


        hero.currentHeat = Mathf.Min(hero.maxHeat, hero.currentHeat + 2);
        yield return new WaitForSeconds(1f);

        if (opponent.currentHealth <= 0)
        {
            currentState = BattleStates.WIN;
            GameManager.Instance.MarkOpponentAsDefeated(opponent);
            EndBattle();
        }
        else
        {
            currentState = BattleStates.OPPONENTCHOICE;
            OpponentTurn();
        }
    }

    IEnumerator PlayerSpecial()
    {
        battleHUD.BattleText.colorGradientPreset = hero.heroTextColor;
        StartCoroutine(TypeText(hero.heroSpecialAttackText));

        GameObject specialAttack = Instantiate(hero.specialAttack, battleHUD.opponentSprite.transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(hero.heroSpecialAttackSound);
        Destroy(specialAttack, 0.3f);

        hero.UseSpecialAttack(opponent);
        yield return new WaitForSeconds(2f);

        currentState = BattleStates.OPPONENTCHOICE;
        OpponentTurn();
    }

    IEnumerator OpponentAttack()
    {
        if (hero.IsEnemyConfused())
        {
            GameObject confusedOpponentAttack = Instantiate(opponent.opponentAttack, battleHUD.opponentSprite.transform.position, Quaternion.identity);
            AudioManager.Instance.PlaySFX(opponent.opponentAttackSound);
            Destroy(confusedOpponentAttack, 0.3f);

            int RandomDamage = UnityEngine.Random.Range(1, 10);
            opponent.TakeDamage(RandomDamage);

            yield return new WaitForSeconds(1f);
            currentState = BattleStates.PLAYERCHOICE;
            PlayerTurn();
        }
        else
        {
            //Deal damage based on factors (level and attack)
            GameObject opponentAttackInstance = Instantiate(opponent.opponentAttack, battleHUD.heroSprite.transform.position, Quaternion.identity);
            AudioManager.Instance.PlaySFX(opponent.opponentAttackSound);

            Destroy(opponentAttackInstance, 0.3f);

            hero.TakeDamage(opponent);
            yield return new WaitForSeconds(1f);

            //Check if the hero is dead
            if (hero.currentHealth <= 0)
            {
                currentState = BattleStates.LOSE;
                EndBattle();
            }
            else
            {
                currentState = BattleStates.PLAYERCHOICE;
                PlayerTurn();
            }
        }
    }

    void TurnOffControls()
    {
        foreach (var button in battleHUD.playerChoices)
        {
            button.interactable = false;
        }
    }

    void TurnOnControls()
    {
        foreach (var button in battleHUD.playerChoices)
        {
            button.interactable = true;
        }
    }

    private IEnumerator EndingToBattleSequence()
    {
        yield return new WaitForEndOfFrame();
        StopCoroutine(textCoroutine);
        hero.maxHealth = originalHealth;
        hero.maxHeat = originalHeat;
        hero.attack = originalAttack;
        hero.defense = originalDefense;
        hero.speed = originalSpeed;
        GameManager.Instance.CurrentOpponent = null;
        GameManager.Instance.Currenthero.SaveHeroData();
        SceneManager.LoadScene("DevScene");
    }
    #endregion
}
