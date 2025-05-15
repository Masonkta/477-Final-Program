using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class setObjectiveText : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;
    public DDOL DDOL;
    public GameObject inventoryPanel;
    // Start is called before the first frame update
    void Start()
    {
        objectiveText = GetComponent<TextMeshProUGUI>();
        DDOL = GameObject.Find("DDOL").GetComponent<DDOL>();
    }

    // Update is called once per frame
    void Update()
    {
        hideObjectiveTextIfInventoryIsOpen();


        if (DDOL)
        {
            if (DDOL.highestTaskAchieved == GameState.ENTERED_CITY)
                objectiveText.text = "Current Objective:\nExplore the city, gather intel. Keep distance from marching soldiers.";
            else if (DDOL.highestTaskAchieved == GameState.TALKED_TO_COLONEL)
                objectiveText.text = "Current Objective:\nInvestigate the parking garage as instructed. Maybe you'll find something.";
            else if (DDOL.highestTaskAchieved == GameState.TALKED_TO_NPC)
                if (DDOL.hasEverTalkedToFirstDoctorInStartScene)
                    objectiveText.text = "Current Objective:\nFind the note the doctor is referring to. Remember he said he lived in a two story purple house.";
                else
                    objectiveText.text = "Current Objective:\nFind the note the doctor is referring to.";
            else if (DDOL.highestTaskAchieved == GameState.GRABBED_NOTE)
                objectiveText.text = "Current Objective:\nRefer to the note for finding the key. The note is in your inventory.";
            else if (DDOL.highestTaskAchieved == GameState.GRABBED_KEY)
                objectiveText.text = "Current Objective:\nWith the key you grabbed, unlock the door to the hideout. Check the map for location";
            else
                objectiveText.text = "You shouldn't see this.";
        }

        else
            objectiveText.text = "NO DDOL!";

    }

    void hideObjectiveTextIfInventoryIsOpen()
    {
        objectiveText.enabled = !inventoryPanel.activeInHierarchy;
    }

}
