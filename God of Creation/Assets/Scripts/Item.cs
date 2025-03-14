using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string ItemName;
    public string ItemDescription;
    public string PictureID;
    public int ItemCost;
    public int ItemCount;
    public int MaxStackCount;
    public Sprite ItemIcon;
    public System.Action<HeroStats> ApplyEffect;
    [HideInInspector] public bool IsSoldOut;
    [HideInInspector] public float Cooldown;
}