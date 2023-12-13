using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DiamondHover : MonoBehaviour
{
    [SerializeField] GameObject panel; 
    void Start()
    {
        panel = transform.GetChild(0).gameObject;
        panel.SetActive(false);
        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry onPointerEnter = new EventTrigger.Entry();
        EventTrigger.Entry onPointerExit = new EventTrigger.Entry();
        onPointerEnter.eventID = EventTriggerType.PointerEnter;
        onPointerExit.eventID = EventTriggerType.PointerExit;
        onPointerEnter.callback.AddListener((pointerEventData) => { OnPointerEnter((PointerEventData)pointerEventData); });
        onPointerExit.callback.AddListener((pointerEventData) => { OnPointerExit((PointerEventData)pointerEventData); });
        eventTrigger.triggers.Add(onPointerEnter);
        eventTrigger.triggers.Add(onPointerExit);
    }

    void OnPointerEnter(PointerEventData pointerEventData)
    {
        panel.SetActive(true);
    }
    void OnPointerExit(PointerEventData pointerEventData)
    {
        panel.SetActive(false);
    }
    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}
