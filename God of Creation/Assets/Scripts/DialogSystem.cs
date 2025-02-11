using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] TextMeshProUGUI characterNameDisplay;
    [SerializeField] Image avatarDisplay;
    [SerializeField] Dialog[] dialogs;
    private string[] sentences;
    [SerializeField] float typingSpeed = 0.02f;

    private int index;
    private int dialogIndex;

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

    public void SetDialog(Dialog dialogs)
    {
        sentences = dialogs.dialog;
        textDisplay.colorGradientPreset = dialogs.textColor;

        characterNameDisplay.text = dialogs.characterName;
        characterNameDisplay.colorGradientPreset = dialogs.textColor;

        avatarDisplay.sprite = dialogs.characterAvatar;
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
        dialogIndex = 0;
        ClearDialog();
        SetDialog(dialogs[dialogIndex]);
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
        else if (dialogIndex < dialogs.Length - 1)
        {
            dialogIndex++;
            index = 0;
            ClearDialog();
            SetDialog(dialogs[dialogIndex]);
            StartCoroutine(Type());
        }
        else
        {
            ClearDialog();
            gameObject.SetActive(false);
        }
    }
}
