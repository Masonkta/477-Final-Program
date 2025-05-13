using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    public bool paused;
    public TextMeshProUGUI Timer;
    DateTime time;
    public static TimerScript Instance;
    public bool hasTryedAtLeastOnce = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameObject TimeManager = GameObject.Find("TimeManager");
            DontDestroyOnLoad(TimeManager);
            GameObject Room = GameObject.Find("Room");
            GameObject TimerTV = Room.transform.Find("Timer TV").gameObject;
            GameObject Cube = TimerTV.transform.Find("Cube.006").gameObject;
            GameObject Canvas = Cube.transform.Find("Canvas").gameObject;
            Timer = Canvas.transform.Find("Time").GetComponent<TextMeshProUGUI>();
            time = DateTime.Now;
            Timer.text = "Time: \n" + time.ToString("HH:mm:ss");
            StartCoroutine(TimeRunning());
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    // Update is called once per frame
    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "StartScene"){
            if (!Timer){
                GameObject Room = GameObject.Find("Room");
                GameObject TimerTV = Room.transform.Find("Timer TV").gameObject;
                GameObject Cube = TimerTV.transform.Find("Cube.006").gameObject;
                GameObject Canvas = Cube.transform.Find("Canvas").gameObject;
                Timer = Canvas.transform.Find("Time").GetComponent<TextMeshProUGUI>();
            }
            paused = false;
        }
        else{
            paused = true;
            hasTryedAtLeastOnce = true;
        }
    }
    IEnumerator TimeRunning(){
        while (true){
            while(paused){
                yield return null;
            }
            yield return new WaitForSeconds(1f);
            time = time.AddSeconds(1);
            Timer.text = "Time: \n" + time.ToString("HH:mm:ss");
            //Debug.Log(time);
        }
    }
}
