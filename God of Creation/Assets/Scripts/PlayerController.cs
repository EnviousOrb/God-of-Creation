using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float speed;
    private Animator animator;
    private Camera cam;
    private float moveDir;
    private NPC currentNPC;

    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(IsDialogActive())
        {
            moveDir = 0;
            return;
        }
        HandleMovementInput();
        animator.SetFloat("HeroSpeed", Mathf.Abs(moveDir));
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleMovementInput()
    {
        moveDir = Input.GetAxisRaw("Horizontal");
        if(moveDir != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveDir) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void MovePlayer()
    {
        transform.position += speed * Time.deltaTime * new Vector3(moveDir, 0, 0);
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
        {
            collision.GetComponent<NPC>().Interactable.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
        {
            currentNPC = collision.GetComponent<NPC>();

            if (Input.GetKey(KeyCode.E))
            {
                if(currentNPC.wasSpared)
                {
                    currentNPC.SpareDialog.SetActive(true);
                }
                else
                {
                    currentNPC.DialogBox.SetActive(true);
                }

                if (currentNPC.isFightable && !currentNPC.wasSpared)
                {
                    StartCoroutine(currentNPC.IntroToBattleSequence());
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
        {
            collision.GetComponent<NPC>().Interactable.SetActive(false);
            currentNPC = null;
        }
    }

    private bool IsDialogActive()
    {
        return currentNPC != null && currentNPC.DialogBox.activeSelf;
    }

    public void TogglePlayerControl()
    {
        moveDir = 0;
        enabled = !enabled;
    }
}
