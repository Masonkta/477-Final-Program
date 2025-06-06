using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class miniMapScript : MonoBehaviour
{
    Button mapButton;
    bool mapButtonClicked;
    public GameObject miniMapCamera;
    public DDOL DDOL;
    public GameObject xOnMap;
    // Start is called before the first frame update
    void Start()
    {
        GameObject UI = GameObject.Find("Game UI");
        GameObject MiniMap = UI.transform.Find("MiniMap").gameObject;
        mapButton = MiniMap.transform.Find("MiniMap Button").GetComponent<Button>();
        mapButton.onClick.AddListener(toggleMap);
        miniMapCamera = GameObject.Find("minimapCamera");
        miniMapCamera.SetActive(false);

        DDOL = GameObject.Find("DDOL").GetComponent<DDOL>();
        print(gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) || mapButtonClicked)
        {
            if (miniMapCamera.activeInHierarchy)
            {
                miniMapCamera.SetActive(false);
            }
            else
            {
                miniMapCamera.SetActive(true);
            }
            mapButtonClicked = false;
        }

        if (DDOL)
            if (DDOL.highestTaskAchieved == GameState.EXIT)
                miniMapCamera.SetActive(false);

        if (DDOL) xOnMap.SetActive(DDOL.highestTaskAchieved == GameState.GRABBED_KEY && miniMapCamera.activeInHierarchy);
    }

    void toggleMap(){
        mapButtonClicked = true;
    }

    float Map(float value, float start1, float end1, float start2, float end2)
    {
        return (value - start1) / (end1 - start1) * (end2 - start2) + start2;
    }
}
