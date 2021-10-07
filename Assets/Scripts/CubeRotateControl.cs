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

    //public void handleOutline(GameObject touchedObject)
    //{
    //    // handle outline here
    //    if (lastSelectedShape != null && lastSelectedShape != touchedObject.transform.root.gameObject)
    //    {
    //        Debug.Log("outing is being deactivated");
    //        lastSelectedShape.GetComponent<Outline>().enabled = false;
    //    }
    //    if (touchedObject.transform.root.GetComponent<Outline>() != null)
    //    {
    //        Debug.Log("outing is being activated");
    //        touchedObject.transform.root.GetComponent<Outline>().enabled = true;
    //    }
    //    lastSelectedShape = touchedObject.transform.root.gameObject;
    //}

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
    private void InitializeCubeEasy(Pose pose, float duration)
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

    //private void snapObject(GameObject touchedObject, float x, float y, float z)
    //{
    //    //gameController.touchEnabled = false;
    //    //Debug.Log("snapping : " + touchedObject.name);
    //    //Vector3 newAngle = new Vector3(x, y, z);
    //    //touchedObject.transform.eulerAngles = newAngle;
    //    //Debug.Log("snapped object is: " + touchedObject.name + " with local angle: " + touchedObject.transform.eulerAngles + " and parent angle: " + touchedObject.transform.parent.transform.eulerAngles);
    //    if (curCubeSnappedSides == 5)
    //    {
    //        soundManager.PlaySuccessSound();
    //        float duration = 0;
    //        //next phase
    //        if (!isPhase2)
    //        {
    //            // start of phase 2
    //            duration = gameController.StartCompleteCubeSubtitleWithAudio();
    //            gameController.SetGamePhaseWithDelay("phase2", 8f);
    //            isPhase2 = true;
    //        }
    //        else
    //        {
    //            // end of phase 2
    //            duration = soundManager.PlayPhase2EndAudio();
    //            gameController.SetGamePhaseWithDelay("phase3", duration);
    //        }
    //        uiController.SetCursorActive(false);
    //        GameObject chinchilla = GameObject.FindGameObjectWithTag("character");
    //        GameObject shapeObject = GameObject.FindGameObjectWithTag("cube_main");
    //        StartCoroutine(utils.LerpMovement(shapeObject.transform.position, chinchilla.transform.position, duration, shapeObject));
    //        curCubeSnappedSides = 0;
    //    }
    //    else
    //    {
    //        soundManager.PlaySnapSound();
    //    }
    //}

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
        Vector3 pos2 = placementPose.position + new Vector3(-0.6f, 0f, 0.6f);
        Vector3 pos3 = placementPose.position + new Vector3(-0.8f, 0f, -0.7f);


        cubeWrong = utils.PlaceObjectInSky(cubeWrong, pos1, Quaternion.Euler(rot), startAudioLen, 0.5f);
        cubeMed = utils.PlaceObjectInSky(cubeMed, pos2, Quaternion.Euler(rot), startAudioLen, 0.5f);
        cubeMed2 = utils.PlaceObjectInSky(cubeMed2, pos3, Quaternion.Euler(rot), startAudioLen, 0.5f);

        //StartCoroutine(SetupCubeSubtitleWithAudio(placementPose));
    }

}
