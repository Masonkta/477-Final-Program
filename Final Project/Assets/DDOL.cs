using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    WOKE_UP,
    ENTERED_CITY,
    TALKED_TO_COLONEL,
    TALKED_TO_NPC,
    FIND_NOTE,
    GRAB_KEY,
    EXIT
}

public class DDOL : MonoBehaviour
{   
    public GameState highestTaskAchieved;
    public int resets;
    public float sensitivityMultiplier = 1f; // 0.3x - 3x
    public float textReadSpeed = 1f; // 1 - 5
    public bool hasEverTalkedToFirstDoctorInStartScene;
    public float score = 1200f;

    [Header("City Scene Stuff")]
    public GameObject player;

    private static DDOL instance;
    // ADD CLOCK LOGIC


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    // void Start()
    // {
    //     if (GameObject.Find("DDOL"))
    //         Destroy(this);              // For when we reset back, dont make a new DDOL object

    //     DontDestroyOnLoad(this);
    // }

    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name + ", Mode: " + mode);
        if (scene.name == "City Scene")
            player = GameObject.Find("Alien Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
