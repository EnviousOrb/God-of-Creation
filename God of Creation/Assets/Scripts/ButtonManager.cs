using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] List<WheelButton> wheelButtons;
    private void Start()
    {
        if(wheelButtons == null)
        {
            Debug.LogError("Wheel buttons are not assigned!");
        }

        DeactivateWheel();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            ActivateWheel();
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            DeactivateWheel();
        }
    }
    private void ActivateWheel()
    {
        foreach (var button in wheelButtons)
        {
            button.gameObject.SetActive(true);
        }
    }

    private void DeactivateWheel()
    {
        foreach (var button in wheelButtons)
        {
            button.gameObject.SetActive(false);
        }
    }
}