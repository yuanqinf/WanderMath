using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UINumberControl : MonoBehaviour
{
    public TextMeshProUGUI volDisplay;
    private Camera cam;
    

    private void Start()
    {
        cam = Camera.main;
    }

    public void SetVolDisplay(double num)
    {
        string formattedDisplayStr = "Vol: " + num + " ft<sup>3</sup>";
        volDisplay.text = formattedDisplayStr;
    }

    private void LateUpdate()
    {
        if(volDisplay != null)
        {
            volDisplay.transform.LookAt(volDisplay.transform.position + cam.transform.forward);
        }
    }
}
