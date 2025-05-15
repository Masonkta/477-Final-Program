using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HighScore;

public enum GameState
{
    WOKE_UP,
    ENTERED_CITY,
    TALKED_TO_COLONEL,
    TALKED_TO_NPC,
    GRABBED_NOTE,
    GRABBED_KEY,
    EXIT
}

public class DDOL : MonoBehaviour
{   
    public GameState highestTaskAchieved;
    public float sensitivityMultiplier = 1f; // 0.3x - 3x
    public float textReadSpeedMultiplier = 1f; // 1 - 5
    public bool hasEverTalkedToFirstDoctorInStartScene;
    public float score = 1200f;
    public float totalPlayTime = 0f;
    public int resets = 0;
    public int NPCVisits = 0;

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

        HS.Init(this, "Final Directive");
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name + ", Mode: " + mode);
        if (scene.name == "City Scene")
            player = GameObject.Find("Alien Player");
        if (scene.name == "Victory Screen")
            calculateAndSubmitHighScore();
    }

    // Update is called once per frame
    void Update()
    {
        totalPlayTime += Time.deltaTime;

        sensitivityMultiplier = Mathf.Clamp(sensitivityMultiplier, 0.3f, 3f);
        textReadSpeedMultiplier = Mathf.Clamp(textReadSpeedMultiplier, 0.5f, 5f);
        
    }

    void calculateAndSubmitHighScore()
    {
        score = 1200 - totalPlayTime - 100 * resets - 20 * (NPCVisits - 3);

        if (score < 0f) score = 0f;
        // Submit this score to cherry's site
        print(score);
        HS.SubmitHighScore(this, "Carl", (int)score);
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
