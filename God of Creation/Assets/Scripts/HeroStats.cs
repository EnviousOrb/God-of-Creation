using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class HeroStats : MonoBehaviour
{
    #region Fields
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
    private float dodgeChance; //The chance the hero has to dodge an attack
    [HideInInspector] public int credixAmount; //The amount of credix the hero has
    [HideInInspector] public Inventory inventory; //The inventory of the hero

    [Header("Hero XP")]
    public int xpToLevel; //The amount of XP needed to level up the hero
    [HideInInspector] public int currentXP; //The current XP of the hero


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

    [Header("Hero Dialogue")]
    public string heroOpeningDialog; //This is the opening dialog of the hero
    public string heroVictoryDialog; //This is the victory dialog of the hero
    public string heroDefeatDialog; //This is the defeat dialog of the hero

    [HideInInspector] public int currentHealth; //This is the current health of the hero
    [HideInInspector] public int currentHeat; //This is the current heat of the hero

    [Header("Hero Skills")]
    public List<Skill> heroSkills; //This is the list of skills the hero has
    [HideInInspector] public int chosenPath; //This is the path the hero has chosen

    [Header("Hero Audio")]
    public AudioClip heroTextSound; //This is the sound that plays when the hero speaks
    public AudioClip heroSpecialAttackSound; //This is the sound that plays when the hero uses their special attack
    public AudioClip heroNormalAttackSound; //This is the sound that plays when the hero uses their normal attack
    public AudioClip heroHealSound; //This is the sound that plays when the hero heals

    private MenuManager menuManager;
    #endregion

    private void Awake()
    {
        menuManager = FindAnyObjectByType<MenuManager>();
        inventory = GetComponent<Inventory>();
        LoadHeroData();
    }

    #region Public Methods
    public void TakeDamage(NPC opponent)
    {
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            //Handle death stuff for hero here
            return;
        }

        if(Random.value < dodgeChance)
        {
            //Dodge the attack
            return;
        }

        //Calcualte the damage the hero takes
        int damage = CalculateDamage(opponent.opponentDamage, opponent.opponentLevel, defense);
        currentHealth -= damage;
    }

    public void UseSpecialAttack(NPC opponent)
    {
        //Apply the effect of the special attack to the opponent
        int damage = attack * Level - opponent.opponentDefense;
        damage = Mathf.Max(1, damage);
        currentHeat = 0;
        opponent.currentHealth -= damage;
    }

    public void UseSkill(Skill skill, NPC opponent)
    {
        // Apply the effect of the skill to the opponent
        if(skill.IsUnlocked)
            skill.ApplyEffect?.Invoke(this, opponent);
    }

    public void AddXP(int amount)
    {
        // Add the amount of XP to the hero and check if they need to level up
        currentXP += amount;
        if (currentXP >= xpToLevel)
        {
            if(Level < 25)
                LevelUp();
            else
                currentXP = xpToLevel;
        }
    }

    public bool DoesOpponentFlee(NPC Opponent)
    {
        var acSkill = heroSkills.Find(skill => skill.SkillName == "Apex Creationist");
        if (acSkill != null && acSkill.IsUnlocked && !Opponent.CompareTag("Boss"))
        {
            float fleeChance = 0.5f;
            return Random.value < fleeChance;
        }
        return false;
    }

    public bool IsEnemyConfused()
    {
        var aiSkill = heroSkills.Find(skill => skill.SkillName == "Afterimages");
        if (aiSkill != null && aiSkill.IsUnlocked)
        {
            float confuseChance = 0.5f;
            return Random.value < confuseChance;
        }
        return false;
    }

    public void SaveHeroData()
    {
        // Save the data to a file
        using StreamWriter writer = new(GetFilePath(), false);
        writer.WriteLine(heroName);
        writer.WriteLine(heroBio);
        writer.WriteLine(heroSpecialAttackText);
        writer.WriteLine(string.Join(", ", heroSkills.Where(skill => skill.IsUnlocked).Select(skill => skill.SkillName).ToArray()));
        writer.WriteLine(maxHealth);
        writer.WriteLine(Level);
        writer.WriteLine(maxHeat);
        writer.WriteLine(attack);
        writer.WriteLine(defense);
        writer.WriteLine(speed);
        writer.WriteLine(credixAmount);
        writer.WriteLine(currentHealth);
        writer.WriteLine(currentHeat);
        writer.WriteLine(currentXP);
        writer.WriteLine(xpToLevel);
        writer.WriteLine(chosenPath);

        foreach (var item in inventory.items)
        {
            writer.WriteLine($"{item.ItemName}:{item.ItemCount}:{item.PictureID}");
        }
    }

    public void LoadHeroData()
    {
        // Load the data from the file
        string path = GetFilePath();
        if (File.Exists(path))
        {
            using StreamReader reader = new(path);
            heroName = reader.ReadLine();
            heroBio = reader.ReadLine();
            heroSpecialAttackText = reader.ReadLine();
            string unlockedSkills = reader.ReadLine();
            if (!string.IsNullOrEmpty(unlockedSkills))
                heroSkills.Where(skill => unlockedSkills.Contains(skill.SkillName)).ToList().ForEach(skill => skill.IsUnlocked = true);
            else
                foreach (Skill skill in heroSkills)
                    skill.IsUnlocked = false;

            LoadHeroSKills();

            maxHealth = int.Parse(reader.ReadLine());
            Level = int.Parse(reader.ReadLine());
            maxHeat = int.Parse(reader.ReadLine());
            attack = int.Parse(reader.ReadLine());
            defense = int.Parse(reader.ReadLine());
            speed = float.Parse(reader.ReadLine());
            credixAmount = int.Parse(reader.ReadLine());
            currentHealth = int.Parse(reader.ReadLine());
            currentHeat = int.Parse(reader.ReadLine());
            currentXP = int.Parse(reader.ReadLine());
            xpToLevel = int.Parse(reader.ReadLine());
            chosenPath = int.Parse(reader.ReadLine());

            LoadInventory(reader);
        }
        else
        {
            InitalizeHeroSkills();
        }

        menuManager.heroUI.SetOverworldUI(this);
        menuManager.skillTreeMenu.GetComponent<SkillTree>().OnCharacterSelect(this);
    }

    #endregion

    #region Private Methods

    private void LoadHeroSKills()
    {
        if (heroName == "William")
            LoadWilliamSkills();
        else if (heroName == "EnviousOrb")
            LoadEnviousOrbSkills();
        else if (heroName == "Dr.Creeper")
            LoadDrCreeperSkills();
    }

    private void LoadInventory(StreamReader reader)
    {
        if (inventory.items == null)
            inventory.items = new List<Item>();
        else
            inventory.items.Clear();

        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();

            if (string.IsNullOrEmpty(line))
                continue;

            var parts = line.Split(':');
            if (parts.Length == 3)
            {
                Item item = ScriptableObject.CreateInstance<Item>();
                item.ItemName = parts[0];
                item.ItemCount = int.Parse(parts[1]);
                item.ItemIcon = LoadSprite(parts[2]);
                inventory.items.Add(item);
            }
        }
    }

    private void InitalizeHeroSkills()
    {
        currentHealth = maxHealth;
        currentHeat = 0;
        currentXP = 0;
        credixAmount = 0;
        xpToLevel = Level * 100;
    }

    private int CalculateDamage(int opponentDamage, int opponentLevel, int heroDefense)
    {
        int damage = opponentDamage * (1 + (opponentLevel / 10) - heroDefense);
        return Mathf.Max(1, damage);
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
        return Path.Combine(Application.persistentDataPath, heroName + "_data.txt");
    }

    private Sprite LoadSprite(string pictureIndex)
    {
        // Load the sprite from the resources folder
        return Resources.Load<Sprite>("Items/" + pictureIndex);
    }

    private IEnumerator DealDamageOverTime(HeroStats hero, NPC opponent, int damage, float time)
    {
        int damagerPerTick = damage / 5;
        float tick = time / 5;

        for (int i = 0; i < 5; i++)
        {
            int Totaldamage = Mathf.Max(1, damagerPerTick - opponent.opponentDefense);
            opponent.currentHealth -= Totaldamage;
            yield return new WaitForSeconds(tick);
        }
    }

    private void LoadWilliamSkills()
    {
        foreach (var skill in heroSkills)
        {
            switch (skill.SkillName)
            {
                case "Chain Attack":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        for(int i = 0; i < 3; i++)
                        {
                            int damage = hero.attack * hero.Level - opponent.opponentDefense;
                            damage = Mathf.Max(1, damage);
                            opponent.currentHealth -= damage;
                        }
                    };
                    break;
                case "Creationist's Regen":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        hero.currentHealth += 5;
                    };
                    break;
                case "Fortified Axe":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        int damage = hero.attack * hero.Level + 10 - opponent.opponentDefense;
                        damage = Mathf.Max(1, damage);
                        opponent.currentHealth -= damage;
                    };
                    break;
                case "Light-Footed":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        hero.speed += 4;
                    };
                    break;
                case "Fortified Regen":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        hero.currentHealth += 10;
                    };
                    break;
                case "Party Regen":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        foreach (var partyMember in GameManager.Instance.heroParty)
                        {
                            partyMember.currentHealth += 5;
                        }
                    };
                    break;
                case "Nuh Uh!":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        foreach (var partyMember in GameManager.Instance.heroParty)
                        {
                            partyMember.speed += 5;
                            partyMember.defense += 5;
                        }
                    };
                    break;

                default:
                    break;
            }
        }
    }
    private void LoadEnviousOrbSkills()
    {
        foreach (var skill in heroSkills)
        {
            switch (skill.SkillName)
            {
                case "Improved Sword Techniques":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        int damage = hero.attack * hero.Level + 5 - opponent.opponentDefense;
                        damage = Mathf.Max(1, damage);
                        opponent.currentHealth -= damage;
                    };
                    break;
                case "Swordsman's Resolve":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        hero.attack += 5;
                        hero.defense += 5;
                        hero.speed += 5;
                    };
                    break;
                case "Sharpened Katana":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        int damage = hero.attack * hero.Level + 10 - opponent.opponentDefense;
                        damage = Mathf.Max(1, damage);
                        opponent.currentHealth -= damage;
                    };
                    break;
                case "Enhanced Senses":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        dodgeChance = 0.5f;
                    };
                    break;
                case "Prideful Swordsman":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                       foreach(var partyMember in GameManager.Instance.heroParty)
                        {
                            partyMember.attack += 5;
                        }
                    };
                    break;
                case "Soul Slice":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        hero.StartCoroutine(DealDamageOverTime(hero, opponent, 5, 1.0f));
                    };
                    break;
                case "Parry & Counterstrike":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        dodgeChance = 1.0f;
                        int damage = hero.attack * hero.Level + 15 - opponent.opponentDefense;
                        damage = Mathf.Max(1, damage);
                        opponent.currentHealth -= damage;
                    };
                    break;
            }
        }
    }
    private void LoadDrCreeperSkills()
    {
        foreach (var skill in heroSkills)
        {
            switch (skill.SkillName)
            {
                case "Chemistry Refinement":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        hero.attack += 5;
                        hero.maxHealth += 10;
                    };
                    break;
                case "99.1% Purity":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        hero.attack += 10;
                        hero.defense += 10;
                        hero.maxHealth += 20;
                    };
                    break;
                case "Chemist's Sword":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        if(Random.value < 0.3f)
                            StartCoroutine(DealDamageOverTime(hero, opponent, 2, 2.0f));
                    };
                    break;
                case "Knight's Training":
                    skill.ApplyEffect = (hero, opponent) =>
                    {
                        dodgeChance = Random.value;
                    };
                    break;

            }
        }
    }
    #endregion
}

[System.Serializable]
public class HeroData
{
    public string heroName;
    public string heroBio;
    public string heroSpecialAttackText;
    public List<string> unlockedSkills;
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
    public int chosenPath;
}

[System.Serializable]
public class InventoryData 
{
    public string itemName;
    public int itemCount;
}