using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject interactableIcon;
    [SerializeField] private GameObject DialogBox;
    [SerializeField] private Animator transitionAnim;
    [SerializeField] private float transitionTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactableIcon.SetActive(true);
            if (DialogBox)
                DialogBox.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.E))
                StartCoroutine(LoadScene());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactableIcon.SetActive(false);
            if (DialogBox)
                DialogBox.SetActive(false);
        }
    }

    public IEnumerator LoadScene()
    {
        if (transitionAnim)
        {
            transitionAnim.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
        }

        SceneManager.LoadScene(sceneName);
    }
}
