using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// using UnityEditor.Animations;
// using UnityEngine.Apple;

public class CountDownScript : MonoBehaviour
{
    public float timeLimit = 600; //inSeconds
    public float timeElapsed = 0f;
    public float alienSusMeter = 0f;

    public GameObject player;
    public playerMovement playerMovement;
    public Transform Aliens;
    public List<GameObject> marchingAliens = new List<GameObject>();
    public bool captured = false;
    public Image blackFadeOut;

    // float timeRemaining;
    // public DateTime timerValue;

    void Start()
    {
        player = GameObject.Find("Alien Player");
        playerMovement = player.GetComponent<playerMovement>();
        foreach (Transform child in Aliens.transform){
            if (child.name.StartsWith("Enemy Soldier")){
                marchingAliens.Add(child.gameObject);
            }
        }

    }


    void Update()
    {   
        timeElapsed += Time.deltaTime;
        if (timeElapsed > timeLimit){
            print("SCENE LOAD");
            SceneManager.LoadScene("StartScene");
        }

        bool anyAreSus = false;

        foreach (GameObject alienPos in marchingAliens){
            if (Vector3.Distance(alienPos.transform.position, player.transform.position) < 3f){
                if (!anyAreSus) {
                    anyAreSus = true;
                    break;
                }
            }
        }

        if (anyAreSus)
            alienSusMeter += Time.deltaTime / 5f;
        else
            alienSusMeter -= Time.deltaTime / 10f;
        alienSusMeter = Mathf.Clamp(alienSusMeter, 0f, 1.1f);

        if (alienSusMeter > 1f){
            captured = true;
        }

        if (captured){
            playerMovement.canMove = false;

            if (blackFadeOut.color.a < 0.97f){
                Color temp = blackFadeOut.color;
                temp.a += Time.deltaTime / 2;
                blackFadeOut.color = temp;
            }
            else{
                SceneManager.LoadScene("StartScene");
            }
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
