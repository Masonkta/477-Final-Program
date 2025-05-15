using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class setObjectiveText : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;
    public DDOL DDOL;
    // Start is called before the first frame update
    void Start()
    {
        objectiveText = GetComponent<TextMeshProUGUI>();
        DDOL = GameObject.Find("DDOL").GetComponent<DDOL>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DDOL.highestTaskAchieved == GameState.ENTERED_CITY)
            objectiveText.text = "Current Objective:\nExplore the city, gather intel. Be careful of other soldiers.";
        if (DDOL.highestTaskAchieved == GameState.TALKED_TO_COLONEL)
            objectiveText.text = "Current Objective:\nInvestigate the parking garage to avoid suspicion. Maybe you'll find something.";
        if (DDOL.highestTaskAchieved == GameState.TALKED_TO_NPC)
            objectiveText.text = "Current Objective:\nFind the note the doctor is referring to. It is in his backyard.";
        if (DDOL.highestTaskAchieved == GameState.GRABBED_NOTE)
            objectiveText.text = "Current Objective:\nRefer to the note for finding the key. The note is in your inventory.";
        if (DDOL.highestTaskAchieved == GameState.GRABBED_KEY)
            objectiveText.text = "Current Objective:\nWith the key you grabbed, unlock the door to the hideout.";
        else
            objectiveText.text = "Current Objective:\n";

        
    }

}
