using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteManager : MonoBehaviour
{
    [SerializeField] private Image noteImage;
    [SerializeField] private TextMeshProUGUI storeToMemory;
    //public GameObject physicalNote;
    //public playerMovement movementScript;

    // can't see up close UI at the start of the game
    void Start() {
        //noteImage.enabled = false;
        //movementScript.enabled = true;
    }

    void Update() {
        // note UI and note disappears after the player presses tab - note reappears in inventory
        //if (noteImage.enabled == true && Input.GetKeyDown(KeyCode.Tab)) {
            //noteImage.enabled = false;
            //movementScript.enabled = true;
        //}
    }

    // the note UI can be seen if the player presses tab while being near note - disables movement
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            noteImage.enabled = true;
            storeToMemory.enabled = true;
            //movementScript.enabled = false;
            //Destroy(physicalNote);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            noteImage.enabled = false;
            storeToMemory.enabled = false;
        }
    }
}
