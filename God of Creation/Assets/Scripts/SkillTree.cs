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
    [SerializeField] private TextMeshProUGUI SkillTreePathDisplayArea;
    [SerializeField] private Image[] skillIconAreas;
    public GameObject skillTreePathMenu;
    public GameObject closeButton;
    [SerializeField] private Button skillPath1Button;
    [SerializeField] private Button SkillPath2Button;
    [SerializeField] private Button SoulButton;

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

        var eventTrigger1 = skillPath1Button.gameObject.AddComponent<EventTrigger>();
        var pointerClick1 = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerClick1.callback.AddListener((data) => { OnButtonHovered(skillPath1Button); });
        eventTrigger1.triggers.Add(pointerClick1);

        var eventTrigger2 = SkillPath2Button.gameObject.AddComponent<EventTrigger>();
        var pointerClick2 = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerClick2.callback.AddListener((data) => { OnButtonHovered(SkillPath2Button); });
        eventTrigger2.triggers.Add(pointerClick2);

        var eventTrigger3 = SoulButton.gameObject.AddComponent<EventTrigger>();
        var pointerClick3 = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerClick3.callback.AddListener((data) => { OnSoulButtonHovered(); });
        eventTrigger3.triggers.Add(pointerClick3);
    }

    private void OnSkillHovered(Image skillIcon)
    {
        var skill = GetSkillFromIcon(skillIcon);
        if (skill != null)
        {
            SkillTreeDescriptionDisplayArea.colorGradientPreset = null;
            if (skill.IsUnlocked)
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

    private void OnButtonHovered(Button button)
    {
        var skillPath = GetSkillPathFromButton(button);

        if (skillPath != null)
        {
            SkillTreePathDisplayArea.text = skillPath.SkillPathDescription;
        }
    }

    private void OnSoulButtonHovered()
    {
        if (GameManager.Instance.Currenthero.isSoulSkillFound)
        {
            SkillTreeUpgradeNameDisplayArea.text = GameManager.Instance.Currenthero.soulSkill.SkillName;
            SkillTreeDescriptionDisplayArea.colorGradientPreset = GameManager.Instance.Currenthero.heroTextColor;
            SkillTreeDescriptionDisplayArea.text = GameManager.Instance.Currenthero.soulSkill.SkillDescription;
            SkillTreeUpgradePriceDisplayArea.text = "";
        }
        else
        {
            SkillTreeUpgradeNameDisplayArea.text = GameManager.Instance.Currenthero.heroName + "'s Soul Skill";
            SkillTreeDescriptionDisplayArea.colorGradientPreset = null;
            SkillTreeDescriptionDisplayArea.text = GameManager.Instance.Currenthero.soulSkill.SkillPathDescription;
            SkillTreeUpgradePriceDisplayArea.text = "";
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
        if (Index >= 0 && Index < GameManager.Instance.Currenthero.heroSkills.Count)
        {
            return GameManager.Instance.Currenthero.heroSkills[Index];
        }
        return null;
    }

    private Skill GetSkillPathFromButton(Button button)
    {
        if (button == skillPath1Button)
        {
            return GameManager.Instance.Currenthero.heroSkills.Find(skill => skill.name == "UpgradeA");
        }
        else if (button == SkillPath2Button)
        {
            return GameManager.Instance.Currenthero.heroSkills.Find(skill => skill.name == "UpgradeB");
        }
        return null;
    }

    public bool UnlockSkill(string skillName, HeroStats hero)
    {
        var Skill = hero.heroSkills.Find(skill => skill.SkillName == skillName);
        if (Skill != null && !Skill.IsUnlocked)
        {
            if (Skill.PrequisiteSkill != null && !Skill.PrequisiteSkill.IsUnlocked)
            {
                SkillTreeUpgradeNameDisplayArea.text = "";
                SkillTreeDescriptionDisplayArea.text = "Prerequisite Skill Not Unlocked!";
                SkillTreeUpgradePriceDisplayArea.text = "";
                return false;
            }

            if (hero.credixAmount <= Skill.SkillCost)
            {
                SkillTreeUpgradeNameDisplayArea.text = "";
                SkillTreeDescriptionDisplayArea.text = "Not Enough Credix!";
                SkillTreeUpgradePriceDisplayArea.text = "";
                return false;
            }

            hero.credixAmount -= Skill.SkillCost;
            Skill.IsUnlocked = true;
            hero.SaveHeroData();
            if (Skill.name == "UpgradeA" || Skill.name == "UpgradeB")
            {
                Skill UpgradeA = hero.heroSkills.Find(skill => skill.name == "UpgradeA");
                Skill UpgradeB = hero.heroSkills.Find(skill => skill.name == "UpgradeB");
                if (UpgradeA.IsUnlocked && UpgradeB.IsUnlocked)
                {
                    skillTreePathMenu.SetActive(true);
                    skillPath1Button.image.sprite = UpgradeA.SkillPathIcon;
                    SkillPath2Button.image.sprite = UpgradeB.SkillPathIcon;
                }
            }
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
                skillIconAreas[i].color = hero.heroSkills[i].IsUnlocked ? Color.white : Color.grey;
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
        UpdateSkillIcons();
    }

    public void OnSoulClick()
    {
        if(GameManager.Instance.Currenthero.isSoulSkillFound)
        {
            GameManager.Instance.Currenthero.soulInfo.SetActive(true);
            closeButton.SetActive(true);
        }
    }

    public void OnCloseClick()
    {
        GameManager.Instance.Currenthero.soulInfo.SetActive(false);
        closeButton.SetActive(false);
    }

    private void UpdateSkillIcons()
    {
        foreach (var skillIcon in skillIconAreas)
        {
            var skill = GetSkillFromIcon(skillIcon);
            if (skill != null)
            {
                skillIcon.color = skill.IsUnlocked ? Color.white : Color.grey;
            }
        }
    }

    public void OnSkillPathSelected(int path)
    {
        if (path == 1)
        {
            for (int i = 5; i <= 7; i++)
            {
                skillIconAreas[i].gameObject.SetActive(false);
            }
            skillTreePathMenu.SetActive(false);
            GameManager.Instance.Currenthero.chosenPath = 1;
        }
        else if (path == 2)
        {
            for (int i = 2; i <= 4; i++)
            {
                skillIconAreas[i].gameObject.SetActive(false);
            }
            skillTreePathMenu.SetActive(false);
            GameManager.Instance.Currenthero.chosenPath = 2;
        }
        GameManager.Instance.Currenthero.SaveHeroData();
    }

    public void ResetSkillTreePaths()
    {
        for (int i = 0; i < skillIconAreas.Length; i++)
        {
            skillIconAreas[i].gameObject.SetActive(true);
        }
    }

    public void OnCharacterSelect(HeroStats hero)
    {
        ResetSkillTreePaths();
        DisplaySkillTreeName(hero);
        DisplaySkillIcons(hero);

        if (hero.chosenPath == 1)
        {
            for (int i = 5; i <= 7; i++)
            {
                skillIconAreas[i].gameObject.SetActive(false);
            }
        }
        else if (hero.chosenPath == 2)
        {
            for (int i = 2; i <= 4; i++)
            {
                skillIconAreas[i].gameObject.SetActive(false);
            }
        }
    }
    public void UpdateSoulButton(HeroStats currenthero)
    {
        if (currenthero.isSoulSkillFound)
            SoulButton.GetComponent<Image>().sprite = currenthero.soulSkill.SkillIcon;
    }
}