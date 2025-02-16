using System;
using System.Collections;
using UnityEngine;


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
    [HideInInspector] public BattleStates currentState;

    [Header("Battle Stats")]
    [SerializeField] public HeroStats heroStats; //The hero's stats
    [SerializeField] public OpponentStats opponentStats; //The opponent's stats

    [Header("Battle HUD")]
    [SerializeField] public BattleHUD battleHUD; // Reference to the BattleHUD script

    [SerializeField] private float typingSpeed; //The speed at which the battle text will be displayed on screen

    private Coroutine textCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        battleHUD.SetBattleUI(heroStats, opponentStats);
        currentState = BattleStates.START;
        StartCoroutine(TypeText(opponentStats.opponentFlavorTexts[0]));
        StartCoroutine(SetupBattle());
    }

    // Update is called once per frame
    void Update()
    {
        battleHUD.UpdateBattleUI(heroStats, opponentStats);
    }

    IEnumerator SetupBattle()
    {
        TurnOffControls();

        yield return new WaitForSeconds(2f);
        if(heroStats.speed >= opponentStats.opponentSpeed)
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
        if(textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }
        textCoroutine = StartCoroutine(TypeTextCoroutine(text, delayAfterText));
        yield return textCoroutine;
    }

    IEnumerator TypeTextCoroutine(string text, float delayAfterText = 2f)
    {
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
        TurnOnControls();

        battleHUD.BattleText.colorGradientPreset = null;
        if(heroStats.currentHealth <= 0)
        {
            currentState = BattleStates.LOSE;
            StartCoroutine(EndBattle());
        }
        for(int i = 0; i < battleHUD.playerChoices.Length; i++)
        {
            battleHUD.playerChoices[i].interactable = true;
        }
        StartCoroutine(TypeText("Time to..."));
    }

    void OpponentTurn()
    {
        battleHUD.BattleText.colorGradientPreset = null;
        if(opponentStats.currentHealth <= 0)
        {
            currentState = BattleStates.WIN;
            StartCoroutine(EndBattle());
        }

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
            int healAmount = UnityEngine.Random.Range(1, opponentStats.maxHealth / 2);
            opponentStats.currentHealth += Mathf.Min(opponentStats.maxHealth / 2, healAmount);
            if(opponentStats.currentHealth > opponentStats.maxHealth)
            {
                opponentStats.currentHealth = opponentStats.maxHealth;
            }

            GameObject heal = Instantiate(opponentStats.opponentHeal, battleHUD.opponentSprite.transform.position, Quaternion.identity);
            Destroy(heal, 0.3f);

            //Display the amount healed
            StartCoroutine(TypeText("Opponent healed for " + healAmount + " health!"));

            //Update the UI
            battleHUD.UpdateBattleUI(heroStats, opponentStats);
            currentState = BattleStates.PLAYERCHOICE;
            PlayerTurn();
        }
    }

    IEnumerator EndBattle()
    {
        TurnOffControls();

        if(currentState == BattleStates.WIN)
        {
            StartCoroutine(TypeText("You win!"));
            yield return new WaitForSeconds(1f);
            battleHUD.BattleText.colorGradientPreset = opponentStats.opponentTextColor;
            StartCoroutine(TypeText(opponentStats.opponentFlavorTexts[1]));
            //Transition over to previous screen here, for now exit the editor
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else if(currentState == BattleStates.LOSE)
        {
            StartCoroutine(TypeText("You lose!"));
            yield return new WaitForSeconds(1f);
            battleHUD.BattleText.colorGradientPreset = opponentStats.opponentTextColor;
            StartCoroutine(TypeText(opponentStats.opponentFlavorTexts[2]));
            //Go to game over screen here, for now exit the editor
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    IEnumerator PlayerAttack()
    {
        //Deal damage based on factors (level and attack)
        GameObject normalAttack = Instantiate(heroStats.normalAttack, battleHUD.opponentSprite.transform.position, Quaternion.identity);
        Destroy(normalAttack, 0.3f);
        opponentStats.TakeDamage(heroStats);
        heroStats.currentHeat += 2;
        yield return new WaitForSeconds(1f);

        //Check if the opponent is dead
        if(opponentStats.currentHealth <= 0)
        {
            currentState = BattleStates.WIN;
            StartCoroutine(EndBattle());
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
        int healAmount = UnityEngine.Random.Range(1, heroStats.maxHealth);
        heroStats.currentHealth += Mathf.Min(heroStats.maxHealth, healAmount);
        if(heroStats.currentHealth > heroStats.maxHealth)
        {
            heroStats.currentHealth = heroStats.maxHealth;
        }

        //Display the amount healed
        StartCoroutine(TypeText("You healed for " + healAmount + " health!"));

        GameObject heal = Instantiate(heroStats.heal, battleHUD.heroSprite.transform.position, Quaternion.identity);
        Destroy(heal, 0.3f);

        //Update the UI
        battleHUD.UpdateBattleUI(heroStats, opponentStats);
        yield return new WaitForSeconds(2f);

        currentState = BattleStates.OPPONENTCHOICE;
        OpponentTurn();
    }

    IEnumerator PlayerSpecial()
    {
        battleHUD.BattleText.colorGradientPreset = heroStats.heroTextColor;
        StartCoroutine(TypeText(heroStats.heroSpecialAttackText));

        GameObject specialAttack = Instantiate(heroStats.specialAttack, battleHUD.opponentSprite.transform.position, Quaternion.identity);
        Destroy(specialAttack, 0.3f);

        heroStats.UseSpecialAttack(opponentStats);
        yield return new WaitForSeconds(1f);

        currentState = BattleStates.OPPONENTCHOICE;
        OpponentTurn();
    }

    IEnumerator OpponentAttack()
    {
        //Deal damage based on factors (level and attack)
        GameObject opponentAttack = Instantiate(opponentStats.opponentAttack, battleHUD.heroSprite.transform.position, Quaternion.identity);
        Destroy(opponentAttack, 0.3f);
        
        heroStats.TakeDamage(opponentStats);
        yield return new WaitForSeconds(1f);

        //Check if the hero is dead
        if(heroStats.currentHealth <= 0)
        {
            currentState = BattleStates.LOSE;
            StartCoroutine(EndBattle());
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

        else if (heroStats.currentHeat < heroStats.maxHeat)
        {
            battleHUD.BattleText.colorGradientPreset = heroStats.heroTextColor;
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

        if(heroStats.speed >= opponentStats.opponentSpeed)
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
}
