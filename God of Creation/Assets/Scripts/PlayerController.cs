using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float speed;
    private Animator animator;
    private Camera cam;
    private float moveDir;
    private NPC currentNPC;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsDialogActive())
        {
            moveDir = 0;
            return;
        }
        moveDir = Input.GetAxisRaw("Horizontal");
        if(moveDir > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveDir < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        animator.SetFloat("HeroSpeed", Mathf.Abs(moveDir));
    }

    void FixedUpdate()
    {
        transform.position += speed * Time.deltaTime * new Vector3(moveDir, 0, 0);
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
        {
            currentNPC = collision.GetComponent<NPC>();

            if (Input.GetKey(KeyCode.E))
            {
                currentNPC.StartDialog();
            }
        }
    }

    private bool IsDialogActive()
    {
        if(currentNPC)
            return currentNPC.IsDialogActive();
        else
            return false;
    }
}
