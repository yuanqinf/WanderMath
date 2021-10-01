using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirthdayCardController : MonoBehaviour
{
    [SerializeField]
    private GameObject birthdayCard;
    private HelperUtils utils;

    private void Start()
    {
        utils = FindObjectOfType<HelperUtils>();
    }

    public void InitializeBirthdayCard(Pose pose)
    {
        Debug.Log("placing birthday card object");
        Vector3 rot = pose.rotation.eulerAngles;
        var newRot = new Vector3(rot.x, rot.y + 90, rot.z);

        utils.PlaceObjectInSky(birthdayCard, pose.position, Quaternion.Euler(newRot), 3.0f, 0.5f);
    }
}
