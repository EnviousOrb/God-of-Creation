using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    [SerializeField] public string[] dialog; //Holds all the dialog that a character will say
    [HideInInspector] public string characterName; //The name of the character that is speaking
    [HideInInspector] public Sprite characterAvatar; //The avatar of the character that is speaking
    [HideInInspector] public TMP_ColorGradient textColor; //The color of the text

    void Start()
    {
        if(gameObject.CompareTag("Player"))
        {
            characterName = gameObject.GetComponent<HeroStats>().heroName;
            characterAvatar = gameObject.GetComponent<HeroStats>().heroIcon;
            textColor = gameObject.GetComponent<HeroStats>().heroTextColor;
        }
        else if (gameObject.CompareTag("Opponent"))
        {
            characterName = gameObject.GetComponent<NPC>().npcName;
            characterAvatar = gameObject.GetComponent<NPC>().npcIcon;
            textColor = gameObject.GetComponent<NPC>().npcTextColor;
        }
    }
}