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
            if (heroParty.Find(hero => hero.heroName == Currenthero.heroName) == null)
                heroParty.Add(Currenthero);
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
        if (!Currenthero)
        {
            Debug.LogError("Current hero is not set!");
            return;
        }

        HeroStats[] heroesInScene = FindObjectsByType<HeroStats>(FindObjectsSortMode.None);
        if (heroesInScene.Length == 0)
        {
            GameObject heroPrefab = Resources.Load<GameObject>($"HeroPrefabs/{Currenthero.heroName}");
            if (heroPrefab)
            {
                GameObject heroInstance = Instantiate(heroPrefab);
                heroInstance.name = Currenthero.heroName;
                Currenthero = heroInstance.GetComponent<HeroStats>();
                AddHeroToParty(heroInstance.GetComponent<HeroStats>());
            }
            else
            {
                Debug.LogError($"Hero with name {Currenthero.heroName} not found!");
            }
        }
        else
        {
            foreach (var hero in heroesInScene)
            {
               if(hero.heroName == Currenthero.heroName)
                {
                    Currenthero = hero;
                    Currenthero.gameObject.SetActive(true);
                    break;
                }
            }
        }

    }

    public void MarkOpponentAsDefeated(NPC opponent)
    {
        defeatedOpponents.Add(opponent.npcName);
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
