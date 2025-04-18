using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TalkScript : MonoBehaviour
{
    public TextMeshProUGUI helpText;
    public bool spoke = false;
    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Alien Player");
        GameObject UI = GameObject.Find("Game UI");
        GameObject Panel = UI.transform.Find("Panel").gameObject;
        Transform textTransform = Panel.transform.Find("Help Text");
        helpText = textTransform.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.name.StartsWith("Alien Player") && spoke == false){
            helpText.text = "Press T to talk";
            spoke = true;
            Player.GetComponent<NPCManager>().talkingTo = gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.name.StartsWith("Alien Player")){
            helpText.text = "";
        }
    }
}
