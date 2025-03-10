using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Animator transitionAnim;
    [SerializeField] private float transitionTime;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           if(Input.GetKey(KeyCode.E))
            {
                StartCoroutine(LoadScene());
            }
        }
    }

    public IEnumerator LoadScene()
    {
        transitionAnim.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
}
