using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Logo : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] string url;
    [SerializeField] bool enable;
    Vector3 scaleOriginal = Vector3.one;
    [SerializeField] Vector3 scaleForHover;
    [Range(0,1)]
    [SerializeField] float alpha = 255;
    void Start()
    {

        if (enable)
        {
            EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry onPointerEnter = new EventTrigger.Entry();
            EventTrigger.Entry onPointerExit = new EventTrigger.Entry();
            //EventTrigger.Entry onPointerClick = new EventTrigger.Entry();
            onPointerEnter.eventID = EventTriggerType.PointerEnter;
            //onPointerClick.eventID = EventTriggerType.PointerClick;
            onPointerExit.eventID = EventTriggerType.PointerExit;
            onPointerEnter.callback.AddListener((pointerEventData) => { OnPointerEnter((PointerEventData)pointerEventData); });
            onPointerExit.callback.AddListener((pointerEventData) => { OnPointerExit((PointerEventData)pointerEventData); });
            //onPointerClick.callback.AddListener((pointerEventData) => { OnPointerClick((PointerEventData)pointerEventData); });
            eventTrigger.triggers.Add(onPointerEnter);
            eventTrigger.triggers.Add(onPointerExit);
        }
  
    }

    void OnPointerEnter(PointerEventData pointerEventData)
    {
        transform.localScale = scaleForHover;
        //alpha = 180;
        GetComponent<Image>().color = new Color(1,1,1,alpha);
    }

    void OnPointerExit(PointerEventData pointerEventData)
    {
        transform.localScale = scaleOriginal;
        GetComponent<Image>().color = new Color(1, 1, 1, 255);
    }

    public void OnPointerClick()
    {
        Application.OpenURL(url);
        //Debug.Log("calisti");
        transform.localScale = scaleOriginal;
        GetComponent<Image>().color = new Color(1, 1, 1, 255);
    }

}
