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

    public GameObject lastSelectedShape;

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

    private string[] initPhase2Subtitles =
    {
        "I just need one more cube to finish wrapping the presents.",
        "Do you think that one makes a cube too?"
    };
    private string[] wrongPhase2Subtitles =
    {
        "I totally thought that would make a cube! It has six faces, but when you fold it, they overlap.",
        "No worries, I bet I'll find something cool to make out of it. Wanna see if we can find one that makes a cube?"
    };
    private string[] completePhase2Subtitles =
    {
        "Wow, you're really good at this! I couldn't have done it without you.",
        "If you want, we can wrap some more presents? I think we might have some funky shaped boxes..."
    };

    public int curCubeSnappedSides = 0;


    public void handleSelected(GameObject touchedObject)
    {
        lastSelectedShape = touchedObject.transform.root.gameObject;
    }

    #region phase1 code
    /// <summary>
    /// Play subtitles and audio for phase 1
    /// </summary>
    public void StartPhase1(Pose placementPose)
    {
        StartCoroutine(PreSetupCubeEasySubtitleWithAudio(placementPose));
        var duration = soundManager.GetStartPhase1AudioDuration();
        characterController.PlayTalkingAnimationWithDuration(duration);
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
        yield return new WaitForSeconds(duration);

        Vector3 cubeEasyPos = placementPose.position + new Vector3(0.5f, 0f, 0.5f);

        Vector3 rot = placementPose.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);

        Instantiate(cubeEasy, cubeEasyPos, Quaternion.Euler(rot));
        FindObjectOfType<CubeEasy>().numSnapped = 0;
        ColliderUtils.SwitchCubeEasyCollider(true);
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
            characterController.PlayTalkingAnimationWithDuration(audioDuration);
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
        StartCoroutine(CompleteCubeSubtitleWithAudio());
        //TODO: add lerpmovements
        //utils.LerpMovement(cubeEasy)
        var duration = soundManager.GetCompleteCubeEasySubtitleAudio();
        characterController.PlayTalkingAnimationWithDuration(1.8f + duration);
    }

    IEnumerator CompleteCubeSubtitleWithAudio()
    {
        yield return new WaitForSeconds(1.8f); // wait for success sound to play finish
        uiController.SetSubtitleActive(true);
        for (int i = 0; i < completeCubeEasySubtitles.Length; i++)
        {
            var duration = soundManager.PlaySetupCubeSubtitleAudio(i + 5);
            uiController.PlaySubtitles(completeCubeEasySubtitles[i], duration);
            yield return new WaitForSeconds(duration);
        }
        uiController.SetSubtitleActive(false);
        gameController.SetGamePhase(Constants.GamePhase.PHASE2);
    }
    #endregion

    #region phase 2 code
    /// <summary>
    /// Play subtitles and audio for phase 2
    /// </summary>
    public void StartPhase2(Pose placementPose)
    {
        StartCoroutine(SetupCubesSubtitleWithAudio(placementPose));
        var audioLen = soundManager.GetInitPlayCubesSubtitleAudio();
        characterController.PlayTalkingAnimationWithDuration(audioLen);
    }

    IEnumerator SetupCubesSubtitleWithAudio(Pose placementPose)
    {
        var totalLen = initPhase2Subtitles.Length;
        for (int i = 0; i < totalLen; i++)
        {
            var audioDuration = soundManager.PlayInitCubesSubtitleAudio(i);
            uiController.PlaySubtitles(initPhase2Subtitles[i], audioDuration);
            yield return new WaitForSeconds(audioDuration);
        }
        uiController.SetSubtitleActive(false);

        Vector3 rot = placementPose.rotation.eulerAngles;
        var wrongRot = new Vector3(rot.x, rot.y, rot.z);
        rot = new Vector3(rot.x, rot.y + 165, rot.z);
        Vector3 pos1 = placementPose.position;
        Vector3 pos2 = placementPose.position + new Vector3(0.6f, 0f, 0.9f);
        Vector3 pos3 = placementPose.position + new Vector3(0.95f, 0f, 0.0f);

        Instantiate(cubeMed, pos1, Quaternion.Euler(rot));
        Instantiate(cubeWrong, pos2, Quaternion.Euler(wrongRot));
        Instantiate(cubeMed2, pos3, Quaternion.Euler(rot));

        FindObjectOfType<CubeMed>().numSnapped = 0;
        FindObjectOfType<CubeWrong>().numSnapped = 0;
        FindObjectOfType<CubeMedTwo>().numSnapped = 0;
        ColliderUtils.SwitchCubesCollider(true); // enable colliders
    }

    public IEnumerator CompletePhase2WrongCubeSubtitleWithAudio()
    {
        uiController.SetSubtitleActive(true);
        for (int i = 0; i < wrongPhase2Subtitles.Length; i++)
        {
            var duration = soundManager.PlayWrongCubesSubtitleAudio(i);
            characterController.PlayTalkingAnimationWithDuration(duration);
            uiController.PlaySubtitles(wrongPhase2Subtitles[i], duration);
            yield return new WaitForSeconds(duration);
        }
        uiController.SetSubtitleActive(false);
    }

    public void EndPhase2(string cubeName)
    {
        StartCoroutine(CompletePhase2CubeSubtitleWithAudio());
        //TODO: add lerpmovements
        //utils.LerpMovement(cube)
        var duration = soundManager.GetCompletePlayCubesSubtitleAudio();
        characterController.PlayTalkingAnimationWithDuration(1.8f + duration);
    }

    IEnumerator CompletePhase2CubeSubtitleWithAudio()
    {
        yield return new WaitForSeconds(1.8f); // wait for success sound to play finish
        ColliderUtils.SwitchCubesCollider(false);
        uiController.SetSubtitleActive(true);
        for (int i = 0; i < completePhase2Subtitles.Length; i++)
        {
            var duration = soundManager.PlayCompleteCubesSubtitleAudio(i);
            uiController.PlaySubtitles(completePhase2Subtitles[i], duration);
            yield return new WaitForSeconds(duration);
        }
        uiController.SetSubtitleActive(false);
        gameController.SetGamePhase(Constants.GamePhase.PHASE3);
    }
    #endregion
}
