using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TalkScript : MonoBehaviour
{
    public TextMeshProUGUI helpText;
    public bool spoke = false;
    public GameObject Player;
    TextMeshProUGUI pickUpText;
    public bool hasBeenSpokenTo = false;
    // Start is called before the first frame update
    void Start()
    {
        GameObject UI = GameObject.Find("Game UI");
        GameObject Panel = UI.transform.Find("Panel").gameObject;
        Transform textTransform = Panel.transform.Find("Help Text");
        helpText = textTransform.GetComponent<TextMeshProUGUI>();
        Transform textTransformPickUp = Panel.transform.Find("PickUpText");
        pickUpText = textTransformPickUp.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player && GameObject.Find("Alien Player")){
            Player = GameObject.Find("Alien Player");
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (Player){
            if (other.transform.name.StartsWith("Alien Player") && spoke == false){
                if (pickUpText.text == ""){
                    helpText.text = "Press T to talk";
                }
                Debug.Log(Player.transform.name);
                Player.GetComponent<NPCManager>().talkingTo = gameObject;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.name.StartsWith("Alien Player")){
            helpText.text = "";
        }
    }
}
