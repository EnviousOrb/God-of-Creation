using UnityEngine;

public class Item : MonoBehaviour
{
    public string ItemName;
    public string ItemDescription;
    public int ItemCost;
    public int ItemCount;
    public Sprite ItemIcon;
    public System.Action<HeroStats> ApplyEffect;
}