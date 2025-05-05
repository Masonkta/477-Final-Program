using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class gameHandler : MonoBehaviour
{

    GameObject InventoryPanel; 
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 144;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (GameObject.Find("Game UI")){
            GameObject UI = GameObject.Find("Game UI");
            if (UI.transform.Find("Inventory Panel")){
                Debug.Log("found panel");
                InventoryPanel = UI.transform.Find("Inventory Panel").gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
        if (InventoryPanel){
            if (!InventoryPanel.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                    ToggleCursor();
            }
            else{
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else{
            if (Input.GetKeyDown(KeyCode.Tab))
                    ToggleCursor();
        }
    }

    /////////////////////////////////////////////////////////////  

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #endif
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

    ////////////////////////////////////////////////////////////



}
