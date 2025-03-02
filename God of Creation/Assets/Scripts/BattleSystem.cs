using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    private BattleStates currentState;

    [SerializeField] HeroUI heroUI;

    private HeroStats hero; //The hero's stats
    private NPC opponent; //The opponent's stats

    private BattleHUD battleHUD; // Reference to the BattleHUD script

    [SerializeField] private float typingSpeed; //The speed at which the battle text will be displayed on screen

    private Coroutine textCoroutine;

    public bool isBattleTextFinished { get { return battleHUD.BattleText.text == string.Empty; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hero = GameManager.Instance.Currenthero;
        opponent = GameManager.Instance.CurrentOpponent;
        battleHUD = FindObjectsByType<BattleHUD>(FindObjectsSortMode.None)[0];

        battleHUD.SetBattleUI(hero, opponent);
        currentState = BattleStates.START;
        StartCoroutine(TypeText(opponent.opponentFlavorTexts[0]));
        StartCoroutine(SetupBattle());
    }

    // Update is called once per frame
    void Update()
    {
        if(hero.currentHealth <= 0)
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

        yield return new WaitForSeconds(2f);
        if(hero.speed >= opponent.opponentSpeed)
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
        //If there is already text being displayed, stop it and display the new text
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }
        textCoroutine = StartCoroutine(TypeTextCoroutine(text, delayAfterText));
        yield return textCoroutine;
    }

    IEnumerator TypeTextCoroutine(string text, float delayAfterText)
    {
        //Clear the text and then display it letter by letter
        battleHUD.BattleText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            battleHUD.BattleText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(delayAfterText);
    }

    void PlayerTurn()
    {
        //Save the hero's data before the player makes a choice
        GameManager.Instance.Currenthero.SaveHeroData();

        //Enable the player's choices and display the text
        TurnOnControls();

        battleHUD.BattleText.colorGradientPreset = null;

        for(int i = 0; i < battleHUD.playerChoices.Length; i++)
        {
            battleHUD.playerChoices[i].interactable = true;
        }

        StartCoroutine(TypeText("Time to..."));
    }

    void OpponentTurn()
    {
        battleHUD.BattleText.colorGradientPreset = null;

        TurnOffControls();

        StartCoroutine(TypeText("Opponent's turn..."));

        //Figure out a choice depending on health and likelyhood of winning
        //If your health is low, then defend or heal, depends on hidden coin toss mechanic
        //If health is high, then attack
        //If the hero's special was just used, then block for a turn
        //For now, just attack or heal randomly
        int choice = UnityEngine.Random.Range(0, 2);
        if(choice == 0)
        {
            StartCoroutine(OpponentAttack());
        }
        else if (choice == 1)
        {
            //Heal the opponent a random amount
            int healAmount = UnityEngine.Random.Range(1, opponent.maxHealth / 2);
            opponent.currentHealth += Mathf.Min(opponent.maxHealth / 2, healAmount);
            if(opponent.currentHealth > opponent.maxHealth)
            {
                opponent.currentHealth = opponent.maxHealth;
            }

            GameObject heal = Instantiate(opponent.opponentHeal, battleHUD.opponentSprite.transform.position, Quaternion.identity);
            Destroy(heal, 0.3f);

            //Display the amount healed
            StartCoroutine(TypeText("Opponent healed for " + healAmount + " health!"));

            //Update the UI
            battleHUD.UpdateBattleUI(hero, opponent);
            currentState = BattleStates.PLAYERCHOICE;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        TurnOffControls();

        if(currentState == BattleStates.WIN)
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
        else if(currentState == BattleStates.LOSE)
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
    }

    IEnumerator PlayerAttack()
    {
        //Deal damage based on factors (level and attack)
        GameObject normalAttack = Instantiate(hero.normalAttack, battleHUD.opponentSprite.transform.position, Quaternion.identity);
        Destroy(normalAttack, 0.3f);
        opponent.TakeDamage(hero);
        hero.currentHeat += 2;
        yield return new WaitForSeconds(1f);

        //Check if the opponent is dead
        if(opponent.currentHealth <= 0)
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

    IEnumerator PlayerInventory()
    {
        //Open inventory...is what I would say if I had one, for now, heal the hero a random amount
        int healAmount = UnityEngine.Random.Range(1, hero.maxHealth);
        hero.currentHealth += Mathf.Min(hero.maxHealth, healAmount);
        if(hero.currentHealth > hero.maxHealth)
        {
            hero.currentHealth = hero.maxHealth;
        }

        //Display the amount healed
        StartCoroutine(TypeText("You healed for " + healAmount + " health!"));

        GameObject heal = Instantiate(hero.heal, battleHUD.heroSprite.transform.position, Quaternion.identity);
        Destroy(heal, 0.3f);

        //Update the UI
        battleHUD.UpdateBattleUI(hero, opponent);
        yield return new WaitForSeconds(2f);

        currentState = BattleStates.OPPONENTCHOICE;
        OpponentTurn();
    }

    IEnumerator PlayerSpecial()
    {
        battleHUD.BattleText.colorGradientPreset = hero.heroTextColor;
        StartCoroutine(TypeText(hero.heroSpecialAttackText));

        GameObject specialAttack = Instantiate(hero.specialAttack, battleHUD.opponentSprite.transform.position, Quaternion.identity);
        Destroy(specialAttack, 0.3f);

        hero.UseSpecialAttack(opponent);
        yield return new WaitForSeconds(1f);

        currentState = BattleStates.OPPONENTCHOICE;
        OpponentTurn();
    }

    IEnumerator OpponentAttack()
    {
        //Deal damage based on factors (level and attack)
        GameObject opponentAttack = Instantiate(opponent.opponentAttack, battleHUD.heroSprite.transform.position, Quaternion.identity);
        Destroy(opponentAttack, 0.3f);
        
        hero.TakeDamage(opponent);
        yield return new WaitForSeconds(1f);

        //Check if the hero is dead
        if(hero.currentHealth <= 0)
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

    public void OnAttack()
    {
        if(currentState != BattleStates.PLAYERCHOICE)
            return;

        TurnOffControls();

        StartCoroutine(PlayerAttack());
    }

    public void OnInventory()
    {
        if(currentState != BattleStates.PLAYERCHOICE)
            return;

        TurnOffControls();

        StartCoroutine(PlayerInventory());
    }

    public void OnSpecial()
    {
        if (currentState != BattleStates.PLAYERCHOICE)
            return;

        else if (hero.currentHeat < hero.maxHeat)
        {
            battleHUD.BattleText.colorGradientPreset = hero.heroTextColor;
            StartCoroutine(TypeText("Gotta build up more heat first..."));
            battleHUD.BattleText.colorGradientPreset = null;
            return;
        }

        TurnOffControls();

        //Special attack here
        StartCoroutine(PlayerSpecial());
    }

    public void OnRun()
    {
        if(currentState != BattleStates.PLAYERCHOICE)
            return;
        
        TurnOffControls();

        if(hero.speed >= opponent.opponentSpeed)
        {
            StartCoroutine(TypeText("You got away safely!"));
            //Transition over to previous screen here, for now exit the editor
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        
        else
        {
            StartCoroutine(TypeText("You couldn't get away!"));
            currentState = BattleStates.OPPONENTCHOICE;
            OpponentTurn();
        }
    }

    void TurnOffControls()
    {
        for(int i = 0; i < battleHUD.playerChoices.Length; i++)
        {
            battleHUD.playerChoices[i].interactable = false;
        }
    }

    void TurnOnControls()
    {
        for(int i = 0; i < battleHUD.playerChoices.Length; i++)
        {
            battleHUD.playerChoices[i].interactable = true;
        }
    }

    private IEnumerator EndingToBattleSequence()
    {
        yield return new WaitUntil(() => isBattleTextFinished);
        StopCoroutine(textCoroutine);
        GameManager.Instance.CurrentOpponent = null;
        GameManager.Instance.Currenthero.SaveHeroData();
        
        //Transition over to previous screen
        //SceneManager.LoadScene("Overworld");
        SceneManager.LoadScene("DevScene");
    }
}
