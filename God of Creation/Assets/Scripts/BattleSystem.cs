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

        private int choice;

        private int cooldown;

        private int skipTurns;

        public bool IsBattleTextFinished { get { return battleHUD.BattleText.text == string.Empty; } }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            hero = GameManager.Instance.Currenthero;
            opponent = GameManager.Instance.CurrentOpponent;
            battleHUD = FindObjectsByType<BattleHUD>(FindObjectsSortMode.None)[0];

            battleHUD.SetBattleUI(hero, opponent);
            foreach(var button in battleHUD.InventoryButtons)
            {
                button.onClick.AddListener(() => UseItem(button.transform.GetSiblingIndex()));
            }
            battleHUD.UpdateInventoryUI(hero.inventory);
            currentState = BattleStates.START;
            StartCoroutine(TypeText(opponent.opponentFlavorTexts[0]));
            StartCoroutine(SetupBattle());
        }

        // Update is called once per frame
        void Update()
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

        //YandereDev-like code, don't touch
        private void LoadItemEffects()
        {
            foreach (var item in hero.inventory.items)
            {
                switch (item.ItemName)
                {
                    case "Chibi-Bean© Ms.Slime Bandage":
                        item.ApplyEffect = (hero) =>
                        {
                            hero.currentHealth += 3;
                        };
                        break;
                    case "Captain Spark's \"All-In-One\" Buffet Stew":
                        item.ApplyEffect = (hero) =>
                        {
                            GameManager.Instance.heroParty.ForEach(hero => hero.currentHealth = hero.maxHealth);
                        };
                        break;
                    case "Chibi-Bean© \"Friends Forever!\" Care Package":
                        item.ApplyEffect = (hero) =>
                        {
                            hero.currentHealth = hero.maxHealth;
                            skipTurns = 1;
                        };
                        break;
                    case "First-Aid Kit":
                        item.ApplyEffect = (hero) =>
                        {
                            hero.currentHealth = hero.maxHealth / 2;
                        };
                        break;
                    case "SOMA© XJ9-Runman":
                        item.ApplyEffect = (hero) =>
                        {
                            GameManager.Instance.heroParty.ForEach(hero => hero.currentHeat = hero.maxHeat / 2);
                        };
                        break;
                    case "SOMA© XJ9-Runman Pro Max":
                        item.ApplyEffect = (hero) =>
                        {
                            GameManager.Instance.heroParty.ForEach(hero => hero.currentHeat = hero.maxHeat);
                        };
                        break;
                    case "Revitanix":
                        item.ApplyEffect = (hero) =>
                        {
                            hero.currentHeat += 3;
                        };
                        break;
                    case "Revitanix X":
                        item.ApplyEffect = (hero) =>
                        {
                            hero.currentHeat = 9;
                        };
                        break;
                    case "Revitanix MAX":
                        item.ApplyEffect = (hero) =>
                        {
                            hero.currentHeat += 21;
                        };
                        break;
                    case "Staminfuel":
                        item.ApplyEffect = (hero) =>
                        {
                            hero.currentHeat += 3;
                            hero.currentHealth += 3;
                        };
                        break;
                    case "Staminfuel X":
                        item.ApplyEffect = (hero) =>
                        {
                            hero.currentHeat += 9;
                            hero.currentHealth += 9;
                        };
                        break;
                    case "Staminfuel MAX":
                        item.ApplyEffect = (hero) =>
                        {
                            hero.currentHeat += 21;
                            hero.currentHealth += 21;
                        };
                        break;
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
                    hero.UseSkill(hero.heroSkills[0], opponent);
                    hero.UseSkill(hero.heroSkills[1], opponent);
                    hero.UseSkill(hero.heroSkills[2], opponent);
                    hero.UseSkill(hero.heroSkills[3], opponent);
                    hero.UseSkill(hero.heroSkills[5], opponent);
                    hero.UseSkill(hero.heroSkills[7], opponent);
                }
                else if (hero.heroName == "Dr.Creeper")
                {
                    for (int i = 0; i < hero.heroSkills.Count; i++)
                    {
                        if (i == 1)
                            continue;

                        hero.UseSkill(hero.heroSkills[i], opponent);
                    }
                }

                cooldown = 5;
            }
        }

        void PlayerTurn()
        {
            if (skipTurns > 0)
            {
                skipTurns--;
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
                battleHUD.BattleText.colorGradientPreset = hero.heroTextColor;
                StartCoroutine(TypeText("The Opponent's next move is: " + GetOpponentNextMove()));
            }
            //Save the hero's data before the player makes a choice
            GameManager.Instance.Currenthero.SaveHeroData();

            //Enable the player's choices and display the text
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
            battleHUD.BattleText.colorGradientPreset = null;

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

            //Figure out a choice depending on health and likelyhood of winning
            //If your health is low, then defend or heal, depends on hidden coin toss mechanic
            //If health is high, then attack
            //If the hero's special was just used, then block for a turn
            //For now, just attack or heal randomly

            if (choice == 0)
            {
                StartCoroutine(OpponentAttack());
            }
            else if (choice == 1)
            {
                //Heal the opponent a random amount
                int healAmount = UnityEngine.Random.Range(1, opponent.maxHealth / 2);
                opponent.currentHealth += Mathf.Min(opponent.maxHealth / 2, healAmount);
                if (opponent.currentHealth > opponent.maxHealth)
                {
                    opponent.currentHealth = opponent.maxHealth;
                }

                GameObject heal = Instantiate(opponent.opponentHeal, battleHUD.opponentSprite.transform.position, Quaternion.identity);
                AudioManager.Instance.PlaySFX(opponent.opponentHealSound);
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

            if (currentState == BattleStates.WIN)
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
            else if (currentState == BattleStates.LOSE)
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

            heroUI.UpdateOverworldUI(hero);
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
                opponent.TakeDamage(hero);
            }
            else if (hero.heroName == "EnviousOrb")
            {
                hero.UseSkill(hero.heroSkills[6], opponent);
                opponent.TakeDamage(hero);
            }
            else
                opponent.TakeDamage(hero);


            hero.currentHeat += 2;
            if (hero.currentHeat > hero.maxHeat)
            {
                hero.currentHeat = hero.maxHeat;
            }
            yield return new WaitForSeconds(1f);

            //Check if the opponent is dead
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

        private string GetOpponentNextMove()
        {
            if (choice == 0)
                return "Attack";
            else if (choice == 1)
                return "Heal";
            else
                return "Unknown";
        }

        public void OnAttack()
        {
            if (currentState != BattleStates.PLAYERCHOICE)
                return;

            TurnOffControls();

            StartCoroutine(PlayerAttack());
        }

        public void OnInventory()
        {
            if (currentState != BattleStates.PLAYERCHOICE)
                return;

            battleHUD.ToggleInventory();
        }

        public void UseItem(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= hero.inventory.items.Count)
                return;

            Item item = hero.inventory.items[itemIndex];
            item.ApplyEffect(hero);
            hero.inventory.RemoveItem(item);
            battleHUD.UpdateBattleUI(hero, opponent);
            battleHUD.UpdateInventoryUI(hero.inventory);
            battleHUD.ToggleInventory();
            currentState = BattleStates.OPPONENTCHOICE;
            OpponentTurn();
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
            if (currentState != BattleStates.PLAYERCHOICE)
                return;

            TurnOffControls();

            if (hero.speed >= opponent.opponentSpeed)
            {
                StartCoroutine(TypeText("You got away safely!"));
                //Transition over to previous screen here
                StartCoroutine(EndingToBattleSequence());
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
            for (int i = 0; i < battleHUD.playerChoices.Length; i++)
            {
                battleHUD.playerChoices[i].interactable = false;
            }
        }

        void TurnOnControls()
        {
            for (int i = 0; i < battleHUD.playerChoices.Length; i++)
            {
                battleHUD.playerChoices[i].interactable = true;
            }
        }

        private IEnumerator EndingToBattleSequence()
        {
            yield return new WaitForEndOfFrame();
            StopCoroutine(textCoroutine);
            GameManager.Instance.CurrentOpponent = null;
            GameManager.Instance.Currenthero.SaveHeroData();

            //Transition over to previous screen
            //SceneManager.LoadScene("Overworld");
            SceneManager.LoadScene("DevScene");
        }
    }
