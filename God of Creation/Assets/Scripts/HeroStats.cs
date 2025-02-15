using TMPro;
using UnityEngine;

public class HeroStats : MonoBehaviour
{
    [Header("Hero Stats")]
    public string heroName;
    public string heroSpecialAttackText; //This is the text that appears when the hero uses their special attack
    [Range(0, 100)] public int maxHealth; //This is the maximum health of the hero
    [Range(0, 25)] public int Level; //This is the maximum level of the hero
    [Range(0, 100)] public int maxHeat; //This is the maximum heat of the hero
    [Range(0, 100)] public int attack; //Base attack of the hero
    [Range(0, 100)] public int defense; //Base defense of the hero
    [Range(0, 100)] public float speed; //Base speed of the hero
    

    [Header("Hero Icons")]
    public Sprite heroIcon; //This is the sprite seen during dialog, overworld, and the main UI
    public Sprite typeIcon; //This is the sprite seen on the main UI, used to show the type of the hero
    public Sprite battleIcon; //This is the sprite seen during battle
    public Sprite specialAttackIcon; //This is the sprite seen on the battle UI, used to show the special attack of the hero

    [Header("Hero Color")]
    public TMP_ColorGradient heroTextColor; //The color of the hero's dialog text

    [HideInInspector] public int currentHealth; //This is the current health of the hero
    [HideInInspector] public int currentHeat; //This is the current heat of the hero

    void Start()
    {
        currentHealth = maxHealth;
        currentHeat = maxHeat;
    }

    public void TakeDamage(OpponentStats opponentStats)
    {
        print("In Hero's TakeDamage");
        int damage = opponentStats.opponentDamage * (1 + (opponentStats.opponentLevel / 10) - defense);
        damage = Mathf.Max(1, damage);
        currentHealth -= damage;

        print("Hero health: " + currentHealth);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            //Handle death stuff for hero here
        }
    }

    public void UseSpecialAttack(OpponentStats opponentStats)
    {
        print("In UseSpecialAttack");
        int damage = attack * 2 - opponentStats.opponentDefense;
        damage = Mathf.Max(1, damage);
        currentHeat = 0;
        opponentStats.currentHealth -= damage;
        print("Opponent health: " + opponentStats.currentHealth);
    }
}
