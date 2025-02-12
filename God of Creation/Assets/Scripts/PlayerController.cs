using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float speed = 5.0f;
    private Camera cam;
    private Vector2 moveDir;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void FixedUpdate()
    {
        transform.Translate(moveDir * speed * Time.fixedDeltaTime);
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
    }
}
