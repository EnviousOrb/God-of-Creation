using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroUI : MonoBehaviour
{
    [Header("Hero UI Elements")]
    [SerializeField] Image heroSprite;
    [SerializeField] Image typeSprite;
    [SerializeField] Slider heroHealth;
    [SerializeField] Slider heroHeat;
    [SerializeField] TextMeshProUGUI heroName;
    [SerializeField] TextMeshProUGUI heroLevel;
    [SerializeField] TextMeshProUGUI heroCredix;

    private HeroStats heroStats;
    void Start()
    {
        heroStats = GameManager.Instance.Currenthero;
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
