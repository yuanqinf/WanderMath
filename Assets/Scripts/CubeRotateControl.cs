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
    private GameController gameController;

    private string cubeEasySubtitles = "Do you think you can make a cube out of that?";

    public int curCubeSnappedSides = 0;

    private void Start()
    {
        utils = FindObjectOfType<HelperUtils>();
        soundManager = FindObjectOfType<SoundManager>();
        uiController = FindObjectOfType<UiController>();
        gameController = FindObjectOfType<GameController>();
    }

    public void rotateFace(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        switch (touchedObject.name)
        {
            // something wrong with the snapping of face 1
            case "NetFace_1":
                if (newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, 1));
                    if (touchedObject.transform.eulerAngles.z > 70)
                    {
                        touchedObject.transform.transform.eulerAngles = new Vector3(touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                        if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
                        soundManager.PlaySnapSound();
                        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
                        //snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                    }
                }
                break;
            case "NetFace_2":
                if (newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, 1));
                    if (touchedObject.transform.eulerAngles.z > 70)
                    {
                        if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
                        snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                break;
            case "NetFace_3":
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, 1));
                    if (touchedObject.transform.eulerAngles.z > 70)
                    {
                        if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
                        snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                break;
            case "NetFace_4":
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, 1));
                    if (touchedObject.transform.eulerAngles.z > 70)
                    {
                        if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
                        snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                break;
            case "NetFace_5":
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, 1));
                    if (touchedObject.transform.eulerAngles.z > 70)
                    {
                        if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
                        snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                break;
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
        if (!touchedObject.transform.GetComponent<BoxCollider>().enabled) return;
        Vector3 newAngle = new Vector3(x, y, z);
        Debug.Log("newAngle: " + newAngle);
        touchedObject.transform.transform.eulerAngles = newAngle;
        soundManager.PlaySnapSound();
        Debug.Log("snapped object is: " + touchedObject.name + " with local angle: " + touchedObject.transform.eulerAngles + " and parent angle: " + touchedObject.transform.parent.transform.eulerAngles);
        if (curCubeSnappedSides == 5)
        {
            var duration = gameController.StartCompleteCubeSubtitleWithAudio();
            uiController.SetCursorActive(false);
            GameObject chinchilla = GameObject.FindGameObjectWithTag("character");
            GameObject shapeObject = GameObject.FindGameObjectWithTag("cube_easy");
            StartCoroutine(utils.LerpMovement(shapeObject.transform.position, chinchilla.transform.position, duration, shapeObject));
            curCubeSnappedSides = 0;
        }
    }

}
