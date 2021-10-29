using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LineController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI distanceText;
    [SerializeField]
    private GameObject measuringTape;

    public void SetDistance(float distance)
    {
        var newRatio = distance / Constants.ONE_FEET;
        distanceText.text = newRatio.ToString("F1") + " ft";
    }

    public void SetPosition(Vector3 pos)
    {
        var newPos = Camera.main.WorldToScreenPoint(pos);
        distanceText.transform.position = pos;
        measuringTape.transform.position = pos; // + new Vector3(0.1f, 0.0f, 0.0f);
    }
}
