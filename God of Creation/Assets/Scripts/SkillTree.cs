using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillTree : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI SkillTreeNameDisplayArea;
    [SerializeField] private TextMeshProUGUI SkillTreeDescriptionDisplayArea;
    [SerializeField] private TextMeshProUGUI SkillTreeUpgradeNameDisplayArea;
    [SerializeField] private TextMeshProUGUI SkillTreeUpgradePriceDisplayArea;
    [SerializeField] private Image[] skillIconAreas;

    private void Start()
    {
        foreach (var skillIcon in skillIconAreas)
        {
            var eventTrigger = skillIcon.gameObject.AddComponent<EventTrigger>();
            var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnter.callback.AddListener((data) => { OnSkillHovered(skillIcon); });
            eventTrigger.triggers.Add(pointerEnter);

            var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExit.callback.AddListener((data) => { OnSkillUnhovered(); });
            eventTrigger.triggers.Add(pointerExit);
        }
    }

    private void OnSkillHovered(Image skillIcon)
    {
        var skill = GetSkillFromIcon(skillIcon);
        if (skill != null)
        {
            if(skill.IsUnlocked)
            {
                SkillTreeUpgradeNameDisplayArea.text = skill.SkillName + " (Unlocked)";
                SkillTreeDescriptionDisplayArea.text = skill.SkillDescription;
                SkillTreeUpgradePriceDisplayArea.text = "";
                return;
            }
            SkillTreeUpgradeNameDisplayArea.text = skill.SkillName;
            SkillTreeDescriptionDisplayArea.text = skill.SkillDescription;
            SkillTreeUpgradePriceDisplayArea.text = "Cost: " + skill.SkillCost + " Credix";
        }
    }

    private void OnSkillUnhovered()
    {
        SkillTreeUpgradeNameDisplayArea.text = "";
        SkillTreeDescriptionDisplayArea.text = "";
        SkillTreeUpgradePriceDisplayArea.text = "";
    }

    private Skill GetSkillFromIcon(Image skillIcon)
    {
       int Index = System.Array.IndexOf(skillIconAreas, skillIcon);
       if(Index >= 0 && Index < GameManager.Instance.Currenthero.heroSkills.Count)
       {
           return GameManager.Instance.Currenthero.heroSkills[Index];
       }
       return null;
    }

    public bool UnlockSkill(string skillName, HeroStats hero) 
    {
        var Skill = hero.heroSkills.Find(skill => skill.SkillName == skillName);
        if (Skill != null && !Skill.IsUnlocked)
        {
            if(hero.credixAmount <= Skill.SkillCost)
            {
                SkillTreeUpgradeNameDisplayArea.text = "";
                SkillTreeDescriptionDisplayArea.text = "Not Enough Credix!";
                SkillTreeUpgradePriceDisplayArea.text = "";
                return false;
            }

            hero.credixAmount -= Skill.SkillCost;
            Skill.IsUnlocked = true;
            hero.SaveHeroData();
            return true;
        }
        return false;
    }

    public void DisplaySkillTreeName(HeroStats hero)
    {
        SkillTreeNameDisplayArea.text = hero.heroName + "'s Skill Tree";
    }

    public void DisplaySkillIcons(HeroStats hero)
    {
        for (int i = 0; i < skillIconAreas.Length; i++)
        {
            if (i < hero.heroSkills.Count)
            {
                skillIconAreas[i].sprite = hero.heroSkills[i].SkillIcon;
            }
            else
            {
                skillIconAreas[i].sprite = null;
            }
        }
    }

    public void OnSkillBuy()
    {
        var skillName = SkillTreeUpgradeNameDisplayArea.text;
        if (UnlockSkill(skillName, GameManager.Instance.Currenthero))
        {
            SkillTreeUpgradeNameDisplayArea.text = "";
            SkillTreeDescriptionDisplayArea.text = "Sold!";
            SkillTreeUpgradePriceDisplayArea.text = "";
        }
    }
}
