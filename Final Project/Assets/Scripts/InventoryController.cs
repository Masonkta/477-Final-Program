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

    // Start is called before the first frame update
    void Start()
    {
        //assigning variables
        GameObject UI = GameObject.Find("Game UI");
        InventoryPanel = UI.transform.Find("Inventory Panel").gameObject;
        Player = GameObject.Find("Alien Player");
        hand = Player.transform.Find("Hand").gameObject;
        GameObject Panel = UI.transform.Find("Panel").gameObject;
        Transform textTransform = Panel.transform.Find("PickUpText");
        pickUpText = textTransform.GetComponent<TextMeshProUGUI>();
        Collectables = GameObject.Find("Collectables");

        for (int i = 0; i < inventoryBoxes.Count; i++){
            //Debug.Log(i);
            inventoryBoxes[i] = InventoryPanel.transform.GetChild(i+1).gameObject;
            Button boxButton = inventoryBoxes[i].GetComponent<Button>();
            int boxClicked = i;
            boxButton.onClick.AddListener(() => inventoryBoxClicked(boxClicked));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //toggles inventory panel
        if (Input.GetKeyDown(KeyCode.E)){
            InventoryPanel.SetActive(!inventoryPanelStatus);
            inventoryPanelStatus = !inventoryPanelStatus;
            ToggleCursor();
        } 

        //drop object if desired   im thinking x to drop and c to collect/add to inventory
        if (hand.transform.childCount > 0){
            pickUpText.text = "Press X to drop " + hand.transform.GetChild(0).name + " or press C to store it in your inventory. Press E to access your inventory at any time.";
            if (Input.GetKeyDown(KeyCode.X)){
                DropObject();
            }
            else if (Input.GetKeyDown(KeyCode.C)){
                //put the object in inventory
                grabObject(hand.transform.GetChild(0).gameObject);
            }
        }
        //the player is not holding anything
        else{
            //determines if the player can and should grab the object
            GameObject[] collectable = GameObject.FindGameObjectsWithTag("Collectable");
            foreach (GameObject obj in collectable){
                float distance = (Player.transform.position - obj.transform.position).magnitude;
                    if (distance < grabDistance){
                        pickUpText.text = "Press C to collect " + obj.transform.name;
                        if (Input.GetKeyDown(KeyCode.C)){
                            PickUpObject(obj);
                        }
                    }
                    else{
                        pickUpText.text = "";
                    }
            }
        }
    }     

    void inventoryBoxClicked(int num){
        Debug.Log("Box # " + num + " clicked");

        //puts the selected object in your "hand"
        Debug.Log(hand.transform.transform.childCount);
        if (hand.transform.transform.childCount == 0){
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
    }

    void DropObject(){
        hand.transform.GetChild(0).tag = "Collectable";
        hand.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
        hand.transform.GetChild(0).SetParent(Collectables.transform);
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