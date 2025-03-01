using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get; private set; }

    public HeroStats Currenthero { get; set; }
    public NPC CurrentOpponent { get; set; }

    private HashSet<string> defeatedOpponents = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Currenthero = FindFirstObjectByType<HeroStats>();
            Currenthero.LoadHeroData();
        }
        else
        {
            Destroy(gameObject);
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
}
