using UnityEngine;

public class WheelButton : MonoBehaviour
{
    [SerializeField] private GameObject HeroToSpawn;
    private GameObject currentHero;
    private HeroUI heroUI;

    private void Awake()
    {
        heroUI = FindAnyObjectByType<HeroUI>();
        if (heroUI == null)
        {
            Debug.LogError("HeroUI is not assigned!");
        }
    }

    public void SwapHero()
    {
        currentHero = GameObject.FindGameObjectWithTag("Player");
        if(currentHero == HeroToSpawn)
            return;

        GameObject newHero = Instantiate(HeroToSpawn, currentHero.transform.position, Quaternion.identity);
        newHero.name = HeroToSpawn.name;

        GameManager.Instance.Currenthero = newHero.GetComponent<HeroStats>();
        GameManager.Instance.Currenthero.LoadHeroData();
        Destroy(currentHero);

        heroUI.SetOverworldUI(GameManager.Instance.Currenthero);
        heroUI.UpdateOverworldUI(GameManager.Instance.Currenthero);
    }
}
