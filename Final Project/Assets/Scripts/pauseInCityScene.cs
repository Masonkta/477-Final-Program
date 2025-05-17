using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class pauseInCityScene : MonoBehaviour
{
    public float timeEscapePressed;
    public GameObject optionsPanel;
    public bool paused;
    public bool readyToPause = true;
    public GameObject player;
    public playerMovement playerMovement;
    public InventoryController inventoryController;
    public GameObject holdEscToQuitText;
    public Transform quitProgressBar;
    public Image quitProgressBarImg;


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
        holdEscToQuitText.SetActive(optionsPanel.activeInHierarchy);
        quitProgressBar.gameObject.SetActive(holdEscToQuitText.activeInHierarchy);

        playerMovement.canMove = !(paused || inventoryIsOpen);

        // Quit Progress Bar Stuff
        if (timeEscapePressed > 0.25f)
            quitProgressBar.localScale = new Vector3(1f, Map(timeEscapePressed, 0.25f, 2f, 0f, 10.55f), 1f);
        else
            quitProgressBar.localScale = new Vector3(1f, 0f, 1f);

        Color theRed;
        if (ColorUtility.TryParseHtmlString("#FF8A8A", out theRed))
            quitProgressBarImg.color = Color.Lerp(Color.white, theRed, timeEscapePressed / 2f);

        if (timeEscapePressed >= 2f)
            QuitGame();
    }

    void getInput()
    {
        if (Input.GetKey(KeyCode.Escape) && optionsPanel.activeInHierarchy)
            timeEscapePressed += Time.deltaTime;
        else
        {
            timeEscapePressed = 0f;
            readyToPause = true;
        }
        if (timeEscapePressed < 0f) timeEscapePressed = 0f;

        if (Input.GetKeyUp(KeyCode.Escape))// && !readyToPause) || (timeEscapePressed > 0.02f && readyToPause))
        {
            
            readyToPause = false;
            if (inventoryController.inventoryPanelStatus) // Only do this stuff if you press escape while only inventory is up
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

    float Map(float value, float start1, float end1, float start2, float end2)
    {
        return (value - start1) / (end1 - start1) * (end2 - start2) + start2;
    }
    
    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #endif
    }

}
