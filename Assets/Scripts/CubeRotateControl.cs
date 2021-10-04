using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotateControl : MonoBehaviour
{
    public GameObject cubeEasy;
    public GameObject[] cubes;
    private HelperUtils utils;
    private SoundManager soundManager;
    private UiController uiController;
    private GameController gameController;

    private bool selectedRightCube = false;
    private bool isPhase2 = false;

    private string[] initCubeEasySubtitles =
    {
        "I was also going to wrap a present for Quinn!",
        "Well, I was trying to, anyway."
    };
    private string cubeEasySubtitles = "Do you think you can make a cube out of that?";

    public int curCubeSnappedSides = 0;

    private void Start()
    {
        utils = FindObjectOfType<HelperUtils>();
        soundManager = FindObjectOfType<SoundManager>();
        uiController = FindObjectOfType<UiController>();
        gameController = FindObjectOfType<GameController>();
    }

    public void selectWrongCube()
    {
        // select wrong one
        soundManager.PlayPhase2WrongCube();
        return;
    }

    public void selectCorrectCube(GameObject touchedObject)
    {
        Vector3 newPos = touchedObject.transform.position + new Vector3(0, 0, 1f);
        utils.LerpMovement(touchedObject.transform.position, newPos, 2, touchedObject);
        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
        soundManager.PlaySelectACubeAudio();
        selectedRightCube = true;
        return;
    }

    public void rotateFace(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {

        switch (touchedObject.name)
        {
            // something wrong with the snapping of face 1
            case "NetFace_1":
                Debug.Log("touching netface 1!!!!!!!");
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
                Debug.Log("touching netface 2!!!!!!!");
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
                Debug.Log("touching netface 3!!!!!!!");
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
                Debug.Log("touching netface 4!!!!!!!");
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
                Debug.Log("touching netface 5!!!!!!!");
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

    private float PlayCubeEasyWithSubtitles()
    {
        var duration = soundManager.PlaySelectACubeAudio();
        uiController.PlaySubtitles(cubeEasySubtitles, duration);
        return duration;
    }

    private float PlayCubeHardWithSubtitles()
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

    /// <summary>
    /// Play subtitles and audio for phase 1
    /// </summary>
    public void StartPhase1(Pose placementPose)
    {
        StartCoroutine(SetupCubeSubtitleWithAudio(placementPose));
    }

    IEnumerator SetupCubeSubtitleWithAudio(Pose placementPose)
    {
        var totalLen = initCubeEasySubtitles.Length;
        uiController.SetSubtitleActive(true);
        for (int i = 0; i < totalLen; i++)
        {
            yield return new WaitForSeconds(1);
            uiController.SetInitialSubtitleText(i);
            var audioDuration = soundManager.PlaySetubCubeSubtitleAudio(i);

            yield return new WaitForSeconds(audioDuration);
        }
        uiController.SetSubtitleActive(false);
        var duration = PlayCubeEasyWithSubtitles();
        InitializeCube(placementPose, duration);
        yield return new WaitForSeconds(duration);
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
            float duration = 0;
            //next phase
            if (!isPhase2)
            {
                // start of phase 2
                duration = gameController.StartCompleteCubeSubtitleWithAudio();
                gameController.SetGamePhaseWithDelay("phase2", 10f);
                isPhase2 = true;
            }
            else
            {
                // end of phase 2
                soundManager.PlayPhase2EndAudio();
            }
            uiController.SetCursorActive(false);
            GameObject chinchilla = GameObject.FindGameObjectWithTag("character");
            GameObject shapeObject = GameObject.FindGameObjectWithTag("cube_main");
            StartCoroutine(utils.LerpMovement(shapeObject.transform.position, chinchilla.transform.position, duration, shapeObject));
            curCubeSnappedSides = 0;
        }
    }

    /// <summary>
    /// Play subtitles and audio for phase 2
    /// </summary>
    public void StartPhase2(Pose placementPose)
    {
        //start phase 2 here
        Debug.Log("phase 2 started!!!!!!!!!!!");
        var startAudioLen = soundManager.PlayPhase2StartAudio();

        Vector3 rot = placementPose.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        Vector3 pos1 = placementPose.position;
        Vector3 pos2 = placementPose.position + new Vector3(1f, 0, 0);
        Vector3 pos3 = placementPose.position + new Vector3(-1f, 0, 0);
        Vector3 pos4 = placementPose.position + new Vector3(0, 0, 1f);
        Vector3 pos5 = placementPose.position + new Vector3(0, 0, -0.6f);


        var cubeHard = utils.PlaceObjectInSky(cubes[0], pos1, Quaternion.Euler(rot), startAudioLen, 0.5f);
        var cubeMed = utils.PlaceObjectInSky(cubes[1], pos2, Quaternion.Euler(rot), startAudioLen, 0.5f);
        var cubeMed2 = utils.PlaceObjectInSky(cubes[2], pos3, Quaternion.Euler(rot), startAudioLen, 0.5f);
        var cubeWrong1 = utils.PlaceObjectInSky(cubes[3], pos4, Quaternion.Euler(rot), startAudioLen, 0.5f);
        var cubeWrong2 = utils.PlaceObjectInSky(cubes[4], pos5, Quaternion.Euler(rot), startAudioLen, 0.5f);


        //StartCoroutine(SetupCubeSubtitleWithAudio(placementPose));
    }

}
