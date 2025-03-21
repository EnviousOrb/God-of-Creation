using UnityEngine;

public class Transition : MonoBehaviour
{
    [SerializeField] private string sceneToName;
    [SerializeField] private GameObject interactableIcon;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactableIcon.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToName);
            }
        }
    }
}
