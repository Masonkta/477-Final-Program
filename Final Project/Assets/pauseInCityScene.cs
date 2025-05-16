using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class pauseInCityScene : MonoBehaviour
{

    public GameObject inventoryPanel;
    public bool inventoryIsOpen;
    public GameObject optionsPanel;
    public bool paused;
    public GameObject player;
    public playerMovement playerMovement;
    public InventoryController inventoryController;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Alien Player");
        playerMovement = player.GetComponent<playerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            paused = !paused;

        inventoryIsOpen = inventoryPanel.activeInHierarchy;
        optionsPanel.SetActive(paused | inventoryIsOpen);

        if (inventoryIsOpen | paused)
            playerMovement.canMove = false;

        if (!paused && !inventoryIsOpen)
            playerMovement.canMove = true;
        
    }

    void getInput(){
        
        if (Input.GetKeyDown(KeyCode.Escape)){
            paused = !paused;

            inventoryController.inventoryPanelStatus = false;
            inventoryController.InventoryPanel.SetActive(inventoryController.inventoryPanelStatus);

        }

    }
}
