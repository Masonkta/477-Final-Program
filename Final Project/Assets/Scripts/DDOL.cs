using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HighScore;
using System;
using Unity.VisualScripting;

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
    public float textReadSpeedMultiplier = 2f; // 1 - 5
    public bool hasEverTalkedToFirstDoctorInStartScene;
    public float score = 1200f;
    public float totalPlayTime = 0f;
    public int resets = 0;
    public int NPCVisits = 0;
    public float weightedTasksCompleted = 0;
    public float taskWeightMult = 1f;

    [Header("City Scene Stuff")]
    public GameObject player;
    public bool countTime = true;
    private static DDOL instance;
    public GameState previousState;
    public GameState currentState;
    public AudioClip onChangeClip;
    public GameObject startButton;

    // ADD CLOCK LOGIC


    private void Awake()
    {
        countTime = true;
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        HS.Init(this, "Final Directive");
    }

    public void Start()
    {
            startButton.SetActive(false);
    }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name + ", Mode: " + mode);
        if (scene.name == "City Scene")
            player = GameObject.Find("Alien Player");
        if (scene.name == "Victory Screen")
            calculateHighScore();
    }

    // Update is called once per frame
    void Update()
    {
        if (countTime) totalPlayTime += Time.deltaTime;

        sensitivityMultiplier = Mathf.Clamp(sensitivityMultiplier, 0.3f, 3f);
        textReadSpeedMultiplier = Mathf.Clamp(textReadSpeedMultiplier, 0.5f, 5f);

        taskWeightMult -= Time.deltaTime / 30f;
        taskWeightMult = Mathf.Clamp(taskWeightMult, 1f, 3f);

        if (currentState != previousState)
        {
            OnStateChanged();
            previousState = currentState;
        }

    }

    public void awardPointsForTask()
    {
        weightedTasksCompleted += taskWeightMult;
        taskWeightMult = 3f;
    }

    void calculateHighScore()
    {
        countTime = false;
        if (weightedTasksCompleted > 16f) weightedTasksCompleted = 16f;
        score = 6800f - totalPlayTime - 700f * resets - 100f * (NPCVisits - 3) + weightedTasksCompleted * 200f;

        if (score < 0f) score = 0f;
    }

    public void submitHighScore(string name)
    {
        // Submit this score to cherry's site
        print(name + " got a score of " + (int)score);
        HS.SubmitHighScore(this, name, (int)score);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnStateChanged()
    {
        // Play sound on state change
        if (onChangeClip != null && GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().PlayOneShot(onChangeClip);
        }
    }

}
