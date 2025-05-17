using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
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

    public GameObject susBar;
    public Image susBarImg;
    public TextMeshProUGUI susBarLabel;

    public DDOL DDOL;

    // float timeRemaining;
    // public DateTime timerValue;

    void Start()
    {
        player = GameObject.Find("Alien Player");
        playerMovement = player.GetComponent<playerMovement>();
        foreach (Transform child in Aliens.transform)
        {
            if (child.name.StartsWith("Enemy Soldier"))
                marchingAliens.Add(child.gameObject);
        }

        DDOL = GameObject.Find("DDOL").GetComponent<DDOL>();
        alienSusMeter = 0f;
    }


    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > timeLimit)
        {
            // print("SCENE LOAD");
            DDOL.resets += 1;
            SceneManager.LoadScene("StartScene");
        }

        alienSusStuff();



        // timerValue = DateTime.Today.AddSeconds(timeLimit);
        // StartCoroutine(StartTimer());
    }

    void alienSusStuff()
    {
        bool anyAreSus = false;

        foreach (GameObject alienPos in marchingAliens)
        {
            if (Vector3.Distance(alienPos.transform.position, player.transform.position) < 3f)
            {
                if (!anyAreSus)
                {
                    anyAreSus = true;
                    break;
                }
            }
        }

        if (anyAreSus)
            alienSusMeter += Time.deltaTime / 7f;
        else
            alienSusMeter -= Time.deltaTime / 25f;
        alienSusMeter = Mathf.Clamp(alienSusMeter, 0f, 1.1f);

        float susBarScale = Map(alienSusMeter, 0f, 1f, 0f, 11.6f); susBarScale = Mathf.Clamp(susBarScale, 0f, 11.6f);
        susBar.transform.localScale = new Vector3(1f, susBarScale, 1f);

        if (alienSusMeter > 0.01f)
        {
            float barAlpha = Map(alienSusMeter, 0.0f, 0.66f, 0f, 1f); barAlpha = Mathf.Clamp(barAlpha, 0f, 1f);
            Color temp = Color.Lerp(Color.white, new Color(1f, 0.3f, 0.3f), alienSusMeter); temp.a = barAlpha;
            susBarImg.color = temp;


            float labelAlpha = Map(alienSusMeter, 0f, 0.2f, -.3f, 1f); labelAlpha = Mathf.Clamp01(labelAlpha);

            temp = Color.Lerp(Color.white, new Color(1f, 0.3f, 0.3f), alienSusMeter); temp.a = labelAlpha; 
            susBarLabel.color = temp;

        }

        if (alienSusMeter > 1f)
        {
            captured = true;
        }

        if (captured)
        {
            playerMovement.canMove = false;

            if (blackFadeOut.color.a < 0.97f)
            {
                Color temp = blackFadeOut.color;
                temp.a += Time.deltaTime / 2;
                blackFadeOut.color = temp;
            }
            else
            {
                
                DDOL.resets += 1;
                SceneManager.LoadScene("StartScene");
            }
        }
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
    
    
    float Map(float value, float start1, float end1, float start2, float end2)
    {
        return (value - start1) / (end1 - start1) * (end2 - start2) + start2;
    }
}
