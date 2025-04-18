using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIClicker : MonoBehaviour
{
    public Camera playerCamera;
    private Button currentlyHoveredButton;
    public Animator m_Animator;

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
        Debug.Log("START");
        m_Animator.SetTrigger("Salute");
    }

    public void OpenOptions()
    {
        Debug.Log("Options");
    }
}
