using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections;

public class NPC : MonoBehaviour
{
    //Overworld visuals/Generic set-up
    [SerializeField] public GameObject DialogBox;
    public Sprite npcIcon;
    public string npcName;
    public TMP_ColorGradient npcTextColor; //The color of the npc/opponent's dialog text

    public bool isFightable;

    [Header("Opponent visuals/stats")]
    [SerializeField] public Sprite opponentIcon;
    public string[] opponentFlavorTexts; //The text that appears right as soon as a battle begins

    public GameObject opponentAttack;
    public GameObject opponentHeal;

    //Opponent stats
    [Range(0, 25)] public int opponentLevel;
    [Range(0, 100)] public int maxHealth;
    [Range(0, 100)] public int opponentDamage;
    [Range(0, 100)] public float opponentSpeed;
    [Range(0, 100)] public int opponentDefense;

    [Header("Opponent Audio")]
    public AudioClip opponentAttackSound;
    public AudioClip opponentHealSound;

    [HideInInspector] public int currentHealth;

    private void Start()
    {
        if(GameManager.Instance != null)
        {
            if (GameManager.Instance.IsOpponentDefeated(this))
            {
               //Destroy(gameObject);
            }
        }
    }

    public void StartDialog()
    {
        DialogBox.SetActive(true);
    }

    public bool IsDialogActive()
    {
        return DialogBox.activeSelf;
    }

    public void TakeDamage(HeroStats heroStats)
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            //Handle death stuff for enemy here
            return;
        }
        int damage = heroStats.attack * (1 + (heroStats.Level / 10)) - opponentDefense;
        damage = Mathf.Max(1, damage);
        currentHealth -= damage;
    }
    public void TakeDamage(int damageTaken)
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            //Handle death stuff
        }

        int damage = damageTaken - opponentDefense;
        damage = Mathf.Max(1, damage);
        currentHealth -= damage;
    }
    public IEnumerator IntroToBattleSequence()
    {
        yield return new WaitUntil(() => !IsDialogActive());

        GameManager.Instance.CurrentOpponent = this;
        SceneManager.LoadScene("BattleScene");
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(NPC))]
public class NPCEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NPC npc = (NPC)target;
        npc.npcName = EditorGUILayout.TextField("NPC Name", npc.npcName);
        npc.npcIcon = (Sprite)EditorGUILayout.ObjectField("NPC Icon", npc.npcIcon, typeof(Sprite), false);
        npc.npcTextColor = (TMP_ColorGradient)EditorGUILayout.ObjectField("NPC Text Color", npc.npcTextColor, typeof(TMP_ColorGradient), false);
        npc.DialogBox = (GameObject)EditorGUILayout.ObjectField("Dialog Box", npc.DialogBox, typeof(GameObject), true);
        npc.isFightable = EditorGUILayout.Toggle("Is Fightable", npc.isFightable);
        if (npc.isFightable)
        {
            npc.opponentIcon = (Sprite)EditorGUILayout.ObjectField("Opponent Icon", npc.opponentIcon, typeof(Sprite), false);
            SerializedProperty opponentFlavorTextsProperty = serializedObject.FindProperty("opponentFlavorTexts");
            EditorGUILayout.PropertyField(opponentFlavorTextsProperty, new GUIContent("Opponent Flavor Texts"), true);
            npc.opponentAttack = (GameObject)EditorGUILayout.ObjectField("Opponent Attack", npc.opponentAttack, typeof(GameObject), false);
            npc.opponentHeal = (GameObject)EditorGUILayout.ObjectField("Opponent Heal", npc.opponentHeal, typeof(GameObject), false);
            npc.opponentLevel = EditorGUILayout.IntSlider("Opponent Level", npc.opponentLevel, 0, 25);
            npc.maxHealth = EditorGUILayout.IntSlider("Max Health", npc.maxHealth, 0, 100);
            npc.opponentDamage = EditorGUILayout.IntSlider("Opponent Damage", npc.opponentDamage, 0, 100);
            npc.opponentSpeed = EditorGUILayout.Slider("Opponent Speed", npc.opponentSpeed, 0, 100);
            npc.opponentDefense = EditorGUILayout.IntSlider("Opponent Defense", npc.opponentDefense, 0, 100);
            npc.opponentAttackSound = (AudioClip)EditorGUILayout.ObjectField("Opponent Attack Sound", npc.opponentAttackSound, typeof(AudioClip), false);
            npc.opponentHealSound = (AudioClip)EditorGUILayout.ObjectField("Opponent Heal Sound", npc.opponentHealSound, typeof(AudioClip), false);
        }
        serializedObject.ApplyModifiedProperties();
    }
}

#endif
