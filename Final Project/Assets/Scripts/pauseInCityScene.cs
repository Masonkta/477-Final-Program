using System.Collections;
using UnityEngine;

public class pauseInCityScene : MonoBehaviour
{
    public GameObject optionsPanel;
    public bool paused;
    public GameObject player;
    public playerMovement playerMovement;
    public InventoryController inventoryController;

    void Start()
    {
        player = GameObject.Find("Alien Player");
        playerMovement = player.GetComponent<playerMovement>();
    }

    void Update()
    {
        getInput();

        bool inventoryIsOpen = inventoryController.inventoryPanelStatus;

        optionsPanel.SetActive(paused || inventoryIsOpen);

        playerMovement.canMove = !(paused || inventoryIsOpen);
    }

    void getInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryController.inventoryPanelStatus)
            {
                inventoryController.CloseInventory();
                paused = false;
                return;
            }

            paused = !paused;
            
            inventoryController.CloseInventory();
            
            if (!paused)
            {
                print("DISAPPEAR MOUSE");
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

        }
    }
}
