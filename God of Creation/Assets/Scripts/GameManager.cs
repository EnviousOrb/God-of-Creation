using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get; private set; }

    public HeroStats Currenthero { get; set; }
    public NPC CurrentOpponent { get; set; }

    private HashSet<string> defeatedOpponents = new();

    public List<HeroStats> heroParty = new();

    private HeroUI heroUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Currenthero = FindAnyObjectByType<HeroStats>();
            AddHeroToParty(Currenthero);
            heroUI = FindAnyObjectByType<HeroUI>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "BattleScene")
        {
            return;
        }

        heroParty.Clear();
        OnSwapHero(Currenthero.heroName);

        HeroStats[] heroes = FindObjectsByType<HeroStats>(FindObjectsSortMode.None);

        foreach (HeroStats hero in heroes)
        {
            if(hero != Currenthero)
            {
                Destroy(hero.gameObject);
            }
        }
    }

    public void MarkOpponentAsDefeated(NPC opponent)
    {
        defeatedOpponents.Add(opponent.npcName);
    }

    public void MarkOpponentAsSpared(NPC opponent)
    {
        opponent.wasSpared = true;
        PlayerPrefs.SetInt(opponent.npcName + "_wasSpared", 1);
        PlayerPrefs.Save();
    }

    public bool IsOpponentDefeated(NPC opponent)
    {
        return defeatedOpponents.Contains(opponent.npcName);
    }

    public void AddHeroToParty(HeroStats hero)
    {
        if(!heroParty.Contains(hero))
            heroParty.Add(hero);
    }

    public void RemoveHeroFromParty(HeroStats hero)
    {
        if (heroParty.Contains(hero))
            heroParty.Remove(hero);
    }

    public HeroStats GetHeroFromParty(string heroName)
    {
        return heroParty.Find(hero => hero.heroName == heroName);
    }

    public void SetCurrentHero(string heroName)
    {
        Currenthero = GetHeroFromParty(heroName);
    }

    public void OnSwapHero(string HeroName)
    {
        HeroStats hero = GetHeroFromParty(HeroName);

        if (!hero)
        {
            GameObject heroPrefab = Resources.Load<GameObject>($"HeroPrefabs/{HeroName}");
            if (heroPrefab)
            {
                GameObject heroInstance = Instantiate(heroPrefab);
                heroInstance.name = HeroName;
                hero = heroInstance.GetComponent<HeroStats>();
                AddHeroToParty(hero);
            }
            else
            {
                Debug.LogError($"Hero with name {HeroName} not found!");
                return;
            }
        }

        if(Currenthero)
            Currenthero.gameObject.SetActive(false);

        hero.gameObject.SetActive(true);

        Currenthero = hero;
        Currenthero.LoadHeroData();

        if (hero.gameObject.scene.name != SceneManager.GetActiveScene().name)
        {
            SceneManager.LoadScene(hero.gameObject.scene.name);
        }

        if(heroUI)
            heroUI.UpdateOverworldUI(Currenthero);
    }
}
