using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class CountDownScript : MonoBehaviour
{
    public float timeLimit = 600; //inSeconds
    float timeRemaining;
    public DateTime timerValue;
    // Start is called before the first frame update
    void Start()
    {
        timerValue = DateTime.Today.AddSeconds(timeLimit);
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer(){
        timeRemaining = timeLimit;
        while (timeRemaining > 0){
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            timerValue = DateTime.Today.AddSeconds(timeRemaining);
        }
        SceneManager.LoadScene("StartScene");
    }
}
