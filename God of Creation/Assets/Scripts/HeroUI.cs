using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroUI : MonoBehaviour
{
    [SerializeField] Image heroSprite; //The location that the hero's sprite will be displayed on the main UI
    [SerializeField] Image typeSprite; //The location that the hero's type sprite will be displayed on the main UI
    [SerializeField] TextMeshProUGUI heroName; //The location that the hero's name will be displayed on the main UI
    [SerializeField] Slider heroHealth; //The location that the hero's health will be displayed on the main UI
    [SerializeField] Slider heroHeat; //The location that the hero's heat will be displayed on the main UI
    [SerializeField] TextMeshProUGUI heroLevel; //The location that the hero's level will be displayed on the main UI
    [SerializeField] TextMeshProUGUI heroCredix; //The location that the hero's credix will be displayed on the main UI

    private HeroStats heroStats; //The hero's stats
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        heroStats = FindAnyObjectByType<HeroStats>();
        SetOverworldUI(heroStats);
    }

    public void SetOverworldUI(HeroStats heroStats)
    {
        heroName.text = heroStats.heroName;
        heroHealth.maxValue = heroStats.maxHealth;
        heroHeat.maxValue = heroStats.maxHeat;
        heroLevel.text = heroStats.Level.ToString();
        heroCredix.text = heroStats.credixAmount.ToString();
        heroSprite.sprite = heroStats.heroIcon;
        typeSprite.sprite = heroStats.typeIcon;
        UpdateOverworldUI(heroStats);
    }

    public void UpdateOverworldUI(HeroStats heroStats)
    {
        heroHealth.value = heroStats.currentHealth;
        heroHeat.value = heroStats.currentHeat;
        heroCredix.text = heroStats.credixAmount.ToString();
    }
}
