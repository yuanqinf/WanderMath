using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;// Required when using Event data.


public class TestUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public void OnPointerDown(PointerEventData eventData) { Debug.Log(this.gameObject + " Down"); }
    public void OnPointerEnter(PointerEventData eventData) { eventData.pointerPress = gameObject; }
    public void OnPointerUp(PointerEventData eventData) { Debug.Log(this.gameObject + " Up"); }
}