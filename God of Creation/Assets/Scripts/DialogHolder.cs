using System.Collections;
using UnityEngine;


namespace DialogSystem
{
    public class DialogHolder : MonoBehaviour
    {
        private bool notFirstRun;
        [SerializeField] private bool hasFinalDialog;
        private void OnEnable()
        {
            StartCoroutine(DialogSequence());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                gameObject.SetActive(false);
            }
        }

        private IEnumerator DialogSequence()
        {
            if (hasFinalDialog)
            {
                if (!notFirstRun)
                {
                    for (int i = 0; i < transform.childCount - 1; i++)
                    {
                        Deactivate();
                        transform.GetChild(i).gameObject.SetActive(true);
                        yield return new WaitUntil(() => transform.GetChild(i).GetComponent<Dialog>().IsFinished);
                    }
                }
                else
                {
                    int index = transform.childCount - 1;
                    Deactivate();
                    transform.GetChild(index).gameObject.SetActive(true);
                    yield return new WaitUntil(() => transform.GetChild(index).GetComponent<Dialog>().IsFinished);
                }

                notFirstRun = true;
                gameObject.SetActive(false);
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Deactivate();
                    transform.GetChild(i).gameObject.SetActive(true);
                    yield return new WaitUntil(() => transform.GetChild(i).GetComponent<Dialog>().IsFinished);
                }
            }

            gameObject.SetActive(false);
        }

        private void Deactivate()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
