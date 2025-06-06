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
    public bool mouseClicked = false;
    String dialogue;
    String displayedText;
    public GameObject GameHandler;
    public GameObject TimeManager;
    public DDOL DDOL;
    Coroutine typing;
    //public bool canLeave = true;
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
        GameHandler = GameObject.Find("GameHandler");
        TimeManager = GameObject.Find("TimeManager");
        DDOL = GameObject.Find("DDOL").GetComponent<DDOL>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        if (speed > 0.01f && !GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
        else if (!(speed > 0.01f) && GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Stop();
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            GetComponent<AudioSource>().pitch = 1.4f;
        }
        else
        {
            GetComponent<AudioSource>().pitch = 0.5f;
        }

        //toggling the cursor based on whether or not the talk panel is active
        if (talkPanel.activeInHierarchy != talkPanelStatus)
        {
            ToggleCursor();
            talkPanelStatus = !talkPanelStatus;
        }

        //ending a conversation
        if (freezeCamera)
        {
            if (speed > 0.01f)
            {
                isMoving = true;
                endConversation();
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            isMoving = false;
        }

        // starting a convorsation 
        if (talkingTo != null && isMoving == false && Input.GetKeyDown(KeyCode.T) && talkingTo.GetComponent<TalkScript>().spoke == false)
        {
            freezeCamera = true;
            Vector3 playerChest = Player.transform.position;
            playerChest.y = talkingTo.transform.parent.position.y;
            talkingTo.transform.parent.LookAt(playerChest);

            Vector3 NPCChest = talkingTo.transform.parent.position;
            NPCChest.y = Player.transform.position.y;
            Player.transform.LookAt(NPCChest);
            DDOL.NPCVisits += 1;
            StartCoroutine(leaveDelay());
            if (!talkingTo.GetComponent<AudioSource>().isPlaying)
            {
                talkingTo.GetComponent<AudioSource>().Play();
            }

            talkingTo.GetComponent<TalkScript>().spoke = true;
            if (talkingTo.transform.name == "NPC Doctor")
            {
                Talking = StartCoroutine(TalkToDoctor());
            }
            else if (talkingTo.transform.name.StartsWith("NPC Guard"))
            {
                Talking = StartCoroutine(TalkToGuard());
            }
            else if (talkingTo.transform.name == "NPC Key Guard")
            {
                Talking = StartCoroutine(TalkToKeyGuard());
            }
            else if (talkingTo.transform.name == "NPC Good Guy")
            {
                Talking = StartCoroutine(talkToHuman());
            }
            else if (talkingTo.transform.name == "NPC Human")
            {
                Talking = StartCoroutine(talkToHider());
            }
            else if (talkingTo.transform.name == "NPC Special Human")
            {
                Talking = StartCoroutine(talkToSpecialHuman());
            }
        }

        lastPosition = transform.position;

        if (Input.GetMouseButtonDown(0))
        {
            mouseClicked = true;
        }

    }

    void OnButtonClicked(string buttonName)
    {
        buttonSelection = buttonName;
        Debug.Log("Button Name: " + buttonName);
    }

    IEnumerator leaveDelay()
    {
        Player.GetComponent<playerMovement>().enabled = false;
        yield return new WaitForSeconds(1f);
        Player.GetComponent<playerMovement>().enabled = true;
    }
    void endConversation()
    {
        freezeCamera = false;
        talkingTo.GetComponent<TalkScript>().spoke = false;
        buttonSelection = "";
        if (talkingTo.GetComponent<AudioSource>().isPlaying)
        {
            talkingTo.GetComponent<AudioSource>().Stop();
        }
        if (typing != null)
        {
            StopCoroutine(typing);
        }
        talkingTo.GetComponent<TalkScript>().hasBeenSpokenTo = true;
        if (Talking != null)
        {
            StopCoroutine(Talking);
            Talking = null;
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
            dialogue = "";
            displayedText = "";
            talkingTo = null;
        }
    }

    IEnumerator TalkToCharacter(List<String> dialogue, List<String> speakers)
    {
        playerCamera.transform.position = talkCameraPos.transform.position;
        talkPanel.SetActive(true);

        for (int i = 0; i < dialogue.Count; i++)
        {
            speakerText.text = speakers[i];
            String displayedText = "";
            foreach (char letter in dialogue[i])
            {
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

    IEnumerator talkToHider()
    {
        yield return null;
        //you are talking
        playerCamera.transform.position = talkCameraPos.transform.position;
        talkPanel.SetActive(true);

        //you are talking
        speakerText.text = "You";
        oneOptionPanel.SetActive(true);
        oneOptionOneText.text = "Don't be afraid! I come in peace.";
        while (buttonSelection == "")
        {
            yield return null; // wait for next frame
        }
        mouseClicked = false;
        oneOptionPanel.SetActive(false);
        if (buttonSelection == "oneOptionOne")
        {
            speakerText.text = "Private";
            if (talkingTo.GetComponent<TalkScript>().hasBeenSpokenTo == false)
            {
                List<string> dialogueLines = new List<string>
                {
                    "No! Get away! I hate you!",
                    "You may be terraforming but you'll never squash the resistance!",
                    "Wha wha bu bu huhhh get away",
                    "I want my mommy.",
                    "I'll never tell you anything.",
                    "There's a suspicious guy in that parking garage maybe he knows something. Please don't kill me!"
                };

                int index = UnityEngine.Random.Range(0, dialogueLines.Count);

                String dialogue = dialogueLines[index];
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
            }
            else
            {
                String dialogue = "Please leave me alone.";
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
            }
        }
        talkingTo.GetComponent<TalkScript>().hasBeenSpokenTo = true;
        endConversation();
    }

    IEnumerator TalkToGuard()
    {
        //you are talking
        playerCamera.transform.position = talkCameraPos.transform.position;
        talkPanel.SetActive(true);

        //you are talking
        speakerText.text = "You";
        oneOptionPanel.SetActive(true);
        oneOptionOneText.text = "Can you help me find the human base?";
        while (buttonSelection == "")
        {
            yield return null; // wait for next frame
        }
        mouseClicked = false;
        oneOptionPanel.SetActive(false);
        if (buttonSelection == "oneOptionOne")
        {
            speakerText.text = "Private";
            if (talkingTo.GetComponent<TalkScript>().hasBeenSpokenTo == false)
            {
                List<string> dialogueLines = new List<string>
                {
                    "Get back to your orders, solider!",
                    "I’m currently on a mission to seize supplies. Do not interrupt me again.",
                    "All hail Colonel Black!",
                    "Down with humans and their greed! Their total elimination was a necessary part of our plan.",
                    "Don’t stray far from base. Trust is hard to keep these days...",
                    "You must be new, solider. Be mindful of your rank. Remember that the leutenient and colonel are your superiors.",
                    "To defend a human is to forsake your own species, brother.",
                    "The brothers are playing a round of chicken jockey tonight, I won’t miss out this time.",
                    "This planet is much safer under our rule, as we are the apex predators of mankind.",
                    "I don’t know why we’re keeping that scientist human alive. We should have killed him on sight like all of the other ones…",
                    "I’ll take on any assignment, except for guarding the scientist. I don’t know why we’re even keeping him alive."
                };

                int index = UnityEngine.Random.Range(0, dialogueLines.Count);

                String dialogue = dialogueLines[index];
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
            }
            else
            {
                String dialogue = "Why are you so curious. I'm going to have to report you.";
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
                //THIS HAS AROUSED SUSPICION
                //DDOL.resets += 1;
                GameHandler.GetComponent<CountDownScript>().alienSusMeter = 1f;
                GameHandler.transform.Find("Alarm").GetComponent<AudioSource>().Play();
                GameHandler.GetComponent<CountDownScript>().captured = true;
            }
        }
        talkingTo.GetComponent<TalkScript>().hasBeenSpokenTo = true;
        endConversation();
    }

    IEnumerator TalkToKeyGuard()
    {
        //you are talking
        playerCamera.transform.position = talkCameraPos.transform.position;
        talkPanel.SetActive(true);

        //you are talking
        speakerText.text = "You";
        oneOptionPanel.SetActive(true);
        oneOptionOneText.text = "Can you help me find the human base?";
        while (buttonSelection == "")
        {
            yield return null; // wait for next frame
        }
        mouseClicked = false;
        oneOptionPanel.SetActive(false);
        if (buttonSelection == "oneOptionOne")
        {
            speakerText.text = "Private";
            String dialogue = "Why do you need to find the human base?";
            typing = StartCoroutine(TypeText(dialogue, talkText));
            yield return typing;
        }

        //you are talking
        buttonSelection = "";
        speakerText.text = "You";
        oneOptionPanel.SetActive(true);
        oneOptionOneText.text = "I have orders from the Captain.";
        while (buttonSelection == "")
        {
            yield return null; // wait for next frame
        }
        mouseClicked = false;
        oneOptionPanel.SetActive(false);
        if (buttonSelection == "oneOptionOne")
        {
            speakerText.text = "Private";
            String dialogue = "A strange request from the captain. What is your rank again, solider?";
            typing = StartCoroutine(TypeText(dialogue, talkText));
            yield return typing;
        }
        //you are talking
        buttonSelection = "";
        twoOptionPanel.SetActive(true);
        speakerText.text = "You";
        twoOptionOneText.text = "I am a private.";
        twoOptionTwoText.text = "I am your Colonel. How dare you question me!";
        while (buttonSelection == "")
        {
            yield return null; // wait for next frame
        }
        mouseClicked = false;
        twoOptionPanel.SetActive(false);

        //little guy is talking
        if (buttonSelection == "twoOptionOne")
        {
            buttonSelection = "";
            speakerText.text = "Private";
            String dialogue = "Well the base was cleared many years ago, I doubt you’ll find any maps on base.";
            typing = StartCoroutine(TypeText(dialogue, talkText));
            yield return typing;

            //you are talking
            buttonSelection = "";
            speakerText.text = "You";
            oneOptionPanel.SetActive(true);
            oneOptionOneText.text = "Try Me.";
            while (buttonSelection == "")
            {
                yield return null; // wait for next frame
            }
            mouseClicked = false;
            oneOptionPanel.SetActive(false);
            if (buttonSelection == "oneOptionOne")
            {
                speakerText.text = "Private";
                dialogue = "Nevermind that. Go clear the parking garage private. There have been complaints of rebel scum hiding out there.";

                // THIS IS WHEN WE SWITCH STATES TO VISIT PARKING GARAGE
                if (DDOL)
                    if (DDOL.highestTaskAchieved == GameState.ENTERED_CITY)
                    {
                        DDOL.awardPointsForTask();
                        DDOL.highestTaskAchieved = GameState.TALKED_TO_COLONEL; // NOW WE TALKED TO COLONEL
                        DDOL.currentState = GameState.TALKED_TO_COLONEL;
                    }

                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
            }

        }
        //little guy is talking
        if (buttonSelection == "twoOptionTwo")
        {
            //THIS HAS AROUSED SUSPICION
            buttonSelection = "";
            speakerText.text = "Private";
            String dialogue = "You don't look like a Colonel. I'm going to have to call this in.";
            typing = StartCoroutine(TypeText(dialogue, talkText));
            yield return typing;
            //DDOL.resets += 1;
            GameHandler.GetComponent<CountDownScript>().alienSusMeter = 1f;
            GameHandler.transform.Find("Alarm").GetComponent<AudioSource>().Play();
            GameHandler.GetComponent<CountDownScript>().captured = true;
        }
        talkingTo.GetComponent<TalkScript>().hasBeenSpokenTo = true;
        endConversation();
    }

    IEnumerator TalkToDoctor()
    {
        //you are talking
        playerCamera.transform.position = talkCameraPos.transform.position;
        talkPanel.SetActive(true);
        if (DDOL)
        {
            if (!DDOL.hasEverTalkedToFirstDoctorInStartScene) DDOL.awardPointsForTask();
            DDOL.hasEverTalkedToFirstDoctorInStartScene = true;
        }

        if (TimeManager.GetComponent<TimerScript>().hasTryedAtLeastOnce == true)
        {
            List<string> dialogueLines = new List<string>
            {
                "It takes some time to get used to a new body. You'll get em next time. ",
                "Remember that this is a costly procedure for the resistance. Try to make the most of it.",
                "Thank you for your service. Get back in that chair and we can try again.",
                "You should visit my house when you go back. It's a beautiful purple two story home. My wife and kids are dead but the house was cool."
            };
            int index = UnityEngine.Random.Range(0, dialogueLines.Count);
            String dialogue = dialogueLines[index];
            typing = StartCoroutine(TypeText(dialogue, talkText));
            yield return typing;
        }
        else
        {
            //you are talking
            speakerText.text = "You";
            oneOptionPanel.SetActive(true);
            oneOptionOneText.text = "Where am I?";
            while (buttonSelection == "")
            {
                yield return null; // wait for next frame
            }
            mouseClicked = false;
            oneOptionPanel.SetActive(false);
            if (buttonSelection == "oneOptionOne")
            {
                speakerText.text = "Doctor";
                dialogue = "Woah take it slow. You just woke up. You're safe. You must be disoriented from the last mission.";
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
            }

            //you are talking
            speakerText.text = "You";
            buttonSelection = "";
            oneOptionPanel.SetActive(true);
            oneOptionOneText.text = "What mission?";
            while (buttonSelection == "")
            {
                yield return null; // wait for next frame
            }
            mouseClicked = false;
            oneOptionPanel.SetActive(false);
            if (buttonSelection == "oneOptionOne")
            {
                speakerText.text = "Doctor";
                dialogue = "Three years ago, the first troops showed up and took out our major power centers. They've been terraforming ever since... getting stronger. ";
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
            }

            //you are talking 
            twoOptionPanel.SetActive(true);
            speakerText.text = "You";
            buttonSelection = "";
            twoOptionOneText.text = "What Key?";
            twoOptionTwoText.text = "Why do you need me?";
            while (buttonSelection == "")
            {
                yield return null; // wait for next frame
            }
            mouseClicked = false;
            twoOptionPanel.SetActive(false);
            //little guy is talking
            if (buttonSelection == "twoOptionOne")
            {
                buttonSelection = "";
                speakerText.text = "Doctor";
                dialogue = "That's on a need to know basis, Private.";
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
            }
            speakerText.text = "Doctor";
            dialogue = "We found a weak link in their forces and have penatrated the digital conciousness through time. We can only give you five minutes in that conciousness before we have to reset you.";
            typing = StartCoroutine(TypeText(dialogue, talkText));
            yield return typing;

            speakerText.text = "Doctor";
            dialogue = "You should visit my house when you go back. It's a beautiful purple two story home. My wife and kids are dead but the house was cool.";
            typing = StartCoroutine(TypeText(dialogue, talkText));
            yield return typing;

            //you are talking
            speakerText.text = "You";
            buttonSelection = "";
            oneOptionPanel.SetActive(true);
            oneOptionOneText.text = "So what do you need me to do?";
            while (buttonSelection == "")
            {
                yield return null; // wait for next frame
            }
            mouseClicked = false;
            oneOptionPanel.SetActive(false);
            if (buttonSelection == "oneOptionOne")
            {
                speakerText.text = "Doctor";
                dialogue = "There was a small resistance group still intact at this time that we were able to access. Your mission is to find their base and report back here.";
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;

                speakerText.text = "Doctor";
                dialogue = "Once you get back in the chair we can get started. Hop to it.";
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
            }
        }
        talkingTo.GetComponent<TalkScript>().hasBeenSpokenTo = true;
        endConversation();
    }

    IEnumerator talkToSpecialHuman()
    {
        playerCamera.transform.position = talkCameraPos.transform.position;
        talkPanel.SetActive(true);

        if (talkingTo.GetComponent<TalkScript>().hasBeenSpokenTo)
        {
            speakerText.text = "Brett";
            dialogue = "Why are you still talking to me lol.";
            typing = StartCoroutine(TypeText(dialogue, talkText));
            yield return typing;
        }
        else
        {
            speakerText.text = "Brett";
            dialogue = "Stay back. I don’t care if you’re a trooper or one of their pets. I’ll break your faceplate open.";
            typing = StartCoroutine(TypeText(dialogue, talkText));
            yield return typing;
            //you are talking 
            twoOptionPanel.SetActive(true);
            speakerText.text = "You";
            buttonSelection = "";
            twoOptionOneText.text = "Easy. I’m not here to report. I’m looking for... Dr. Hail. That ring a bell?";
            twoOptionTwoText.text = "Wait! Pizza. Uhh... New York pizza. You miss it too, right?";
            while (buttonSelection == "")
            {
                yield return null; // wait for next frame
            }
            mouseClicked = false;
            twoOptionPanel.SetActive(false);
            //little guy is talking
            if (buttonSelection == "twoOptionOne")
            {
                buttonSelection = "";
                speakerText.text = "Brett";
                dialogue = "That’s my uncle… wait… how do you know him?";
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;

                //you are talking 
                twoOptionPanel.SetActive(true);
                speakerText.text = "You";
                buttonSelection = "";
                twoOptionOneText.text = "I know him... or will know him.";
                twoOptionTwoText.text = "That's not important.";
                while (buttonSelection == "")
                {
                    yield return null; // wait for next frame
                }
                mouseClicked = false;
                twoOptionPanel.SetActive(false);
                //little guy is talking
                if (buttonSelection == "twoOptionOne")
                {
                    buttonSelection = "";
                    speakerText.text = "Brett";
                    dialogue = "What? Thats trippy. Last I heard he was on the roof of the parking garage. Better catch him.";
                    typing = StartCoroutine(TypeText(dialogue, talkText));
                    yield return typing;
                }
                if (buttonSelection == "twoOptionTwo")
                {
                    buttonSelection = "";
                    speakerText.text = "Brett";
                    dialogue = "The hell it's not? Get away from me!";
                    typing = StartCoroutine(TypeText(dialogue, talkText));
                    yield return typing;
                }
            }
            if (buttonSelection == "twoOptionTwo")
            {
                buttonSelection = "";
                speakerText.text = "Brett";
                dialogue = "…What toppings?";
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;

                //you are talking 
                twoOptionPanel.SetActive(true);
                speakerText.text = "You";
                buttonSelection = "";
                twoOptionOneText.text = "Pepperoni";
                twoOptionTwoText.text = "Anchovies";
                while (buttonSelection == "")
                {
                    yield return null; // wait for next frame
                }
                mouseClicked = false;
                twoOptionPanel.SetActive(false);
                //little guy is talking
                if (buttonSelection == "twoOptionOne")
                {
                    buttonSelection = "";
                    speakerText.text = "Brett";
                    dialogue = "Ok Thats cool man";
                    typing = StartCoroutine(TypeText(dialogue, talkText));
                    yield return typing;
                }
                if (buttonSelection == "twoOptionTwo")
                {
                    buttonSelection = "";
                    speakerText.text = "Brett";
                    dialogue = "Even if you aren't an alien, you must be a sociopath. Get away, scum!";
                    typing = StartCoroutine(TypeText(dialogue, talkText));
                    yield return typing;
                }
            }
        }
        
        talkingTo.GetComponent<TalkScript>().hasBeenSpokenTo = true;
        endConversation();
    }

    IEnumerator talkToHuman()
    {
        playerCamera.transform.position = talkCameraPos.transform.position;
        talkPanel.SetActive(true);
        if (talkingTo.GetComponent<TalkScript>().hasBeenSpokenTo)
        {
            speakerText.text = "Human";
            dialogue = "What are you doing standing around here? Go find my note it's in my backyard. Don't make me regret this.";
            // THIS IS WHEN WE SWITCH STATES TO VISIT PARKING GARAGE
            // if (DDOL)
            //     if (DDOL.highestTaskAchieved == GameState.ENTERED_CITY || DDOL.highestTaskAchieved == GameState.TALKED_TO_COLONEL)
            //         // DO NOT AWARD POINTS HERE
            //         DDOL.highestTaskAchieved = GameState.TALKED_TO_NPC; // NOW WE TALKED TO NPC
            typing = StartCoroutine(TypeText(dialogue, talkText));
            yield return typing;
        }
        else
        {
            speakerText.text = "Human";
            dialogue = "Wait! No! Get Away!";
            typing = StartCoroutine(TypeText(dialogue, talkText));
            yield return typing;

            //you are talking
            speakerText.text = "You";
            buttonSelection = "";
            oneOptionPanel.SetActive(true);
            oneOptionOneText.text = "Wait, I’m not here to hurt you! I promise, I come in peace.";
            while (buttonSelection == "")
            {
                yield return null; // wait for next frame
            }
            mouseClicked = false;
            oneOptionPanel.SetActive(false);
            if (buttonSelection == "oneOptionOne")
            {
                speakerText.text = "Human";
                dialogue = "How can we trust you? You’ve killed thousands of our kind!";
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
            }

            //you are talking
            speakerText.text = "You";
            buttonSelection = "";
            oneOptionPanel.SetActive(true);
            oneOptionOneText.text = "I’m human, just like you. I’m posing as a soldier in the alien army.";
            while (buttonSelection == "")
            {
                yield return null; // wait for next frame
            }
            mouseClicked = false;
            oneOptionPanel.SetActive(false);
            if (buttonSelection == "oneOptionOne")
            {
                speakerText.text = "Human";
                dialogue = "So why are you even here, if you really are human?";
                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
            }

            //you are talking
            speakerText.text = "You";
            buttonSelection = "";
            oneOptionPanel.SetActive(true);
            oneOptionOneText.text = "I’ve come from the future to find research that will save the human race. You sent me.";
            while (buttonSelection == "")
            {
                yield return null; // wait for next frame
            }
            mouseClicked = false;
            oneOptionPanel.SetActive(false);
            if (buttonSelection == "oneOptionOne")
            {
                speakerText.text = "Human";
                dialogue = "Good enough for me. I left some old notes of mine in my backyard. If you're really from the future you'll know where that is.";

                // THIS IS WHEN WE SWITCH STATES TO VISIT PARKING GARAGE
                if (DDOL)
                    if (DDOL.highestTaskAchieved == GameState.ENTERED_CITY || DDOL.highestTaskAchieved == GameState.TALKED_TO_COLONEL)
                    {
                        DDOL.awardPointsForTask();
                        DDOL.highestTaskAchieved = GameState.TALKED_TO_NPC; // NOW WE TALKED TO NPC
                        DDOL.currentState = GameState.TALKED_TO_NPC;
                    }

                typing = StartCoroutine(TypeText(dialogue, talkText));
                yield return typing;
            }
        }
        talkingTo.GetComponent<TalkScript>().hasBeenSpokenTo = true;
        endConversation();
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
    





    public IEnumerator TypeText(string dialogue, TextMeshProUGUI targetText)
    {
        string displayedText = "";
        foreach (char letter in dialogue)
        {
            displayedText += letter;
            targetText.text = displayedText;

            // Wait a scaled duration for the text
            if (DDOL)
                yield return new WaitForSeconds(0.04f / DDOL.textReadSpeedMultiplier);
            else
                yield return new WaitForSeconds(0.04f);


            if (mouseClicked) // allow skipping
            {
                mouseClicked = false;
                break;
            }
        }
        targetText.text = dialogue;

        yield return new WaitForSeconds(0.375f); // slight delay before continuing

        while (!Input.GetMouseButtonDown(0)) // wait for player click to proceed
        {
            yield return null;
        }
        mouseClicked = false;
        targetText.text = "";
    }
}