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
        print("Setting up battle");
        yield return new WaitForSeconds(2f);
        print("Hero speed: " + heroStats.speed);
        print("Opponent speed: " + opponentStats.opponentSpeed);
        if(heroStats.speed >= opponentStats.opponentSpeed)
        {
            currentState = BattleStates.PLAYERCHOICE;
            print("Player goes first");
            PlayerTurn();
        }
        else
        {
            currentState = BattleStates.OPPONENTCHOICE;
            print("Opponent goes first");
            OpponentTurn();
        }
    }

    IEnumerator TypeText(string text, float delayAfterText = 2f)
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
        if(currentState == BattleStates.WIN || currentState == BattleStates.LOSE)
            StartCoroutine(EndBattle());
        print("Player's turn");
        for(int i = 0; i < battleHUD.playerChoices.Length; i++)
        {
            battleHUD.playerChoices[i].interactable = true;
        }
        StartCoroutine(TypeText("Time to..."));
    }

    void OpponentTurn()
    {
        print("Opponent's turn");
        for(int i = 0; i < battleHUD.playerChoices.Length; i++)
        {
            battleHUD.playerChoices[i].interactable = false;
        }
        StartCoroutine(TypeText("Opponent's turn..."));

        //Figure out a choice depending on health and likelyhood of winning
        //If your health is low, then defend or heal, depends on hidden coin toss mechanic
        //If health is high, then attack
        //If the hero's special was just used, then block for a turn
        //For now, just attack or heal randomly
        int choice = UnityEngine.Random.Range(0, 1);
        if(choice == 0)
        {
            print("choice is " + choice.ToString() + ": Opponent attacks");
            StartCoroutine(OpponentAttack());
        }
        else if (choice == 1)
        {
            print("choice is " + choice.ToString() + ": Opponent heals");
            //Heal the opponent a random amount
            int healAmount = UnityEngine.Random.Range(1, opponentStats.maxHealth);
            opponentStats.currentHealth += Mathf.Min(opponentStats.maxHealth, healAmount);
            if(opponentStats.currentHealth > opponentStats.maxHealth)
            {
                opponentStats.currentHealth = opponentStats.maxHealth;
            }
            print("Opponent health: " + opponentStats.currentHealth);

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
        if(currentState == BattleStates.WIN)
        {
            StartCoroutine(TypeText("You win!"));
            yield return new WaitForSeconds(1f);
            battleHUD.BattleText.colorGradientPreset = opponentStats.opponentTextColor;
            StartCoroutine(TypeText(opponentStats.opponentFlavorTexts[1]));
            battleHUD.BattleText.colorGradientPreset = null;
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
            battleHUD.BattleText.colorGradientPreset = null;
            //Go to game over screen here, for now exit the editor
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    IEnumerator PlayerAttack()
    {
        print("In PlayerAttack");
        //Deal damage based on factors (level and attack)
        opponentStats.TakeDamage(heroStats);
        print("Opponent health: " + opponentStats.currentHealth);
        heroStats.currentHeat += 2;
        print("Hero heat: " + heroStats.currentHeat);
        yield return new WaitForSeconds(1f);

        //Check if the opponent is dead
        if(opponentStats.currentHealth <= 0)
        {
            currentState = BattleStates.WIN;
            print("current state is: " + currentState);
            StartCoroutine(EndBattle());
        }
        else
        {
            currentState = BattleStates.OPPONENTCHOICE;
            print("current state is: " + currentState);
            OpponentTurn();
        }
    }

    IEnumerator PlayerInventory()
    {
        print("Player uses inventory");
        //Open inventory...is what I would say if I had one, for now, heal the hero a random amount
        int healAmount = UnityEngine.Random.Range(1, heroStats.maxHealth);
        heroStats.currentHealth += Mathf.Min(heroStats.maxHealth, healAmount);
        if(heroStats.currentHealth > heroStats.maxHealth)
        {
            heroStats.currentHealth = heroStats.maxHealth;
        }

        //Display the amount healed
        StartCoroutine(TypeText("You healed for " + healAmount + " health!"));

        //Update the UI
        battleHUD.UpdateBattleUI(heroStats, opponentStats);
        yield return new WaitForSeconds(2f);

        currentState = BattleStates.OPPONENTCHOICE;
        OpponentTurn();
    }

    IEnumerator PlayerSpecial()
    {
        print("Player uses special attack");
        battleHUD.BattleText.colorGradientPreset = heroStats.heroTextColor;
        TypeText(heroStats.heroSpecialAttackText);
        battleHUD.BattleText.colorGradientPreset = null;
        heroStats.UseSpecialAttack(opponentStats);
        yield return new WaitForSeconds(1f);


        currentState = BattleStates.OPPONENTCHOICE;
        OpponentTurn();
    }

    IEnumerator OpponentAttack()
    {
        print("Opponent attacks");
        //Deal damage based on factors (level and attack)
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
        print("Attack button pressed");
        if(currentState != BattleStates.PLAYERCHOICE)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnInventory()
    {
        print("Inventory button pressed");
        if(currentState != BattleStates.PLAYERCHOICE)
            return;

        StartCoroutine(PlayerInventory());
    }

    public void OnSpecial()
    {
        print("Special button pressed");
        if (currentState != BattleStates.PLAYERCHOICE)
            return;

        else if (heroStats.currentHeat < heroStats.maxHeat)
        {
            print("Current heat: " + heroStats.currentHeat);
            battleHUD.BattleText.colorGradientPreset = heroStats.heroTextColor;
            StartCoroutine(TypeText("Gotta build up more heat first..."));
            battleHUD.BattleText.colorGradientPreset = null;
            return;
        }
        //Special attack here
        StartCoroutine(PlayerSpecial());
    }

    public void OnRun()
    {
        print("Run button pressed");

        if(currentState != BattleStates.PLAYERCHOICE)
            return;

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
}
