using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string ItemName;
    public string ItemDescription;
    public int ItemCost;
    public int ItemCount;
    public Sprite ItemIcon;
    public System.Action<HeroStats> ApplyEffect;
    public int MaxStackCount;
    [HideInInspector] public bool IsSoldOut;
    [HideInInspector] public float Cooldown;
}