using System.Collections;
using DialogSystem;
using UnityEngine;

public class SoulSkill : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public float speed;
    public int heroIdToFind;
    public GameObject dialogBox;
    public Dialog finalDialog;
    public GameObject soulSkillAnim;
    public GameObject interactable;
    private float elapsedTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != endPos)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime * speed);
        }
        else
        {
            elapsedTime = 0;
            (endPos, startPos) = (startPos, endPos);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           interactable.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(GameManager.Instance.Currenthero.HeroID != heroIdToFind)
            {
                return;
            }

            if (Input.GetKey(KeyCode.E))
            {
                dialogBox.SetActive(true);
                collision.GetComponent<PlayerController>().TogglePlayerControl();
                StartCoroutine(TriggerSkillCutscene());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactable.SetActive(false);
        }
    }

    private IEnumerator TriggerSkillCutscene()
    {
        yield return new WaitUntil(() => finalDialog.IsFinished);
        soulSkillAnim.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        GameManager.Instance.Currenthero.GetComponent<PlayerController>().enabled = true;
        GameManager.Instance.Currenthero.isSoulSkillFound = true;
        GameManager.Instance.Currenthero.UpgradeHero();
        GameManager.Instance.Currenthero.SaveHeroData();
        GameManager.Instance.Currenthero.LoadHeroData();
        soulSkillAnim.SetActive(false);
        Destroy(gameObject);
    }
}