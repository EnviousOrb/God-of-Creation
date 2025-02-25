using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float speed;
    private Animator animator;
    private Camera cam;
    private float moveDir;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            Debug.Log("NPC Detected");

        }
    }
}
