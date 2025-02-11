using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class Dialog : MonoBehaviour
{
    [SerializeField] public String[] dialog;
    [SerializeField] public string characterName;
    [SerializeField] public Sprite characterAvatar;
    [SerializeField] public TMP_ColorGradient textColor;
}
