using System.IO;
using TMPro;
using UnityEngine;

public class HeroStats : MonoBehaviour
{
    [Header("Hero Stats")]
    public string heroName; //This is the name of the hero
    public string heroBio; //This is the background info on the hero
    public string heroSpecialAttackText; //This is the text that appears when the hero uses their special attack
    [Range(0, 100)] public int maxHealth; //This is the maximum health of the hero
    [Range(0, 25)] public int Level; //This is the maximum level of the hero
    [Range(0, 100)] public int maxHeat; //This is the maximum heat of the hero
    [Range(0, 100)] public int attack; //Base attack of the hero
    [Range(0, 100)] public int defense; //Base defense of the hero
    [Range(0, 100)] public float speed; //Base speed of the hero
    [HideInInspector] public int credixAmount; //The amount of credix the hero has

    [Header("Hero XP")]
    public int xpToLevel; //The amount of XP needed to level up the hero
    public int currentXP; //The current XP of the hero


    [Header("Hero Icons")]
    public Sprite heroIcon; //This is the sprite seen during dialog, overworld, and the main UI
    public Sprite typeIcon; //This is the sprite seen on the main UI, used to show the type of the hero
    public Sprite battleIcon; //This is the sprite seen during battle
    public Sprite StatsIcon; //This is the sprite seen on the stats screen
    public GameObject specialAttack; //This is the sprite seen on the battle UI, used to show the special attack of the hero
    public GameObject normalAttack; //This is the sprite seen on the battle UI, used to show the normal attack of the hero
    public GameObject heal; //This is the sprite seen on the battle UI, used to show the healing of the hero

    [Header("Hero Color")]
    public TMP_ColorGradient heroTextColor; //The color of the hero's dialog text

    [HideInInspector] public int currentHealth; //This is the current health of the hero
    [HideInInspector] public int currentHeat; //This is the current heat of the hero

    private void Start()
    {
        LoadHeroData();
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
        // damage is calculated by the hero's attack multiplied by the hero's level minus the opponent's defense
        int damage = attack * Level - opponent.opponentDefense;
        damage = Mathf.Max(1, damage);
        currentHeat = 0;
        opponent.currentHealth -= damage;
    }

    public void AddXP(int amount)
    {
        // Add the amount of XP to the hero and check if they need to level up
        currentXP += amount;
        if (currentXP >= xpToLevel)
        {
            LevelUp();
        }
    }
    private void LevelUp()
    {
        // Increase the level of the hero and adjust the stats accordingly
        Level++;
        currentXP = 0;
        xpToLevel = 100 * Level;
        credixAmount += 100;
        maxHealth += 10;
        attack += 5;
        defense += 5;
        speed += 2;
        maxHeat += 10;
        currentHealth = maxHealth;
        SaveHeroData();
    }

    private string GetFilePath()
    {
        // Get the file path for the hero data, leads into AppData/LocalLow/SBKGames/God of Creation on Windows
        return Path.Combine(Application.persistentDataPath, heroName + "_data.json");
    }

    public void SaveHeroData()
    {
        // Save the data to a file
        _ = new HeroData()
        {
            heroName = heroName,
            heroBio = heroBio,
            heroSpecialAttackText = heroSpecialAttackText,
            maxHealth = maxHealth,
            Level = Level,
            maxHeat = maxHeat,
            attack = attack,
            defense = defense,
            speed = speed,
            credixAmount = credixAmount,
            currentHealth = currentHealth,
            currentHeat = currentHeat,
            xpToLevel = xpToLevel,
            currentXP = currentXP
        };
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(GetFilePath(), json);
    }

    public void LoadHeroData()
    {
        // Load the data from the file
        string path = GetFilePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            HeroData data = JsonUtility.FromJson<HeroData>(json);
            heroName = data.heroName;
            heroBio = data.heroBio;
            heroSpecialAttackText = data.heroSpecialAttackText;
            maxHealth = data.maxHealth;
            Level = data.Level;
            maxHeat = data.maxHeat;
            attack = data.attack;
            defense = data.defense;
            speed = data.speed;
            credixAmount = data.credixAmount;
            currentHealth = data.currentHealth > maxHealth ? maxHealth : data.currentHealth;
            currentHeat = data.currentHeat > maxHeat ? maxHeat : data.currentHeat;
            xpToLevel = data.xpToLevel;
            currentXP = data.currentXP;
        }
        else
        {
            currentHealth = maxHealth;
            currentHeat = 0;
            currentXP = 0;
            credixAmount = 0;
            xpToLevel = Level * 100;
        }
    }
}

[System.Serializable]
public class HeroData
{
    public string heroName;
    public string heroBio;
    public string heroSpecialAttackText;
    public int maxHealth;
    public int Level;
    public int maxHeat;
    public int attack;
    public int defense;
    public float speed;
    public int credixAmount;
    public int currentHealth;
    public int currentHeat;
    public int currentXP;
    public int xpToLevel;
}
