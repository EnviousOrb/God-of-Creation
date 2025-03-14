using UnityEngine;

public class WheelButton : MonoBehaviour
{
    [SerializeField] private GameObject HeroToSpawn;
    private GameObject currentHero;

    public void SwapHero()
    {
        currentHero = GameObject.FindGameObjectWithTag("Player");
        if(currentHero == HeroToSpawn)
            return;

        GameObject newHero = Instantiate(HeroToSpawn, currentHero.transform.position, Quaternion.identity);
        newHero.name = HeroToSpawn.name;

        // Set the new hero as the current hero
        GameManager.Instance.Currenthero = newHero.GetComponent<HeroStats>();
        GameManager.Instance.Currenthero.LoadHeroData();

        // Destory the old hero
        Destroy(currentHero);
    }
}
