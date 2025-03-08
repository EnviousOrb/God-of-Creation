using System.Collections;
using TMPro;
using UnityEngine;

namespace DialogSystem 
{
    public class DialogBase : MonoBehaviour
    {
        public bool IsFinished { get; protected set; }

        protected IEnumerator WriteText(string text, TextMeshProUGUI textDisplay, TMP_ColorGradient textColor, float delay, AudioClip textSound)
        {
            textDisplay.colorGradientPreset = textColor;
            for(int i = 0; i < text.Length; i++)
            {
                textDisplay.text += text[i];
                if (textSound != null)
                    AudioManager.Instance.PlaySFX(textSound);
                yield return new WaitForSeconds(delay);
            }

            yield return new WaitUntil(() => Input.GetMouseButton(0));
            IsFinished = true;
        }
    }
}
