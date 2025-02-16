using UnityEngine;
using TMPro;

public class OpponentStats : MonoBehaviour
{
    [SerializeField] public Sprite opponentIcon;
    public string opponentName;
    public string[] opponentFlavorTexts; //The text that appears right as soon as a battle begins

    public TMP_ColorGradient opponentTextColor; //The color of the opponent's dialog text

    public GameObject opponentAttack;
    public GameObject opponentHeal;
    [Range (0, 25)] public int opponentLevel;
    [Range(0, 100)] public int maxHealth;
    [Range(0, 100)] public int opponentDamage;
    [Range(0, 100)] public float opponentSpeed;
    [Range(0, 100)] public int opponentDefense;

    [HideInInspector] public int currentHealth;

    public void TakeDamage(HeroStats heroStats)
    {
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            return;
            //Handle death stuff for enemy here
        }
        int damage = heroStats.attack * (1 + (heroStats.Level / 10)) - opponentDefense;
        damage = Mathf.Max(1, damage);
        currentHealth -= damage;
    }
}
