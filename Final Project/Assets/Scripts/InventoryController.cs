using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public bool inventoryPanelStatus = false;
    public GameObject InventoryPanel;
    GameObject Player;
    public playerMovement playerMovement;
    public float grabDistance = 5f;
    List<GameObject> inventoryBoxes = new List<GameObject> { null, null, null, null, null, null, null, null, null, null };
    List<GameObject> boxContents = new List<GameObject> { null, null, null, null, null, null, null, null, null, null };
    TextMeshProUGUI pickUpText;
    GameObject hand;
    GameObject Collectables;
    public NoteManager noteScript;
    public bool hasSeenInventoryPanel = true;
    Button InventoryButton;
    bool inventoryButtonClicked = false;
    public float timeOfNoteGrabbed; bool noteGrabbed = false;
    public GameObject noteItself;
    public DDOL DDOL;
    bool objInRange = false;
    public bool isWarning = false; float timeOfLastWarning;

    // Start is called before the first frame update
    void Start()
    {
        //assigning variables
        GameObject UI = GameObject.Find("Game UI");
        InventoryPanel = UI.transform.Find("Inventory Panel").gameObject;
        InventoryButton = UI.transform.Find("Inventory Button").GetComponent<Button>();
        InventoryButton.onClick.AddListener(toggleInventory);
        Player = GameObject.Find("Alien Player");
        playerMovement = Player.GetComponent<playerMovement>();
        hand = Player.transform.Find("Hand").gameObject;
        GameObject Panel = UI.transform.Find("Panel").gameObject;
        Transform textTransform = Panel.transform.Find("PickUpText");
        pickUpText = textTransform.GetComponent<TextMeshProUGUI>();
        Collectables = GameObject.Find("Collectables");

        for (int i = 0; i < inventoryBoxes.Count; i++)
        {
            //Debug.Log(i);
            inventoryBoxes[i] = InventoryPanel.transform.GetChild(i + 1).gameObject;
            Button boxButton = inventoryBoxes[i].GetComponent<Button>();
            int boxClicked = i;
            boxButton.onClick.AddListener(() => inventoryBoxClicked(boxClicked));
        }

        DDOL = GameObject.Find("DDOL").GetComponent<DDOL>();
        timeOfLastWarning = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //toggles inventory panel
        if ((Input.GetKeyDown(KeyCode.Tab) || inventoryButtonClicked) && (hand.transform.childCount == 0 || !hasSeenInventoryPanel))
        {
            InventoryPanel.SetActive(!inventoryPanelStatus);
            inventoryPanelStatus = !inventoryPanelStatus;
            hasSeenInventoryPanel = true;
            inventoryButtonClicked = false;
            ToggleCursor();
        }

        //drop object if desired   im thinking x to drop and c to collect/add to inventory
        if (hand.transform.childCount > 0)
        {
            pickUpText.text = "R to drop " + hand.transform.GetChild(0).name + " or press E to store it in your inventory.";
            if (Time.time - timeOfLastWarning < 1.4f)
                pickUpText.text = "Inventory is full.";

            if (Input.GetKeyDown(KeyCode.R))
            {
                DropObject();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                //put the object in inventory
                if (!InventoryIsFull())
                {
                    grabObject(hand.transform.GetChild(0).gameObject);
                }
                else
                {
                    pickUpText.text = "Inventory is full.";
                    timeOfLastWarning = Time.time;
                }
            }

            // immediately grab key 
            if (hand.transform.GetChild(0).name == "key")
                grabObject(hand.transform.GetChild(0).gameObject);
        }
        //the player is not holding anything
        else
        {
            GameObject[] collectable = GameObject.FindGameObjectsWithTag("Collectable");
            GameObject closest = null;
            float minDist = grabDistance;

            foreach (GameObject obj in collectable)
            {
                float distance = Vector3.Distance(Player.transform.position, obj.transform.position);
                if (distance < minDist)
                {
                    closest = obj;
                    minDist = distance;
                }
            }

            if (closest != null)
            {
                objInRange = true;
                pickUpText.text = "Press E to collect " + closest.transform.name;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (!closest.name.StartsWith("note"))
                    {
                        PickUpObject(closest);
                    }
                    else
                    {
                        PickUpObject(closest);
                        noteGrabbed = true;
                        timeOfNoteGrabbed = Time.time;
                        noteItself.SetActive(true);

                        if (DDOL && DDOL.highestTaskAchieved != GameState.GRABBED_KEY)
                        {
                            DDOL.awardPointsForTask();
                            DDOL.highestTaskAchieved = GameState.GRABBED_NOTE;
                            DDOL.currentState = GameState.GRABBED_NOTE;
                        }
                    }
                }
            }
            else
            {
                pickUpText.text = "";
            }
        }
        objInRange = false;
        if (noteGrabbed && noteItself.activeInHierarchy && Time.time - timeOfNoteGrabbed > 2 && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.W)))
        {
            noteItself.SetActive(false);
            Debug.Log(hand.transform.GetChild(0).name);
            grabObject(hand.transform.GetChild(0).gameObject);
        }

        if (hand.transform.childCount > 0)
        {

            // Update position every frame to match hand position + camHeight
            Transform held = hand.transform.GetChild(0);
            held.position = hand.transform.position + Vector3.up * Map(playerMovement.actualCamHeight, 0.1f, 2.2f, 1.5f, -0.3f);

            // ... the rest of your existing code here ...
        }
    }   

    float Map(float value, float start1, float end1, float start2, float end2)
    {
        return (value - start1) / (end1 - start1) * (end2 - start2) + start2;
    }

    void toggleInventory()
    {
        inventoryButtonClicked = true;
    }

    void inventoryBoxClicked(int num)
    {
        Debug.Log("Box # " + num + " clicked");

        //puts the selected object in your "hand"
        Debug.Log(hand.transform.transform.childCount);
        if (boxContents[num] != null)
        {
            if (hand.transform.transform.childCount == 0 && boxContents[num].name != "note")
            {
                boxContents[num].SetActive(true);
                // if (playerMovement) boxContents[num].transform.position = hand.transform.position + Vector3.up * playerMovement.actualCamHeight;
                boxContents[num].transform.position = hand.transform.position;
                boxContents[num].transform.SetParent(hand.transform);
                boxContents[num].GetComponent<Rigidbody>().isKinematic = true;
                boxContents[num].tag = "Untagged";
                boxContents[num] = null;
                TextMeshProUGUI boxText = inventoryBoxes[num].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                boxText.text = "";

                InventoryPanel.SetActive(!inventoryPanelStatus);
                inventoryPanelStatus = !inventoryPanelStatus;
                ToggleCursor();
            }
            else if (boxContents[num].name == "note")
            {
                boxContents[num].SetActive(true);
                boxContents[num].transform.position = hand.transform.position;
                boxContents[num].transform.SetParent(hand.transform);
                boxContents[num].GetComponent<Rigidbody>().isKinematic = true;
                boxContents[num].tag = "Untagged";
                boxContents[num] = null;
                TextMeshProUGUI boxText = inventoryBoxes[num].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                boxText.text = "";

                InventoryPanel.SetActive(!inventoryPanelStatus);
                inventoryPanelStatus = !inventoryPanelStatus;
                ToggleCursor();
                noteGrabbed = true;
                timeOfNoteGrabbed = Time.time;
                noteItself.SetActive(true);
            }
        }
    }

    public void CloseInventory()
    {
        InventoryPanel.SetActive(false);
        inventoryPanelStatus = false;
        ToggleCursor();
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

    void grabObject(GameObject obj)
    {
        //find the next inventory box to put the item in
        if (obj.transform.name == "key")
        {
            Player.GetComponent<playerMovement>().hasKey = true;
            if (DDOL)
            {
                DDOL.awardPointsForTask();
                DDOL.highestTaskAchieved = GameState.GRABBED_KEY; // NOW WE GRABBED KEY AND ARE READY
                DDOL.currentState = GameState.GRABBED_KEY;

            }

        }
        for (int i = 0; i < inventoryBoxes.Count; i++)
        {
            if (boxContents[i] == null)
            {
                boxContents[i] = obj;
                TextMeshProUGUI boxText = inventoryBoxes[i].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                boxText.text = boxContents[i].transform.name;
                break;
            }
        }
        //maybe add a shrink and destroy effect here
        obj.transform.SetParent(null);
        obj.SetActive(false);

        //if (obj.name.StartsWith("note")) {
        //    noteScript.noteImage.enabled = false;
        //    //noteScript.storeToMemory.enabled = false;
        //}
    }

    void DropObject()
    {
        // notes cannot be dropped - maybe a UI statement that says "cant drop this item"
        if (hand.transform.GetChild(0).name != "note")
        {
            hand.transform.GetChild(0).tag = "Collectable";
            hand.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
            hand.transform.GetChild(0).SetParent(Collectables.transform);
        }
        else
        {
            Debug.Log("cant drop notes my dude");
        }
    }

    void PickUpObject(GameObject obj)
    {
        if (hand.transform.childCount == 0)
        {
            // if (playerMovement) obj.transform.position = hand.transform.position + Vector3.up * playerMovement.actualCamHeight;
            obj.transform.position = hand.transform.position;
            obj.transform.SetParent(hand.transform);
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.tag = "Untagged";
        }
    }   
    
    bool InventoryIsFull()
    {
        foreach (GameObject item in boxContents)
        {
            if (item == null) return false;
        }
        return true;
    }
}