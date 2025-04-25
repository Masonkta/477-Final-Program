using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using System.IO;
using System;
using System.Runtime.ExceptionServices;

public class NPCManager : MonoBehaviour
{
    public GameObject talkingTo = null;
    private Vector3 lastPosition;
    public bool isMoving;
    Coroutine talking;
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
        Debug.Log(talkPanel.transform.name);
        Transform talkTextTransform = talkPanel.transform.Find("NPC Words");
        Transform speakerTextTransform = talkPanel.transform.Find("Speaker");
        speakerText = speakerTextTransform.GetComponent<TextMeshProUGUI>();
        talkText = talkTextTransform.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        if (freezeCamera){
            if (speed > 0.01f)
            {
                isMoving = true;
                freezeCamera = false;
                talkingTo.GetComponent<TalkScript>().spoke = false;
                if (talking != null){
                    StopCoroutine(talking);
                }
                //playerCamera.transform.position = defaultCameraPos.transform.position;
                if (talkPanel.activeInHierarchy){
                    talkPanel.SetActive(false);
                }
            }
            else{
                isMoving = false;
            }
        }
        else{
            isMoving = false;
        }

        if (talkingTo != null && isMoving == false && Input.GetKeyDown(KeyCode.T) && talkingTo.GetComponent<TalkScript>().spoke == false){
            freezeCamera = true;
            talkingTo.GetComponent<TalkScript>().spoke = true;
            if (talkingTo.transform.name == "NPC Little Guy"){
                Coroutine talking = StartCoroutine(TalkToCharacter(littleGuyDialogue, littleGuySpeakers)); 
            }
        }

        lastPosition = transform.position; 
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
}
