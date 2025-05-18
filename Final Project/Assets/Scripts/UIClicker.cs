using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class UIClicker : MonoBehaviour
{
    public Camera playerCamera;
    private Button currentlyHoveredButton;
    public Animator m_Animator;
    public DDOL godScript;
    public GameObject optionsCanvas;
    public HeadLock headLockScript;
    private bool optionsOpen = false;
    void Start()
    {
        godScript = GameObject.Find("DDOL").GetComponent<DDOL>();
    }

    void Update()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Screen.width / 2, Screen.height / 2)
        };

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);
        Button hoveredButton = null;

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("UIButton"))
            {
                hoveredButton = result.gameObject.GetComponent<Button>();

                if (Input.GetMouseButtonDown(0))
                {
                    hoveredButton.onClick.Invoke();
                }
                break; 
            }
        }

        if (hoveredButton != currentlyHoveredButton)
        {
            if (currentlyHoveredButton != null)
            {
                SetButtonAlpha(currentlyHoveredButton, 0f); 
            }
            if (hoveredButton != null)
            {
                SetButtonAlpha(hoveredButton, 0.2f);
            }
            currentlyHoveredButton = hoveredButton;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsOpen)
            {
                CloseOptions();
            }
        }

}

    void SetButtonAlpha(Button button, float alpha)
    {
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            Color c = image.color;
            c.a = alpha;
            image.color = c;
        }
    }


    public void StartGame()
    {
        if (godScript.highestTaskAchieved == GameState.WOKE_UP)
            godScript.highestTaskAchieved = GameState.ENTERED_CITY;
            godScript.currentState = GameState.ENTERED_CITY;

        Debug.Log("START");
        StartCoroutine(PlaySaluteAndLoadScene());
    }

    private IEnumerator PlaySaluteAndLoadScene()
    {
        m_Animator.SetTrigger("Salute");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("City Scene");
    }

    public void OpenOptions()
    {
        Debug.Log("Opening Options Menu");

        optionsCanvas.SetActive(true);
        headLockScript.inputEnabled = false;
        optionsOpen = true;
    }

    public void CloseOptions()
    {

        optionsCanvas.SetActive(false);  
        headLockScript.inputEnabled = true;
        optionsOpen = false;
    }
    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #endif
    }
}
