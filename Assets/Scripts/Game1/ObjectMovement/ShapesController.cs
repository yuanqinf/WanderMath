using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesController : GenericClass
{
    public GameObject cuboid;
    public GameObject pyramid;
    public GameObject hexagon;

    public int numShapesCompleted = 0;
    public GameObject lastSelectedShape;

    private string phase3Start = "Ok, these ones definitely won't make a cube. But let's see what they will make!";
    private string phase3Pyramid = "Whoa, this one's a pyramid! The bottom is a square, so it's called a square pyramid. That's such a cool gift box.";
    private string phase3Hexagon = "How cool is that! It's closer to round than the cube box, the base is a hexagon.";
    private string phase3Cuboid = "Oh, rad, a rectangular prism box! This would work for presents that are extra long.";
    private string phase3Repeat = "You can keep folding more boxes!";

    internal void StartPhase3(Pose placementPose)
    {
        StartCoroutine(SetupShapesSubtitleWithAudio(placementPose));
        var startAudioLen = soundManager.GetPhase3InitShapes(0);
        characterController.PlayTalkingAnimationWithDuration(startAudioLen);
    }

    IEnumerator SetupShapesSubtitleWithAudio(Pose placementPose)
    {
        uiController.SetSubtitleActive(true);

        var audioDuration = soundManager.PlayPhase3InitShapesSubtitleAudio(0);
        uiController.PlaySubtitles(phase3Start, audioDuration);

        yield return new WaitForSeconds(audioDuration);

        uiController.SetSubtitleActive(false);
        InstantiateObjects(placementPose);
    }

    private void InstantiateObjects(Pose placementPose)
    {
        Vector3 cuboidPos = placementPose.position + new Vector3(0.3f, 0f, 0.0f); ;
        Vector3 pyramidPos = placementPose.position + new Vector3(0.6f, 0f, 0.8f);
        Vector3 hexPos = placementPose.position + new Vector3(1.1f, 0f, 0.0f);

        Instantiate(cuboid, cuboidPos, cuboid.transform.rotation);
        Instantiate(pyramid, pyramidPos, pyramid.transform.rotation);
        Instantiate(hexagon, hexPos, hexagon.transform.rotation);

        FindObjectOfType<CuboidController>().numSnapped = 0;
        FindObjectOfType<PyramidController>().numSnapped = 0;
        FindObjectOfType<HexagonController>().numSnapped = 0;
    }

    public IEnumerator PlayPyramidSubtitleWithAudio()
    {
        yield return new WaitForSeconds(Constants.BIGWIN_ANIMATION_DELAY);
        uiController.SetSubtitleActive(true);
        var audioDuration = soundManager.PlayPhase3InitShapesSubtitleAudio(1);
        characterController.PlayTalkingAnimationWithDuration(audioDuration);
        uiController.PlaySubtitles(phase3Pyramid, audioDuration);
        yield return new WaitForSeconds(audioDuration);
        uiController.SetSubtitleActive(false);
        CheckEndingTransition();
    }

    public IEnumerator PlayHexagonSubtitleWithAudio()
    {
        yield return new WaitForSeconds(Constants.BIGWIN_ANIMATION_DELAY);
        uiController.SetSubtitleActive(true);
        var audioDuration = soundManager.PlayPhase3InitShapesSubtitleAudio(2);
        characterController.PlayTalkingAnimationWithDuration(audioDuration);
        uiController.PlaySubtitles(phase3Hexagon, audioDuration);
        yield return new WaitForSeconds(audioDuration);
        CheckEndingTransition();
    }

    public IEnumerator PlayCuboidSubtitleWithAudio()
    {
        yield return new WaitForSeconds(Constants.BIGWIN_ANIMATION_DELAY);
        uiController.SetSubtitleActive(true);
        var audioDuration = soundManager.PlayPhase3InitShapesSubtitleAudio(3);
        characterController.PlayTalkingAnimationWithDuration(audioDuration);
        uiController.PlaySubtitles(phase3Cuboid, audioDuration);
        yield return new WaitForSeconds(audioDuration);
        CheckEndingTransition();
    }

    public IEnumerator PlayPhase3RepeatSubtitleWithAudio()
    {
        var audioDuration = soundManager.PlayPhase3InitShapesSubtitleAudio(4);
        characterController.PlayTalkingAnimationWithDuration(audioDuration);
        uiController.PlaySubtitles(phase3Repeat, audioDuration);
        yield return new WaitForSeconds(audioDuration);
        uiController.SetSubtitleActive(false);
    }

    private void CheckEndingTransition()
    {
        if (numShapesCompleted < 2)
        {
            StartCoroutine(PlayPhase3RepeatSubtitleWithAudio());
            numShapesCompleted++;
        }
        else if (numShapesCompleted == 2)
        {
            FindObjectOfType<GameController>().PlayEndingAnimation();
        }
    }

    public void handleSelected(GameObject touchedObject)
    {
        lastSelectedShape = touchedObject.transform.root.gameObject;
    }
}
