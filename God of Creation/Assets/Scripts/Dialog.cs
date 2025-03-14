using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace DialogSystem
{
    public class Dialog : DialogBase
    {
        [Header("Dialog Settings")]
        public string dialogText;
        public TMP_ColorGradient textColor;

        [Header("Text Settings")]
        public float textDelay;

        [Header("Sound Settings")]
        public AudioClip textSound;

        [Header("Character Settings")]
        public Sprite avatarSprite;
        public Image avatarDisplay;
        public string characterName;
        public TextMeshProUGUI nameDisplay;

        public bool IsHeroDialog;
        public bool UseHeroOpeningLine;

        private TextMeshProUGUI textDisplay;

        private IEnumerator writeText;

        private void Awake()
        {
            textDisplay = GetComponent<TextMeshProUGUI>();
            ResetLine();
            avatarDisplay.sprite = null;
            nameDisplay.text = "";
        }
        private void OnEnable()
        {
            ResetLine();
            if (IsHeroDialog)
            {
                var hero = GameManager.Instance.Currenthero;
                avatarDisplay.sprite = hero.heroIcon;
                nameDisplay.text = hero.heroName;
                textColor = hero.heroTextColor;
                textSound = hero.heroTextSound;

                if(UseHeroOpeningLine)
                    dialogText = hero.heroOpeningDialog;
            }
            else
            {
                avatarDisplay.sprite = avatarSprite;
                nameDisplay.text = characterName;
            }

            writeText = WriteText(dialogText, textDisplay, textColor, textDelay, textSound);
            StartCoroutine(writeText);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (textDisplay.text != dialogText)
                {
                    StopCoroutine(writeText);
                    textDisplay.text = dialogText;
                }
                else
                    IsFinished = true;
            }
        }

        private void ResetLine()
        {
            textDisplay.text = "";
            IsFinished = false;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Dialog))]
    public class DialogEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Dialog dialog = (Dialog)target;

            dialog.dialogText = UnityEditor.EditorGUILayout.TextArea(dialog.dialogText, GUILayout.Height(100));
            dialog.textDelay = UnityEditor.EditorGUILayout.FloatField("Text Delay", dialog.textDelay);
            dialog.avatarDisplay = (Image)UnityEditor.EditorGUILayout.ObjectField("Avatar Display", dialog.avatarDisplay, typeof(Image), true);
            dialog.nameDisplay = (TextMeshProUGUI)UnityEditor.EditorGUILayout.ObjectField("Name Display", dialog.nameDisplay, typeof(TextMeshProUGUI), true);
            dialog.IsHeroDialog = UnityEditor.EditorGUILayout.Toggle("Is Hero Dialog?", dialog.IsHeroDialog);
            dialog.UseHeroOpeningLine = UnityEditor.EditorGUILayout.Toggle("Use Hero Opening Line?", dialog.UseHeroOpeningLine);
            if (dialog.IsHeroDialog)
            {
                UnityEditor.EditorGUILayout.HelpBox("The avatar, text color, name, and sound will be set to the current hero at play", UnityEditor.MessageType.Info);
            }
            else
            {
                dialog.characterName = UnityEditor.EditorGUILayout.TextField("Character Name", dialog.characterName);
                dialog.avatarSprite = (Sprite)UnityEditor.EditorGUILayout.ObjectField("Avatar Sprite", dialog.avatarSprite, typeof(Sprite), false);
                dialog.textSound = (AudioClip)UnityEditor.EditorGUILayout.ObjectField("Text Sound", dialog.textSound, typeof(AudioClip), false);
                dialog.textColor = (TMP_ColorGradient)UnityEditor.EditorGUILayout.ObjectField("Text Color", dialog.textColor, typeof(TMP_ColorGradient), false);
            }
        }
    }
#endif
}