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
    public GameObject specialAttack; //This is the sprite seen on the battle UI, used to show the special attack of the hero
    public GameObject normalAttack; //This is the sprite seen on the battle UI, used to show the normal attack of the hero
    public GameObject heal; //This is the sprite seen on the battle UI, used to show the healing of the hero

    [Header("Hero Color")]
    public TMP_ColorGradient heroTextColor; //The color of the hero's dialog text

    [HideInInspector] public int currentHealth; //This is the current health of the hero
    [HideInInspector] public int currentHeat; //This is the current heat of the hero

    private void Start()
    {
        currentHealth = maxHealth;
        currentHeat = maxHeat;
    }

    public void TakeDamage(NPC opponent)
    {
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            return;
            //Handle death stuff for hero here
        }
        // damage is calculated by the opponent's damage multiplied by 1 plus the opponent's level divided by 10 minus the hero's defense
        int damage = opponent.opponentDamage * (1 + (opponent.opponentLevel / 10) - defense);
        damage = Mathf.Max(1, damage);
        currentHealth -= damage;
    }

    public void UseSpecialAttack(NPC opponent)
    {
        int damage = attack * Level - opponent.opponentDefense;
        damage = Mathf.Max(1, damage);
        currentHeat = 0;
        opponent.currentHealth -= damage;
    }
}
