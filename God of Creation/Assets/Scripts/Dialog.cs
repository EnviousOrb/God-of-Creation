using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Dialog : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplay;

    [SerializeField] TextMeshProUGUI characterNameDisplay;

    [SerializeField] Image avatarDisplay;
    private string[] sentences;
    [SerializeField] float typingSpeed = 0.02f;

    private int index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ClearDialog();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
            NextLine();
    }

    public void SetDialog(string[] dialog, string characterName, Sprite avatar)
    {
        sentences = dialog;

        characterNameDisplay.text = characterName;

        avatarDisplay.sprite = avatar;
    }

    void ClearDialog()
    {
        textDisplay.text = "";
        characterNameDisplay.text = "";
        avatarDisplay.sprite = null;
    }

    public void StartDialog()
    {
        index = 0;
        StartCoroutine(Type());
    }

    IEnumerator Type()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void NextLine()
    {
        if(index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            ClearDialog();
            gameObject.SetActive(false);

        }
    }
}
