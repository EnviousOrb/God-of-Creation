using UnityEngine;

public class Skill : MonoBehaviour
{
    public string SkillName;
    public string SkillDescription;
    public Skill PrequisiteSkill;
    [HideInInspector] public bool IsUnlocked;
    public int SkillCost;
    public Sprite SkillIcon;
    public Sprite SkillPathIcon;
    public string SkillPathDescription;
    public System.Action<HeroStats, NPC> ApplyEffect;
}