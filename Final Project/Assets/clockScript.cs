using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class clockScript : MonoBehaviour
{      
    public Transform baseOfTower;
    public GameObject player;
    public GameObject playerCamera;
    public GameObject helpTextForClock;
    public GameObject hintForClock;
    public GameObject clockCamera;

    public float timeElapsed = 0f;
    public float seconds;
    public float minutes;
    public float hours;
    public Transform s1;
    public Transform m1;
    public Transform h1;
    public Transform s2;
    public Transform m2;
    public Transform h2;
    public Transform s3;
    public Transform m3;
    public Transform h3;
    public Transform s4;
    public Transform m4;
    public Transform h4;

    void Start()
    {
        player = GameObject.Find("Alien Player");
        playerCamera = player.transform.Find("Player Camera").gameObject;   
    }

    // Update is called once per frame
    void Update()
    {
        timeStuff();
        
        // print(Vector3.Distance(player.transform.position, baseOfTower.position));
        helpTextForClock.SetActive(Vector3.Distance(player.transform.position, baseOfTower.position) < 17f);
        if (Vector3.Distance(player.transform.position, baseOfTower.position) < 17f){
            if (Input.GetMouseButton(1)){
                helpTextForClock.SetActive(false);
                hintForClock.SetActive(true);
                playerCamera.SetActive(false);
                clockCamera.SetActive(true);
            }
            else{
                hintForClock.SetActive(false);
                playerCamera.SetActive(true);
                clockCamera.SetActive(false);
            }
        }
        
    }

    void timeStuff(){
        timeElapsed += Time.deltaTime;
        seconds = Mathf.RoundToInt(timeElapsed % 60f);
        minutes = (timeElapsed / 60f) % 60f;
        hours = (minutes / 60f) % 60f;

        s1.localEulerAngles = new Vector3(0f, 0f, Map(seconds, 0, 60, 0f, -360f));
        m1.localEulerAngles = new Vector3(0f, 0f, Map(minutes, 0, 60, 90f, -270f));
        h1.localEulerAngles = new Vector3(0f, 0f, Map(hours, 0, 12, -180f, -540f));

        s2.localEulerAngles = new Vector3(0f, 0f, Map(seconds, 0, 60, 0f, 360f));
        m2.localEulerAngles = new Vector3(0f, 0f, Map(minutes, 0, 60, 270f, 360f+270f));
        h2.localEulerAngles = new Vector3(0f, 0f, Map(hours, 0, 12, -180f, 180f));

        s3.localEulerAngles = new Vector3(Map(seconds, 0, 60, 0f, -360f), 0f, 0f);
        m3.localEulerAngles = new Vector3(Map(minutes, 0, 60, 90f, -360f+90f), 0f, 0f);
        h3.localEulerAngles = new Vector3(Map(hours, 0, 12, 180f, -180f), 0f, 0f);
        
        s4.localEulerAngles = new Vector3( Map(seconds, 0, 60, 0f, 360f), 0f, 0f);
        m4.localEulerAngles = new Vector3( Map(minutes, 0, 60, -90f, 270f), 0f, 0f);
        h4.localEulerAngles = new Vector3( Map(hours, 0, 12, -180f, 180f), 0f, 0f);
    }

    
    float Map(float value, float start1, float end1, float start2, float end2)
    {
        return (value - start1) / (end1 - start1) * (end2 - start2) + start2;
    }
}
