using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotateControl : MonoBehaviour
{
    [SerializeField]
    private GameObject cubeEasy;
    private HelperUtils utils;
    private SoundManager soundManager;
    private UiController uiController;

    private string cubeEasySubtitles = "Do you think you can make a cube out of that?";

    private void Start()
    {
        utils = FindObjectOfType<HelperUtils>();
        soundManager = FindObjectOfType<SoundManager>();
        uiController = FindObjectOfType<UiController>();
    }

    public void rotateFace(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        if (touchedObject.name == "NetFace_1")
        {
            if (newRealWorldPosition.z > initialRealWorldPosition.z)
            {
                touchedObject.transform.Rotate(new Vector3(0, 0, 5));
                if (touchedObject.transform.eulerAngles.z > 80)
                {
                    Debug.Log("SNAP!!!1");
                    snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                }
            }
        }
        if (touchedObject.name == "NetFace_2")
        {
            if (newRealWorldPosition.z < initialRealWorldPosition.z)
            {
                touchedObject.transform.Rotate(new Vector3(0, 0, 5));
                if (touchedObject.transform.eulerAngles.z > 80)
                {
                    Debug.Log("SNAP!!!2");
                    snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                }
            }
        }
        if (touchedObject.name == "NetFace_3")
        {
            if (newRealWorldPosition.x > initialRealWorldPosition.x)
            {
                touchedObject.transform.Rotate(new Vector3(0, 0, 5));
                if (touchedObject.transform.eulerAngles.z > 80)
                {
                    Debug.Log("SNAP!!!3");
                    snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                }
            }
        }
        if (touchedObject.name == "NetFace_4")
        {
            if (newRealWorldPosition.x < initialRealWorldPosition.x)
            {
                touchedObject.transform.Rotate(new Vector3(0, 0, 5));
                if (touchedObject.transform.eulerAngles.z > 80)
                {
                    Debug.Log("SNAP!!!4");
                    snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                }
            }
        }
        if (touchedObject.name == "NetFace_5")
        {
            if (newRealWorldPosition.x < initialRealWorldPosition.x)
            {
                touchedObject.transform.Rotate(new Vector3(0, 0, 5));
                if (touchedObject.transform.eulerAngles.z > 80)
                {
                    Debug.Log("SNAP!!!5");
                    snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                }
            }
        }
    }

    public float PlayCubeEasyWithSubtitles()
    {
        var duration = soundManager.PlaySelectACubeAudio();
        uiController.PlaySubtitles(cubeEasySubtitles, duration);
        return duration;
    }

    public void InitializeCube(Pose pose, float duration)
    {
        Vector3 rot = pose.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        cubeEasy = utils.PlaceObjectInSky(cubeEasy, pose.position, Quaternion.Euler(rot), duration, 0.5f);
    }

    private void snapObject(GameObject touchedObject, float x, float y, float z)
    {
        Debug.Log("snapped object is: " + touchedObject.name + " with local angle: " + touchedObject.transform.eulerAngles + " and parent angle: " + touchedObject.transform.parent.transform.eulerAngles);
        Vector3 newAngle = new Vector3(x, y, z);
        touchedObject.transform.transform.eulerAngles = newAngle;
        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
    }
}
