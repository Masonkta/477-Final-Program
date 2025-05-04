using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using System.IO;
using System;
using System.Runtime.ExceptionServices;
using UnityEngine.UI;

public class NPCManager : MonoBehaviour
{
    public GameObject talkingTo = null;
    private Vector3 lastPosition;
    public bool isMoving;
    GameObject Player;
    TextMeshProUGUI talkText;
    GameObject talkPanel;
    public GameObject playerCamera;
    public GameObject defaultCameraPos;
    public GameObject talkCameraPos;
    public bool freezeCamera = false;
    public TextMeshProUGUI speakerText;
    List<string> littleGuyDialogue = new List<string> { "Hello There!", "What are you doing here?", "IDK I just woke up" };
    List<string> littleGuySpeakers = new List<string> {"Little Guy", "You", "Little Guy"};
    public GameObject oneOptionPanel;
    public TextMeshProUGUI oneOptionOneText;
    public GameObject twoOptionPanel;
    public TextMeshProUGUI twoOptionOneText;
    public TextMeshProUGUI twoOptionTwoText;
    public GameObject threeOptionPanel;
    public TextMeshProUGUI threeOptionOneText;
    public TextMeshProUGUI threeOptionTwoText;
    public TextMeshProUGUI threeOptionThreeText;
    public string buttonSelection = "";
    public Button oneOptionOneButton;
    public Button twoOptionOneButton;
    public Button twoOptionTwoButton;
    public Button threeOptionOneButton;
    public Button threeOptionTwoButton;
    public Button threeOptionThreeButton;
    public bool talkPanelStatus = false;
    public Coroutine Talking; 
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        Player = GameObject.Find("Alien Player");
        playerCamera = Player.transform.Find("Player Camera").gameObject;
        defaultCameraPos = Player.transform.Find("defaultCameraPos").gameObject;
        talkCameraPos = Player.transform.Find("talkingCameraPos").gameObject;
        GameObject UI = GameObject.Find("Game UI");
        GameObject Panel = UI.transform.Find("Panel").gameObject;
        talkPanel = Panel.transform.Find("NPC Captions").gameObject;
        Transform talkTextTransform = talkPanel.transform.Find("NPC Words");
        Transform speakerTextTransform = talkPanel.transform.Find("Speaker");
        speakerText = speakerTextTransform.GetComponent<TextMeshProUGUI>();
        talkText = talkTextTransform.GetComponent<TextMeshProUGUI>();
        oneOptionPanel = talkPanel.transform.Find("1Options").gameObject;
        twoOptionPanel = talkPanel.transform.Find("2Options").gameObject;
        threeOptionPanel = talkPanel.transform.Find("3Options").gameObject;
        GameObject oneOptionOne = oneOptionPanel.transform.Find("Option1").gameObject;
        GameObject twoOptionOne = twoOptionPanel.transform.Find("Option1").gameObject;
        GameObject twoOptionTwo = twoOptionPanel.transform.Find("Option2").gameObject;
        GameObject threeOptionOne = threeOptionPanel.transform.Find("Option1").gameObject;
        GameObject threeOptionTwo = threeOptionPanel.transform.Find("Option2").gameObject;
        GameObject threeOptionThree = threeOptionPanel.transform.Find("Option3").gameObject;
        oneOptionOneButton = oneOptionOne.transform.Find("Button").GetComponent<Button>();
        twoOptionOneButton = twoOptionOne.transform.Find("Button").GetComponent<Button>();
        twoOptionTwoButton = twoOptionTwo.transform.Find("Button").GetComponent<Button>();
        threeOptionOneButton = threeOptionOne.transform.Find("Button").GetComponent<Button>();
        threeOptionTwoButton = threeOptionTwo.transform.Find("Button").GetComponent<Button>();
        threeOptionThreeButton = threeOptionThree.transform.Find("Button").GetComponent<Button>();
        oneOptionOneButton.onClick.AddListener(() => OnButtonClicked("oneOptionOne"));
        twoOptionOneButton.onClick.AddListener(() => OnButtonClicked("twoOptionOne"));
        twoOptionTwoButton.onClick.AddListener(() => OnButtonClicked("twoOptionTwo"));
        threeOptionOneButton.onClick.AddListener(() => OnButtonClicked("threeOptionOne"));
        threeOptionTwoButton.onClick.AddListener(() => OnButtonClicked("threeOptionTwo"));
        threeOptionThreeButton.onClick.AddListener(() => OnButtonClicked("threeOptionThree"));
        oneOptionOneText = oneOptionOneButton.transform.Find("DialogueOption").GetComponent<TextMeshProUGUI>();
        twoOptionOneText = twoOptionOneButton.transform.Find("DialogueOption").GetComponent<TextMeshProUGUI>();
        twoOptionTwoText = twoOptionTwoButton.transform.Find("DialogueOption").GetComponent<TextMeshProUGUI>();
        threeOptionOneText = threeOptionOneButton.transform.Find("DialogueOption").GetComponent<TextMeshProUGUI>();
        threeOptionTwoText = threeOptionTwoButton.transform.Find("DialogueOption").GetComponent<TextMeshProUGUI>();
        threeOptionThreeText = threeOptionThreeButton.transform.Find("DialogueOption").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        //toggling the cursor based on whether or not the talk panel is active
        if (talkPanel.activeInHierarchy != talkPanelStatus){
            ToggleCursor();
            talkPanelStatus = !talkPanelStatus;
        }

        //ending a conversation
        if (freezeCamera){
            if (speed > 0.01f)
            {
                isMoving = true;
                freezeCamera = false;
                talkingTo.GetComponent<TalkScript>().spoke = false;
                buttonSelection = "";
                if (Talking != null){
                    StopCoroutine(Talking);
                    Talking = null;
                }

                oneOptionOneText.text = "";
                twoOptionOneText.text = "";
                twoOptionTwoText.text = "";
                threeOptionOneText.text = "";
                threeOptionTwoText.text = "";
                threeOptionThreeText.text = "";

                oneOptionPanel.SetActive(false);
                twoOptionPanel.SetActive(false);
                threeOptionPanel.SetActive(false);

                talkText.text = "";
                speakerText.text = "";

                talkPanel.SetActive(false);
            }
            else{
                isMoving = false;
            }
        }
        else{
            isMoving = false;
        }

        // starting a convorsation 
        if (talkingTo != null && isMoving == false && Input.GetKeyDown(KeyCode.T) && talkingTo.GetComponent<TalkScript>().spoke == false){
            freezeCamera = true;
            talkingTo.GetComponent<TalkScript>().spoke = true;
            if (talkingTo.transform.name == "NPC Little Guy"){
                Talking = StartCoroutine(TalkToLittleGuy()); 
            }
        }

        lastPosition = transform.position; 
    }

    void OnButtonClicked(string buttonName)
    {
        buttonSelection = buttonName; 
        //Debug.Log("Button Name: " + buttonName);
    }

    IEnumerator TalkToCharacter(List<String> dialogue, List<String> speakers){
        playerCamera.transform.position = talkCameraPos.transform.position;
        talkPanel.SetActive(true);
        
        for (int i = 0; i < dialogue.Count; i++){
            speakerText.text = speakers[i];
            String displayedText = "";
            foreach(char letter in dialogue[i]){
                displayedText += letter;
                talkText.text = displayedText;
                yield return new WaitForSeconds(0.1f);
            }
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null; // wait for next frame
            }
        }
    
    }

    IEnumerator TalkToLittleGuy()
    {
        //you are talking
        playerCamera.transform.position = talkCameraPos.transform.position;
        talkPanel.SetActive(true);
        twoOptionPanel.SetActive(true);
        speakerText.text = "You";
        twoOptionOneText.text = "Hello! How are you?";
        twoOptionTwoText.text = "Get out of my face.";
        while (buttonSelection == "")
        {
            yield return null; // wait for next frame
        }
        twoOptionPanel.SetActive(false);
        //little guy is talking
        if (buttonSelection == "twoOptionOne")
        {
            buttonSelection = "";
            speakerText.text = "Little Guy";
            String dialogue = "I'm doing great!  And the other thing is, my sister had a baby and I took it over after she passed away and the baby lost all its legs and arms and now its just a stump but I take care of it with my wife. True story.";
            String displayedText = "";
            foreach(char letter in dialogue){
                displayedText += letter;
                talkText.text = displayedText;
                yield return new WaitForSeconds(0.05f);
            }
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null; // wait for next frame
            }
            talkText.text = "";
            //you are talking
            speakerText.text = "You";
            threeOptionPanel.SetActive(true);
            threeOptionOneText.text = "Oh whats its name?";
            threeOptionTwoText.text = "Cool story man.";
            threeOptionThreeText.text = "IDK how you want me to respond to that.";
            while (buttonSelection == "")
            {
                yield return null; // wait for next frame
            }
            threeOptionPanel.SetActive(false);
            //little guy is talking
            if (buttonSelection == "threeOptionOne"){
                speakerText.text = "Little Guy";
                dialogue = "hees name is little shrimp";
                displayedText = "";
                foreach(char letter in dialogue){
                    displayedText += letter;
                    talkText.text = displayedText;
                    yield return new WaitForSeconds(0.05f);
                }
                while (!Input.GetKeyDown(KeyCode.Return))
                {
                    yield return null; // wait for next frame
                }
                talkText.text = "";
            }
            else if (buttonSelection == "threeOptionTwo"){
                speakerText.text = "Little Guy";
                dialogue = "Yep.";
                displayedText = "";
                foreach(char letter in dialogue){
                    displayedText += letter;
                    talkText.text = displayedText;
                    yield return new WaitForSeconds(0.05f);
                }
                while (!Input.GetKeyDown(KeyCode.Return))
                {
                    yield return null; // wait for next frame
                }
                talkText.text = "";
            }
            else if (buttonSelection == "threeOptionThree"){
                speakerText.text = "Little Guy";
                dialogue = "...";
                displayedText = "";
                foreach(char letter in dialogue){
                    displayedText += letter;
                    talkText.text = displayedText;
                    yield return new WaitForSeconds(0.05f);
                }
                while (!Input.GetKeyDown(KeyCode.Return))
                {
                    yield return null; // wait for next frame
                }
                talkText.text = "";
            }
        }
        //little guy is talking
        else if (buttonSelection == "twoOptionTwo")
        {
            buttonSelection = "";
            String dialogue = "Ah! A territorial display! Fascinating. I shall recalibrate my proximity protocols and hover... respectfully.";
            speakerText.text = "Little Guy";
            String displayedText = "";
            foreach(char letter in dialogue){
                displayedText += letter;
                talkText.text = displayedText;
                yield return new WaitForSeconds(0.05f);
            }
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null; // wait for next frame
            }
            talkText.text = "";
            //you are talking
            speakerText.text = "You";
            oneOptionPanel.SetActive(true);
            oneOptionOneText.text = "Woah now I'll back off.";
            while (buttonSelection == "")
            {
                yield return null; // wait for next frame
            }
            oneOptionPanel.SetActive(false);
            if (buttonSelection == "oneOptionOne"){
                speakerText.text = "Little Guy";
                dialogue = "See you around man.";
                displayedText = "";
                foreach(char letter in dialogue){
                    displayedText += letter;
                    talkText.text = displayedText;
                    yield return new WaitForSeconds(0.05f);
                }
                while (!Input.GetKeyDown(KeyCode.Return))
                {
                    yield return null; // wait for next frame
                }
                talkText.text = "";
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
}
