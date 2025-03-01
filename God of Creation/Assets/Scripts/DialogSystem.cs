using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplay; //This is where the dialog will be displayed on the screen
    [SerializeField] TextMeshProUGUI characterNameDisplay; //This is where the character's name will be displayed on screen
    [SerializeField] Image avatarDisplay; //This is where the character's avatar will be displayed on screen
    [SerializeField] float typingSpeed = 0.02f; //The speed at which the dialog will be displayed on screen
    public bool IsDialogFinished { get { return gameObject.activeSelf == false; } }

    [HideInInspector] public Dialog[] dialogs; //Holds all of the dialog that happens in one scene
    private string[] sentences; //Splits the dialog into two categories, for mutliple speakers
    private int index; //The index of the dialog
    private int dialogIndex; //The index of the dialog array

    private Coroutine textCoroutine;

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

    //This function sets the dialog for the current scene
    public void SetDialog(Dialog dialogs)
    {
        sentences = dialogs.dialog;
        textDisplay.colorGradientPreset = dialogs.textColor;

        characterNameDisplay.text = dialogs.characterName;

        avatarDisplay.sprite = dialogs.characterAvatar;
    }

    //This function clears the dialog from the screen
    void ClearDialog()
    {
        textDisplay.text = "";
        characterNameDisplay.text = "";
        avatarDisplay.sprite = null;
    }

    //This function starts the dialog
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
        if(textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }
        textCoroutine = StartCoroutine(TypeCoroutine());
        yield return textCoroutine;
    }

    //This function types out the dialog on the screen
    IEnumerator TypeCoroutine()
    {
        //This foreach loop goes through each letter in the dialog and displays it on the screen
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    //This function moves to the next line of dialog
    void NextLine()
    {
        //If the index is less than the length of the sentences array, increment the index
        if(index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else if (dialogIndex < dialogs.Length - 1) //If the dialogIndex is less than the length of the dialogs array, increment the dialogIndex
        {
            dialogIndex++;
            index = 0;
            ClearDialog();
            SetDialog(dialogs[dialogIndex]);
            StartCoroutine(Type());
        }
        else //If the dialogIndex is greater than the length of the dialogs array, clear the dialog and set the gameObject to false
        {
            ClearDialog();
            gameObject.SetActive(false);
        }
    }
}
