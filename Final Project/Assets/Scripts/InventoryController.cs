using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    bool inventoryPanelStatus = false; 
    GameObject InventoryPanel;
    GameObject Player;
    public float grabDistance = 5f;
    List<GameObject> inventoryBoxes = new List<GameObject>{null, null, null, null, null, null, null, null, null, null};
    List<GameObject> boxContents = new List<GameObject>{null, null, null, null, null, null, null, null, null, null};    
    TextMeshProUGUI pickUpText;
    GameObject hand;
    GameObject Collectables;
    public NoteManager noteScript;
    public bool hasSeenInventoryPanel = true;
    Button InventoryButton;
    bool inventoryButtonClicked = false;
    public float timeOfNoteGrabbed; bool noteGrabbed;
    public GameObject noteItself;
    public DDOL DDOL;
    bool objInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        //assigning variables
        GameObject UI = GameObject.Find("Game UI");
        InventoryPanel = UI.transform.Find("Inventory Panel").gameObject;
        InventoryButton = UI.transform.Find("Inventory Button").GetComponent<Button>();
        InventoryButton.onClick.AddListener(toggleInventory);
        Player = GameObject.Find("Alien Player");
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
    }

    // Update is called once per frame
    void Update()
    {
        //toggles inventory panel
        if ((Input.GetKeyDown(KeyCode.Tab) || inventoryButtonClicked) && (hand.transform.childCount == 0 || !hasSeenInventoryPanel)){
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
            if (Input.GetKeyDown(KeyCode.R))
            {
                DropObject();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                //put the object in inventory
                grabObject(hand.transform.GetChild(0).gameObject);
            }
            if (hand.transform.GetChild(0).name == "key")
                grabObject(hand.transform.GetChild(0).gameObject);
        }
        //the player is not holding anything
        else
        {
            //determines if the player can and should grab the object
            GameObject[] collectable = GameObject.FindGameObjectsWithTag("Collectable");
            foreach (GameObject obj in collectable)
            {
                float distance = (Player.transform.position - obj.transform.position).magnitude;
                //Debug.Log(obj.transform.name);
                //Debug.Log(distance);
                //Debug.Log(grabDistance);
                //Debug.Log(obj.transform.name);
                if (distance < grabDistance)
                {
                    objInRange = true;
                    pickUpText.text = "Press E to collect " + obj.transform.name;
                    if (Input.GetKeyDown(KeyCode.E) && !obj.name.StartsWith("note"))
                    {
                        PickUpObject(obj);
                    }
                    else if (Input.GetKeyDown(KeyCode.E) && obj.name.StartsWith("note"))
                    {
                        // Debug.Log("pressed E while near note");
                        // grabObject(obj);
                        obj.SetActive(false);
                        // Debug.Log("the obj was a note and called grabObject");
                        noteGrabbed = true;
                        timeOfNoteGrabbed = Time.time;
                        noteItself.SetActive(true);

                        // THIS IS WHEN WE SWITCH STATES TO VISIT PARKING GARAGE
                        if (DDOL)
                            if (DDOL.highestTaskAchieved != GameState.GRABBED_KEY)
                                DDOL.highestTaskAchieved = GameState.GRABBED_NOTE; // NOW WE GRABBED NOTE





                    }
                }
                else if (!objInRange)
                {
                    pickUpText.text = "";
                }
            }
        }
        objInRange = false;

        if (noteGrabbed && noteItself.activeInHierarchy && Time.time - timeOfNoteGrabbed > 4f && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.W)))
            noteItself.SetActive(false);
            
    }   

    void toggleInventory(){
        inventoryButtonClicked = true;
    }  

    void inventoryBoxClicked(int num){
        Debug.Log("Box # " + num + " clicked");

        //puts the selected object in your "hand"
        Debug.Log(hand.transform.transform.childCount);
        if (boxContents[num] != null){
            if (hand.transform.transform.childCount == 0 && boxContents[num].name != "note"){
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
            } else if (boxContents[num].name == "note") {
                Debug.Log("This item cannot be taken out of inventory. It can be looked at though");
            }
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

    void grabObject(GameObject obj){
        //find the next inventory box to put the item in
        if (obj.transform.name == "key")
        {
            Player.GetComponent<playerMovement>().hasKey = true;
            if (DDOL)
                DDOL.highestTaskAchieved = GameState.GRABBED_KEY; // NOW WE GRABBED KEY AND ARE READY
        }
        for (int i = 0; i < inventoryBoxes.Count; i++){
            if (boxContents[i] == null){
                boxContents[i] = obj;
                TextMeshProUGUI boxText = inventoryBoxes[i].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                boxText.text = boxContents[i].transform.name;
                break;
            }
        }
        //maybe add a shrink and destroy effect here
        obj.transform.SetParent(null);
        obj.SetActive(false);

        if (obj.name.StartsWith("note")) {
            noteScript.noteImage.enabled = false;
            //noteScript.storeToMemory.enabled = false;
        }
    }

    void DropObject(){
        // notes cannot be dropped - maybe a UI statement that says "cant drop this item"
        if (hand.transform.GetChild(0).name != "note") {
            hand.transform.GetChild(0).tag = "Collectable";
            hand.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
            hand.transform.GetChild(0).SetParent(Collectables.transform);
        } else {
            Debug.Log("cant drop notes my dude");
        }
    }

    void PickUpObject(GameObject obj){
        if (hand.transform.childCount == 0){
            obj.transform.position = hand.transform.position;
            obj.transform.SetParent(hand.transform);
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.tag = "Untagged";
        }
    }   
}