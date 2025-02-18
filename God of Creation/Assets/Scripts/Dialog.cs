using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class Dialog : MonoBehaviour
{
    [SerializeField] public string[] dialog; //Holds all the dialog that a character will say
    [HideInInspector] public string characterName; //The name of the character that is speaking
    [HideInInspector] public Sprite characterAvatar; //The avatar of the character that is speaking
    [HideInInspector] public TMP_ColorGradient textColor; //The color of the text

    void Start()
    {
        if(gameObject.tag == "Opponent")
        {
            characterName = GetComponent<NPC>().npcName;
            characterAvatar = GetComponent<NPC>().npcIcon;
            textColor = GetComponent<NPC>().npcTextColor;
        }

    }
}


