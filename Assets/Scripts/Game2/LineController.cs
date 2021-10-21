using System.Collections;
using System.Collections.Generic;
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
        distanceText.text = distance.ToString("F2");
    }

    public void SetPosition(Vector3 pos)
    {
        var newPos = Camera.main.WorldToScreenPoint(pos);
        distanceText.transform.position = newPos;
    }
}
