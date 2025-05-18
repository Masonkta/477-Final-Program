using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class lastSceneCanvasController : MonoBehaviour
{
    public Image blackFadeInScreen;
    bool resultsPanelActive;

    [Header("Results")]
    public Image resultsBG;
    public Image grayBG;
    public Image nameEnterArea;
    public TextMeshProUGUI congratsTxt;
    public TextMeshProUGUI enterNameTxt;
    public TextMeshProUGUI timeTxt;
    public TextMeshProUGUI resetsTxt;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI tempEnteringText;
    public GameObject nameTypeBox;
    public DDOL DDOL;



    // Start is called before the first frame update
    void Start()
    {
        DDOL = GameObject.Find("DDOL").GetComponent<DDOL>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        handleFadeIn();
        resultsStuff();
    }

    void handleFadeIn()
    {
        if (!blackFadeInScreen.gameObject.activeInHierarchy)
            return;

        if (blackFadeInScreen.color.a > 0.02f)
        {
            Color temp = blackFadeInScreen.color; temp.a -= Time.deltaTime / 5f; // 5 sec fade in
            blackFadeInScreen.color = temp;
        }
        else
            blackFadeInScreen.gameObject.SetActive(false);

    }

    void resultsStuff()
    {
        if (Time.timeSinceLevelLoad > 5.5f && !resultsPanelActive)
            StartCoroutine(loadInPanel());
        if (Time.timeSinceLevelLoad < 5.5f)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    IEnumerator loadInPanel()
    {   
        resultsPanelActive = true;

        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Color temp = resultsBG.color; temp.a = t * 0.765f; resultsBG.color = temp;
            temp = grayBG.color; temp.a = t * 0.867f; grayBG.color = temp;
            temp = nameEnterArea.color; temp.a = t * 0.27f; nameEnterArea.color = temp;
            temp = tempEnteringText.color; temp.a = t * 0.71f; tempEnteringText.color = temp;
            temp = congratsTxt.color; temp.a = t; congratsTxt.color = temp;
            temp = timeTxt.color; temp.a = t; timeTxt.color = temp;
            temp = resetsTxt.color; temp.a = t; resetsTxt.color = temp;
            temp = scoreTxt.color; temp.a = t; scoreTxt.color = temp;
            temp = enterNameTxt.color; temp.a = t; enterNameTxt.color = temp;

            yield return null;
        }
        nameTypeBox.SetActive(true);
    }
}
