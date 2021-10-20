using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINumberControl : MonoBehaviour
{
    public Text volDisplay;

    public void SetVolDisplay(int num)
    {
        string formattedDisplayStr = "Vol: " + num + " Cu.Ft";

        volDisplay.text = formattedDisplayStr;
    }
}
