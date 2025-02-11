using System;
using TMPro;
using UnityEngine;

public class HeroUI : MonoBehaviour
{

    [SerializeField] SpriteRenderer heroSprite;
    [SerializeField] SpriteRenderer typeSprite;
    [SerializeField] TextMeshProUGUI heroName;
    [SerializeField] HeroStats heroStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetHeroUI(heroStats);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetHeroUI(HeroStats heroStats)
    {
        heroName.text = heroStats.heroName;
        heroName.colorGradientPreset = heroStats.heroTextColor;
        heroSprite.sprite = heroStats.heroIcon;
        typeSprite.sprite = heroStats.typeIcon;
    }
}
