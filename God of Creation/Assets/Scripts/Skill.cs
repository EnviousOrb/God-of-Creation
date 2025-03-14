using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    public string SkillName;
    public string SkillDescription;
    public Skill PrequisiteSkill;
    public int SkillCost;
    public Sprite SkillIcon;
    public Sprite SkillPathIcon;
    public string SkillPathDescription;
    public System.Action<HeroStats, NPC> ApplyEffect;
    [HideInInspector] public bool IsUnlocked;
}