using TMPro;
using UnityEngine;

public class HeroStats : MonoBehaviour
{
    [Header("Hero Stats")]
    public string heroName;
    [Range(0, 1)] public float heroHealth;

    [Header("Hero Icons")]
    public Sprite heroIcon;
    public Sprite typeIcon;

    [Header("Hero Color")]
    public TMP_ColorGradient heroTextColor;
}
