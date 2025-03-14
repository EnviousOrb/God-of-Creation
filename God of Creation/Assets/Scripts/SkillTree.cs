using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillTree : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillTreeNameDisplayArea;
    [SerializeField] private TextMeshProUGUI skillTreeDescriptionDisplayArea;
    [SerializeField] private TextMeshProUGUI skillTreeUpgradeNameDisplayArea;
    [SerializeField] private TextMeshProUGUI skillTreeUpgradePriceDisplayArea;
    [SerializeField] private TextMeshProUGUI skillTreePathDisplayArea;
    [SerializeField] private Image[] skillIconAreas;
    [SerializeField] public GameObject skillTreePathMenu;
    [SerializeField] private Button skillPath1Button;
    [SerializeField] private Button skillPath2Button;

    private void Start()
    {
        InitalizeSkillIcons();

        InitalizeSkilPath();
    }

    #region Public Methods
    public void ResetSkillTreePaths()
    {
        foreach (var skillIcon in skillIconAreas)
        {
            skillIcon.gameObject.SetActive(true);
        }
    }

    public void OnCharacterSelect(HeroStats hero)
    {
        ResetSkillTreePaths();
        DisplaySkillTreeName(hero);
        DisplaySkillIcons(hero);

        if (hero.chosenPath == 1)
        {
            SetSkillIconsActive(5, 7, false);
        }
        else if (hero.chosenPath == 2)
        {
            SetSkillIconsActive(2, 4, false);
        }
    }

    public void OnSkillPathSelected(int path)
    {
        if (path == 1)
        {
            SetSkillIconsActive(5, 7, false);
            GameManager.Instance.Currenthero.chosenPath = 1;
        }
        else if (path == 2)
        {
            SetSkillIconsActive(2, 4, false);
            GameManager.Instance.Currenthero.chosenPath = 2;
        }
        skillTreePathMenu.SetActive(false);
        GameManager.Instance.Currenthero.SaveHeroData();
    }

    public void DisplaySkillTreeName(HeroStats hero)
    {
        skillTreeNameDisplayArea.text = hero.heroName + "'s Skill Tree";
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
        var skillName = skillTreeUpgradeNameDisplayArea.text;
        if (UnlockSkill(skillName, GameManager.Instance.Currenthero))
        {
            DisplayMessage("Skill Unlocked!");
        }
        UpdateSkillIcons();
    }

    public bool UnlockSkill(string skillName, HeroStats hero)
    {
        var Skill = hero.heroSkills.Find(skill => skill.SkillName == skillName);
        if (Skill != null && !Skill.IsUnlocked)
        {
            if (Skill.PrequisiteSkill != null && !Skill.PrequisiteSkill.IsUnlocked)
            {
                DisplayMessage("Prerequisite Skill Not Unlocked!");
                return false;
            }

            if (hero.credixAmount <= Skill.SkillCost)
            {
                DisplayMessage("Not Enough Credix!");
                return false;
            }

            hero.credixAmount -= Skill.SkillCost;
            Skill.IsUnlocked = true;
            hero.SaveHeroData();
            UpdateSkillPathMenu(hero);
            return true;
        }
        return false;
    }

    #endregion

    #region Event Handlers
    private void OnSkillHovered(Image skillIcon)
    {
        var skill = GetSkillFromIcon(skillIcon);
        if (skill != null)
        {
            skillTreeUpgradeNameDisplayArea.text = skill.SkillName + (skill.IsUnlocked ? " (Unlocked)" : "");
            skillTreeDescriptionDisplayArea.text = skill.SkillDescription;
            skillTreeUpgradePriceDisplayArea.text = skill.IsUnlocked ? "" : "Cost: " + skill.SkillCost + " Credix";
        }
    }

    private void OnButtonHovered(Button button)
    {
        var skillPath = GetSkillPathFromButton(button);
        if (skillPath != null)
        {
            skillTreePathDisplayArea.text = skillPath.SkillPathDescription;
        }
    }

    private void OnSkillUnhovered()
    {
        skillTreeUpgradeNameDisplayArea.text = "";
        skillTreeDescriptionDisplayArea.text = "";
        skillTreeUpgradePriceDisplayArea.text = "";
    }

    #endregion

    #region Private Methods

    private void InitalizeSkillIcons()
    {
        foreach (var skillIcon in skillIconAreas)
        {
            AddEventTrigger(skillIcon.gameObject, EventTriggerType.PointerEnter, (data) => { OnSkillHovered(skillIcon); });
            AddEventTrigger(skillIcon.gameObject, EventTriggerType.PointerExit, (data) => { OnSkillUnhovered(); });
        }
    }

    private void InitalizeSkilPath()
    {
        AddEventTrigger(skillPath1Button.gameObject, EventTriggerType.PointerEnter, (data) => { OnButtonHovered(skillPath1Button); });
        AddEventTrigger(skillPath2Button.gameObject, EventTriggerType.PointerEnter, (data) => { OnButtonHovered(skillPath2Button); });
    }

    private void AddEventTrigger(GameObject gameObject, EventTriggerType eventTriggerType, System.Action<BaseEventData> action)
    {
        var eventTrigger = gameObject.AddComponent<EventTrigger>();
        var pointerClick = new EventTrigger.Entry { eventID = eventTriggerType };
        pointerClick.callback.AddListener((data) => { action(data); });
        eventTrigger.triggers.Add(pointerClick);
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

    private Skill GetSkillPathFromButton(Button button)
    {
        if (button == skillPath1Button)
        {
            return GameManager.Instance.Currenthero.heroSkills.Find(skill => skill.SkillName == "UpgradeA");
        }
        else if (button == skillPath2Button)
        {
            return GameManager.Instance.Currenthero.heroSkills.Find(skill => skill.SkillName == "UpgradeB");
        }
        return null;
    }

    private void DisplayMessage(string message)
    {
        skillTreeUpgradeNameDisplayArea.text = "";
        skillTreeDescriptionDisplayArea.text = message;
        skillTreeUpgradePriceDisplayArea.text = "";
    }

    private void UpdateSkillPathMenu(HeroStats hero)
    {
        var upgradeA = hero.heroSkills.Find(skill => skill.name == "UpgradeA");
        var upgradeB = hero.heroSkills.Find(skill => skill.name == "UpgradeB");

        if (upgradeA != null && upgradeB != null && upgradeA.IsUnlocked && upgradeB.IsUnlocked)
        {
            skillTreePathMenu.SetActive(true);
            skillPath1Button.image.sprite = upgradeA.SkillPathIcon;
            skillPath2Button.image.sprite = upgradeB.SkillPathIcon;
        }
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

    private void SetSkillIconsActive(int start, int end, bool active)
    {
        for (int i = start; i <= end; i++)
        {
            skillIconAreas[i].gameObject.SetActive(active);
        }
    }

    #endregion
}
