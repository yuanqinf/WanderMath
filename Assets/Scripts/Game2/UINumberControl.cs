using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UINumberControl : MonoBehaviour
{
    public TextMeshProUGUI volDisplay;
    public TextMeshProUGUI heightDisplay;
    public TextMeshProUGUI lengthDisplay;
    public TextMeshProUGUI widthDisplay;
    public Canvas uiDisplay;
    private Camera cam;
    private double _height;
    public double Height { get { return _height; } set { _height = value; heightDisplay.text = "H: " + value.ToString("F1") + " ft"; } }
    public double area;

    private void Start()
    {
        cam = Camera.main;
    }

    public void SetVolDisplay(float num)
    {
        string formattedDisplayStr = "Vol: " + num + " ft<sup>3</sup>";
        volDisplay.text = formattedDisplayStr;
    }

    public void SetAreaDisplay(double num)
    {
        string formattedDisplayStr = "Area: " + num + " ft<sup>2</sup>";
        this.area = num;
        volDisplay.text = formattedDisplayStr;
    }

    public void SetWidthDisplay(string text)
    {
        widthDisplay.text = text;
    }

    public void SetLengthDisplay(string text)
    {
        lengthDisplay.text = text;
    }

    public void IncreaseCanvasY(float num)
    {
        uiDisplay.transform.position += new Vector3(0, num, 0);
    }

    private void LateUpdate()
    {
        var tempTrans = cam.transform;

        if (volDisplay != null)
        {
            //volDisplay.transform.LookAt(new Vector3(tempTrans.position.x, volDisplay.transform.position.y, tempTrans.position.z));

            volDisplay.transform.parent.transform.LookAt(volDisplay.transform.position + cam.transform.forward);
        }
        if (heightDisplay != null)
        {
            heightDisplay.transform.LookAt(heightDisplay.transform.position + cam.transform.forward);
        }
    }
}
