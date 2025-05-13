using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TeleportNewScene : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI enterText;

    [SerializeField]
    private Transform spawnPoint;

    public GameObject player;
    public playerMovement playerMovement;
    public Image blackFadeOut;

    [SerializeField]
    private bool willTeleport;
    // Start is called before the first frame update
    void Start(){
        player = GameObject.Find("Alien Player");
        playerMovement = player.GetComponent<playerMovement>();
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
        if (Vector3.Distance(player.transform.position, new Vector3(-12.7874985f,-37.2249985f,76.3000031f)) < 30f){
                print(1);
                playerMovement.canMove = false;

                if (blackFadeOut.color.a < 0.97f){
                    Color temp = blackFadeOut.color;
                    temp.a += Time.deltaTime / 2;
                    blackFadeOut.color = temp;
                }
                else{
                    SceneManager.LoadScene("Victory Screen");
                }
            
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
