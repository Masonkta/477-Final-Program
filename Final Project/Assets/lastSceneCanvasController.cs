using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class lastSceneCanvasController : MonoBehaviour
{
    public Image blackFadeInScreen;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        handleFadeIn();

    }

    void handleFadeIn()
    {
        if (!blackFadeInScreen.gameObject.activeInHierarchy)
            return;

        if (blackFadeInScreen.color.a > 0.02f)
            {
                Color temp = blackFadeInScreen.color; temp.a -= Time.deltaTime / 4f;
                blackFadeInScreen.color = temp;
            }
            else
                blackFadeInScreen.gameObject.SetActive(false);

    }
}
