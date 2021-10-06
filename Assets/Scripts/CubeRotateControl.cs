using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CubeRotateControl : MonoBehaviour
{
    public GameObject cubeEasy;

    public GameObject cubeMed;
    public GameObject cubeMed2;
    public GameObject cubeWrong;

    private List<GameObject> cubes;

    private HelperUtils utils;
    private SoundManager soundManager;
    private UiController uiController;
    private GameController gameController;

    private bool selectedRightCube = false;
    private GameObject lastSelectedShape = null;
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


    public void selectCorrectCube(GameObject touchedObject)
    {
        var duration = soundManager.PlaySelectACubeAudio();
        Vector3 newPos = touchedObject.transform.position + new Vector3(0, 1f, 0f);
        utils.LerpMovement(touchedObject.transform.position, newPos, duration, touchedObject);
        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
        selectedRightCube = true;
        return;
    }

    public void handleOutline(GameObject touchedObject)
    {
        // handle outline here
        if (lastSelectedShape != null && lastSelectedShape != touchedObject.transform.root.gameObject)
        {
            lastSelectedShape.GetComponent<Outline>().enabled = false;
        }
        if (touchedObject.transform.root.GetComponent<Outline>() != null)
        {
            touchedObject.transform.root.GetComponent<Outline>().enabled = true;
        }
        lastSelectedShape = touchedObject.transform.root.gameObject;
    }

    public void rotateFace(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        handleOutline(touchedObject);
        switch (touchedObject.name)
        {
            // something wrong with the snapping of face 1
            case "NetFace_1":
                Debug.Log("touching netface 1!!!!!!!");
                if (newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    if (touchedObject.transform.localEulerAngles.z > 50)
                    {
                        if (touchedObject.transform.root.tag == "cube_wrong") touchedObject.transform.root.GetComponent<WrongCubeSnap>().increaseSnapNum();
                        else if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
                        snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                break;
            case "NetFace_2":
                Debug.Log("touching netface 2!!!!!!!");
                if(touchedObject.tag == "cube_wrong")
                {
                    if (newRealWorldPosition.x > initialRealWorldPosition.x)
                    {
                        touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                        if (touchedObject.transform.localEulerAngles.z > 50)
                        {
                            if (touchedObject.transform.root.tag == "cube_wrong") touchedObject.transform.root.GetComponent<WrongCubeSnap>().increaseSnapNum();
                            else if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
                            snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                            touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
                        }
                    }
                }
                else
                {
                    if (newRealWorldPosition.z < initialRealWorldPosition.z)
                    {
                        touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                        if (touchedObject.transform.localEulerAngles.z > 50)
                        {
                            if (touchedObject.transform.root.tag == "cube_wrong") touchedObject.transform.root.GetComponent<WrongCubeSnap>().increaseSnapNum();
                            else if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
                            snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                            touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
                        }
                    }
                }
                break;
            case "NetFace_3":
                Debug.Log("touching netface 3!!!!!!!");
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    if (touchedObject.transform.localEulerAngles.z > 50)
                    {
                        if (touchedObject.transform.root.tag == "cube_wrong") touchedObject.transform.root.GetComponent<WrongCubeSnap>().increaseSnapNum();
                        else if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
                        snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                break;
            case "NetFace_4":
                Debug.Log("touching netface 4!!!!!!!");
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    if (touchedObject.transform.localEulerAngles.z > 50)
                    {
                        if (touchedObject.transform.root.tag == "cube_wrong") touchedObject.transform.root.GetComponent<WrongCubeSnap>().increaseSnapNum();
                        else if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
                        snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                break;
            case "NetFace_5":
                Debug.Log("touching netface 5!!!!!!!");
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    if (touchedObject.transform.localEulerAngles.z > 50)
                    {
                        if (touchedObject.transform.root.tag == "cube_wrong") touchedObject.transform.root.GetComponent<WrongCubeSnap>().increaseSnapNum();
                        else if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
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

    /// <summary>
    /// Instantiate cube easy for phase 1
    /// </summary>
    /// <param name="pose"></param>
    /// <param name="duration"></param>
    public void InitializeCubeEasy(Pose pose, float duration)
    {
        Vector3 cubeEasyPos = pose.position + new Vector3(0.25f, 0f, 0.25f);

        Vector3 rot = pose.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);

        cubeEasy = utils.PlaceObjectInSky(cubeEasy, cubeEasyPos, Quaternion.Euler(rot), duration, 0.5f);
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
        InitializeCubeEasy(placementPose, duration);
        yield return new WaitForSeconds(duration);
    }

    private void snapObject(GameObject touchedObject, float x, float y, float z)
    {
        gameController.touchEnabled = false;
        Debug.Log("snapping : " + touchedObject.name);
        if (!touchedObject.transform.GetComponent<BoxCollider>().enabled) return;
        Vector3 newAngle = new Vector3(x, y, z);
        Debug.Log("newAngle: " + newAngle);
        touchedObject.transform.eulerAngles = newAngle;
        //Debug.Log("snapped object is: " + touchedObject.name + " with local angle: " + touchedObject.transform.eulerAngles + " and parent angle: " + touchedObject.transform.parent.transform.eulerAngles);
        if (curCubeSnappedSides == 5)
        {
            soundManager.PlaySuccessSound();
            float duration = 0;
            //next phase
            if (!isPhase2)
            {
                // start of phase 2
                duration = gameController.StartCompleteCubeSubtitleWithAudio();
                gameController.SetGamePhaseWithDelay("phase2", 8f);
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
        else
        {
            soundManager.PlaySnapSound();
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
        Vector3 pos2 = placementPose.position + new Vector3(0f, 0f, 0.8f);
        Vector3 pos3 = placementPose.position + new Vector3(0f, 0f, -0.8f);


        cubeMed = utils.PlaceObjectInSky(cubeMed, pos1, Quaternion.Euler(rot), startAudioLen, 0.5f);
        cubeWrong = utils.PlaceObjectInSky(cubeWrong, pos2, Quaternion.Euler(rot), startAudioLen, 0.5f);
        cubeMed2 = utils.PlaceObjectInSky(cubeMed2, pos3, Quaternion.Euler(rot), startAudioLen, 0.5f);

        //StartCoroutine(SetupCubeSubtitleWithAudio(placementPose));
    }

}
