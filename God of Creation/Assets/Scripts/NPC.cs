using UnityEngine;
using UnityEngine.UI;


public class NPC : MonoBehaviour
{
    [SerializeField] string npcName;
    [SerializeField] Sprite avatar;
    [SerializeField] string[] dialog;
    [SerializeField] Dialog dialogBox;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            dialogBox.SetDialog(dialog, npcName, avatar);
            dialogBox.gameObject.SetActive(true);
            dialogBox.StartDialog();
        }
    }
}
