using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CubeRotateControl : GenericClass
{
    public GameObject cubeEasy;

    public GameObject cubeMed;
    public GameObject cubeMed2;
    public GameObject cubeWrong;

    private string[] preCubeEasySubtitles =
    {
        "I was also going to wrap the roller skates I got for Quinn!",
        "Well, I was trying to, anyway.",
        "Can you help me make a cube, so I can finish wrapping the skates?"
    };
    private string[] initCubeEasySubtitles =
    {
        "I need a box, but I only have a flat piece.",
        "Do you think you can make a cube out of that?"
    };
    private string[] completeCubeEasySubtitles =
    {
        "No way! That's a cube. It makes sense, see, a cube has six faces.",
        "Since you're a folding master, do you want to help me wrap a few more presents?"
    };

    public int curCubeSnappedSides = 0;


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

    private float PlayCubeHardWithSubtitles()
    {
        var duration = soundManager.PlaySelectACubeAudio();
        uiController.PlaySubtitles(initCubeEasySubtitles[1], duration);
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

    #region phase1 code
    /// <summary>
    /// Play subtitles and audio for phase 1
    /// </summary>
    public void StartPhase1(Pose placementPose)
    {
        StartCoroutine(PreSetupCubeEasySubtitleWithAudio(placementPose));
    }

    IEnumerator PreSetupCubeEasySubtitleWithAudio(Pose placementPose)
    {
        var totalLen = preCubeEasySubtitles.Length;
        uiController.SetSubtitleActive(true);
        for (int i = 0; i < totalLen; i++)
        {
            //yield return new WaitForSeconds(1);
            var audioDuration = soundManager.PlaySetupCubeSubtitleAudio(i);
            uiController.PlaySubtitles(preCubeEasySubtitles[i], audioDuration);

            yield return new WaitForSeconds(audioDuration);
        }
        var duration = SetupCubeEasyWithSubtitles();
        InitializeCubeEasy(placementPose, duration);
        yield return new WaitForSeconds(duration);
    }

    private float SetupCubeEasyWithSubtitles()
    {
        StartCoroutine(SetupCubeEasySubtitleWithAudio());
        return soundManager.GetPlayCubeEasySubtitleAudio();
    }

    IEnumerator SetupCubeEasySubtitleWithAudio()
    {
        var totalLen = initCubeEasySubtitles.Length;
        for (int i = 0; i < totalLen; i++)
        {
            var audioDuration = soundManager.PlaySetupCubeSubtitleAudio(i + 3);
            uiController.PlaySubtitles(initCubeEasySubtitles[i], audioDuration);
            yield return new WaitForSeconds(audioDuration);
        }
        uiController.SetSubtitleActive(false);
    }

    /// <summary>
    /// code to end phase 1 and transit to phase 2
    /// </summary>
    /// <param name="placementPose"></param>
    public void EndPhase1()
    {
        var duration = StartCompleteEasyCubeSubtitleWithAudio();
        gameController.SetGamePhaseWithDelay("phase2", duration);
    }

    public float StartCompleteEasyCubeSubtitleWithAudio()
    {
        StartCoroutine(CompleteCubeSubtitleWithAudio());
        return soundManager.GetCompleteCubeEasySubtitleAudio();
    }

    IEnumerator CompleteCubeSubtitleWithAudio()
    {
        uiController.SetSubtitleActive(true);
        for (int i = 0; i < completeCubeEasySubtitles.Length; i++)
        {
            var duration = soundManager.PlaySetupCubeSubtitleAudio(i + 5);
            uiController.PlaySubtitles(completeCubeEasySubtitles[i], duration);
            yield return new WaitForSeconds(duration);
        }
        uiController.SetSubtitleActive(false);
    }
    #endregion

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
