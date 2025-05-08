using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeleportNewScene : MonoBehaviour
{
    public TextMeshProUGUI enterText;
    public transform spawnPoint;
    public GameObject player;
    // Start is called before the first frame update
    void Start(){
        enterText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other) {
        enterText.enabled = true;
        if (Input.GetKeyDown(KeyCode.G)) {
            enterText.enabled = false;
            player.transform.point = SpawnPoint.position;
        }
    }

    public void OnTriggerExit() {
        
    }
}
