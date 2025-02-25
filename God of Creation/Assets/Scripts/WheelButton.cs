using UnityEngine;

public class WheelButton : MonoBehaviour
{
    [SerializeField] private GameObject HeroToSpawn;
    private GameObject currentHero;

    public void SwapHero()
    {
        currentHero = Instantiate(HeroToSpawn, currentHero.transform.position, Quaternion.identity);

        if (currentHero != null)
        {
            Destroy(currentHero);
        }
    }
}
