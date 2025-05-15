using UnityEngine;
using UnityEngine.UI;   

public class OptionsMenu : MonoBehaviour
{
    private DDOL ddolInstance;

    public Slider sensitivitySlider;
    public Slider textSpeedSlider;

    void Start()
    {
        ddolInstance = FindObjectOfType<DDOL>();
        if (ddolInstance == null)
        {
            Debug.LogError("DDOL not found in scene.");
            return;
        }

        // Initialize sliders
        sensitivitySlider.minValue = 0.3f;
        sensitivitySlider.maxValue = 3f;
        sensitivitySlider.value = ddolInstance.sensitivityMultiplier;

        textSpeedSlider.minValue = 1f;
        textSpeedSlider.maxValue = 5f;
        textSpeedSlider.value = ddolInstance.textReadSpeedMultiplier;

        // Listen for changes
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        textSpeedSlider.onValueChanged.AddListener(UpdateTextSpeed);
    }

    void UpdateSensitivity(float value)
    {
        if (ddolInstance != null)
            ddolInstance.sensitivityMultiplier = value;

        PlayerPrefs.SetFloat("sensitivityMultiplier", value);
    }

    void UpdateTextSpeed(float value)
    {
        if (ddolInstance != null)
            ddolInstance.textReadSpeedMultiplier = value;

        PlayerPrefs.SetFloat("textReadSpeedMultiplier", value);
    }
}
