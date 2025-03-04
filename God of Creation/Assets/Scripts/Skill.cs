using UnityEngine;

public class Skill : MonoBehaviour
{
    public string SkillName;
    public string SkillDescription;
    public bool IsUnlocked;
    public int SkillCost;
    public Sprite SkillIcon;
    public System.Action<HeroStats, NPC> ApplyEffect;
}