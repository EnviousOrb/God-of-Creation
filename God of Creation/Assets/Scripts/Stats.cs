using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [SerializeField] private GameObject _namePlace;
    [SerializeField] private GameObject _bioPlace;
    [SerializeField] private GameObject _levelPlace;
    [SerializeField] private GameObject _speedPlace;
    [SerializeField] private GameObject _attackPlace;
    [SerializeField] private GameObject _defensePlace;
    [SerializeField] private GameObject _maxHealthPlace;
    [SerializeField] private GameObject _maxHeatPlace;
    [SerializeField] private GameObject _currentXPPlace;
    [SerializeField] private GameObject statsSpritePlace;
    [SerializeField] private GameObject typeSpritePlace;

    public void SetStats(HeroStats hero)
    {
        _namePlace.GetComponent<TextMeshProUGUI>().text = hero.heroName;
        _bioPlace.GetComponent<TextMeshProUGUI>().text = hero.heroBio;
        _levelPlace.GetComponent<TextMeshProUGUI>().text = hero.Level.ToString();
        _speedPlace.GetComponent<TextMeshProUGUI>().text = hero.speed.ToString();
        _attackPlace.GetComponent<TextMeshProUGUI>().text = hero.attack.ToString();
        _defensePlace.GetComponent<TextMeshProUGUI>().text = hero.defense.ToString();
        _maxHealthPlace.GetComponent<TextMeshProUGUI>().text = hero.maxHealth.ToString();
        _maxHeatPlace.GetComponent<TextMeshProUGUI>().text = hero.maxHeat.ToString();
        _currentXPPlace.GetComponent<TextMeshProUGUI>().text = hero.xpToLevel.ToString();
        statsSpritePlace.GetComponent<Image>().sprite = hero.StatsIcon;
        typeSpritePlace.GetComponent<Image>().sprite = hero.typeIcon;
    }
}
