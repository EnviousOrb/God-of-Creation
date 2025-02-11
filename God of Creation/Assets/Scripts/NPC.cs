using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class NPC : MonoBehaviour
{
    [SerializeField] DialogSystem dialogBox;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            dialogBox.gameObject.SetActive(true);
            dialogBox.StartDialog();
        }
    }
}
