using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeleportNewScene : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI enterText;

    [SerializeField]
    private Transform spawnPoint;

    public GameObject player;

    [SerializeField]
    private bool willTeleport;
    // Start is called before the first frame update
    void Start(){
        player = GameObject.Find("Alien Player");
        enterText.enabled = false;
        willTeleport = false;
    }

    void Update() {
        if (willTeleport && Input.GetKeyDown(KeyCode.G) && player.GetComponent<playerMovement>().hasKey == true) {
            Debug.Log("G was pressed");
            enterText.enabled = false;
            willTeleport = false;

            player.SetActive(false);
            Debug.Log("SpawnPoint position: " + spawnPoint.localPosition);
            player.transform.position = spawnPoint.position;
            Debug.Log("New player position: " + player.transform.position);
            player.SetActive(true);
        }
    }

    public void OnTriggerEnter(Collider other) {
        enterText.enabled = true;
        willTeleport = true;
    }

    public void OnTriggerExit(Collider other) {
        enterText.enabled = false;
        willTeleport = false;
    }
}
