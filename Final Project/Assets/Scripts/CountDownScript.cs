using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class CountDownScript : MonoBehaviour
{
    public float timeLimit = 600; //inSeconds
    public float timeElapsed = 0f;
    // float timeRemaining;
    // public DateTime timerValue;
    // Start is called before the first frame update
    void Update()
    {   
        timeElapsed += Time.deltaTime;
        if (timeElapsed > timeLimit){
            print("SCENE LOAD");
            SceneManager.LoadScene("StartScene");
        }
        // timerValue = DateTime.Today.AddSeconds(timeLimit);
        // StartCoroutine(StartTimer());
    }

    // IEnumerator StartTimer(){
    //     timeRemaining = timeLimit;
    //     while (timeRemaining > 0){
    //         yield return new WaitForSeconds(1f);
    //         timeRemaining -= 1f;
    //         timerValue = DateTime.Today.AddSeconds(timeRemaining);
    //     }
    //     SceneManager.LoadScene("StartScene");
    // }
}
