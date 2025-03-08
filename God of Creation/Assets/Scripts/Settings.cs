using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Settings : MonoBehaviour
{
    Resolution[] resolutions;

    [SerializeField] TMP_Dropdown resolutionDropdown;

    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new();

        int currentResolutionIndex = 0;

        foreach (Resolution resolution in resolutions)
        {
            options.Add(resolution.width + " x " + resolution.height);
            currentResolutionIndex++;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void OnResolutionChange(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
