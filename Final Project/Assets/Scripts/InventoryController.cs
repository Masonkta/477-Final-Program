using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public bool inventoryPanelStatus = false; 
    GameObject InventoryPanel;
    // Start is called before the first frame update
    void Start()
    {
        GameObject UI = GameObject.Find("Game UI");
        InventoryPanel = UI.transform.Find("Inventory Panel").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)){
            InventoryPanel.SetActive(!inventoryPanelStatus);
            inventoryPanelStatus = !inventoryPanelStatus;
            ToggleCursor();
        } 
    }      
    
    void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}