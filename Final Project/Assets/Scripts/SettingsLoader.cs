using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
    void Awake()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("sensitivityMultiplier", 1f);
        float savedTextSpeed = PlayerPrefs.GetFloat("textReadSpeedMultiplier", 1f);

        DDOL ddolInstance = FindObjectOfType<DDOL>();
        if (ddolInstance != null)
        {
            ddolInstance.sensitivityMultiplier = savedSensitivity;
            ddolInstance.textReadSpeedMultiplier = savedTextSpeed;
        }
        else
        {
            Debug.LogWarning("DDOL not found in Start screen — check if it's been initialized yet.");
        }
    }
}