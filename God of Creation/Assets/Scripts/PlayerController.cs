using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float speed = 5.0f;
    public Animator animator;
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
            transform.localScale = new Vector3(0.5f, transform.localScale.y, transform.localScale.z);
        }
        else if (moveDir < 0)
        {
            transform.localScale = new Vector3(-0.5f, transform.localScale.y, transform.localScale.z);
        }
        animator.SetFloat("HeroSpeed", Mathf.Abs(moveDir));
    }

    void FixedUpdate()
    {
        transform.position += new Vector3(moveDir, 0, 0) * speed * Time.deltaTime;
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
    }
}
