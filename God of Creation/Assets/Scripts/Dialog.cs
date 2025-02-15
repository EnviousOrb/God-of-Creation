using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class Dialog : MonoBehaviour
{
    [SerializeField] public String[] dialog; //Holds all the dialog that a character will say
    [SerializeField] public string characterName; //The name of the character that is speaking
    [SerializeField] public Sprite characterAvatar; //The avatar of the character that is speaking
    [SerializeField] public TMP_ColorGradient textColor; //The color of the text
}
