using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Threading;

public class TeleportNewScene : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI enterText;

    [SerializeField]
    private Transform spawnPoint;

    public GameObject player;
    public playerMovement playerMovement;
    public Image blackFadeOut;
    public String nameOfBuilding;
    public DDOL DDOL;
    public bool readyToTeleport;

    [SerializeField]
    private bool willTeleport;
    GameObject MiniMapCamera;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Alien Player");
        playerMovement = player.GetComponent<playerMovement>();
        if (enterText) enterText.enabled = false;
        willTeleport = false;
        DDOL = GameObject.Find("DDOL").GetComponent<DDOL>();
        //miniMapCamera = GameObject.Find("minimapCamera");
        GameObject UI = GameObject.Find("Game UI");
        GameObject MiniMap = UI.transform.Find("MiniMap").gameObject;
        MiniMapCamera = MiniMap.GetComponent<miniMapScript>().miniMapCamera;
    }

    void Update() {
        if (willTeleport && Input.GetKeyDown(KeyCode.G) && playerMovement.hasKey) {
            Debug.Log("G was pressed");
            enterText.enabled = false;
            willTeleport = false;


            player.SetActive(false);
            Debug.Log("SpawnPoint position: " + spawnPoint.localPosition);
            player.transform.position = spawnPoint.position;
            Debug.Log("New player position: " + player.transform.position);
            player.SetActive(true);

            if (nameOfBuilding == "Wiley")
            {
                MiniMapCamera.SetActive(false);
                if (DDOL){
                    DDOL.awardPointsForTask();
                    DDOL.highestTaskAchieved = GameState.EXIT;
                }
                readyToTeleport = true;

            }

        }

        if (readyToTeleport)
            loadVictoryScene();
    }

    public void loadVictoryScene()
    {
        if (blackFadeOut.color.a < 0.97f)
        {
            Color temp = blackFadeOut.color;
            temp.a += Time.deltaTime / 6f;
            blackFadeOut.color = temp;
        }
        else
        {
            SceneManager.LoadScene("Victory Screen");
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        enterText.enabled = true;
        enterText.text = playerMovement.hasKey ? "[G] Enter Building" : "You need a key to access.";
        willTeleport = true;
    }

    public void OnTriggerExit(Collider other) {
        enterText.enabled = false;
        willTeleport = false;
    }
}
