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

    private HeroStats heroInScene;

    [HideInInspector] public int heroIDtoFind;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Currenthero = heroParty[0];
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(SceneManager.GetActiveScene().name == "DevScene")
        {
            heroInScene = FindAnyObjectByType<HeroStats>();
            if (heroInScene != null)
            {
                Currenthero = heroInScene;
            }
            else
            {
                heroInScene = heroParty.Find(hero => hero.HeroID == heroIDtoFind);
                Instantiate(heroInScene);
                Currenthero = heroInScene;
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
}
