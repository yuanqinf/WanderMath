using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class HandleSnapControl : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

    public bool canSnap;

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }


    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("should snap now");
        canSnap = true;
        Debug.Log("changed canSnap: " + canSnap);
    }

}