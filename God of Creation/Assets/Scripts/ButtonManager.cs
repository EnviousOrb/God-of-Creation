using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] List<Button> wheelButtons;
    private void Start()
    {
        if(wheelButtons == null)
            Debug.LogError("Wheel buttons are not assigned!");

        foreach (var button in wheelButtons)
            button.onClick.AddListener(() => GameManager.Instance.OnSwapHero(button.name));

        DeactivateWheel();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            ActivateWheel();
        else if (Input.GetKeyUp(KeyCode.Q))
            DeactivateWheel();
    }
    private void ActivateWheel()
    {
        foreach (var button in wheelButtons)
            button.gameObject.SetActive(true);
    }

    private void DeactivateWheel()
    {
        foreach (var button in wheelButtons)
            button.gameObject.SetActive(false);
    }
}