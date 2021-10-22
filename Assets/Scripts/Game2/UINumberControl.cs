using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UINumberControl : MonoBehaviour
{
    public GameObject volDisplay;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    public void SetVolDisplay(double num)
    {
        string formattedDisplayStr = "Vol: " + num + " Cu.Ft";

        volDisplay.GetComponentInChildren<Text>().text = formattedDisplayStr;
    }

    private void LateUpdate()
    {
        if(volDisplay != null)
        {
            volDisplay.transform.LookAt(volDisplay.transform.position + cam.transform.forward);
        }
    }
}
