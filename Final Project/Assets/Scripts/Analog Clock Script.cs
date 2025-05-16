using TMPro;
using UnityEngine;

public class AnalogClockScript : MonoBehaviour
{
    public CountDownScript countDown;
    public TextMeshProUGUI timeText;
    public float totalTime = 300f; 

    void Update()
    {
        float timeLeft = Mathf.Max(0, totalTime - countDown.timeElapsed);

        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
