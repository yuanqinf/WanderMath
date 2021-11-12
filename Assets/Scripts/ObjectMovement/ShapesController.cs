using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesController : GenericClass
{
    public GameObject cuboid;
    public GameObject pyramid;
    public GameObject hexagon;

    private string phase3Start = "Ok, these ones definitely won't make a cube. But let's see what they will make!";
    private string phase3Pyramid = "Whoa, this one's a pyramid! The bottom is a square, so it's called a square pyramid. That's such a cool gift box.";
    private string phase3Hexagon = "How cool is that! It's closer to round than the cube box, the base is a hexagon.";
    private string phase3Cuboid = "Oh, rad, a rectangular prism box! This would work for presents that are extra long.";
    private string phase3Repeat = "You can keep folding more boxes, or tap on me to go on to the birthday party!";

    internal void StartPhase3(Pose placementPose)
    {
        StartCoroutine(SetupShapesSubtitleWithAudio());
        var startAudioLen = soundManager.GetPhase3InitShapes(0);
        characterController.PlayTalkingAnimationWithDuration(startAudioLen);

        Vector3 cuboidPos = placementPose.position + new Vector3(0.3f, 0f, 0.0f); ;
        Vector3 pyramidPos = placementPose.position + new Vector3(0.6f, 0f, 0.8f);
        Vector3 hexPos = placementPose.position + new Vector3(1.1f, 0f, 0.0f);

        utils.PlaceObjectInSky(cuboid, cuboidPos, placementPose.rotation, startAudioLen, 0.5f);
        utils.PlaceObjectInSky(pyramid, pyramidPos, placementPose.rotation, startAudioLen, 0.5f);
        utils.PlaceObjectInSky(hexagon, hexPos, placementPose.rotation, startAudioLen, 0.5f);

        ColliderUtils.SwitchCubesCollider(false);

        FindObjectOfType<CuboidController>().numSnapped = 0;
        FindObjectOfType<PyramidController>().numSnapped = 0;
        FindObjectOfType<HexagonController>().numSnapped = 0;
    }

    IEnumerator SetupShapesSubtitleWithAudio()
    {
        uiController.SetSubtitleActive(true);

        var audioDuration = soundManager.PlayPhase3InitShapesSubtitleAudio(0);
        uiController.PlaySubtitles(phase3Start, audioDuration);

        yield return new WaitForSeconds(audioDuration);
        ColliderUtils.SwitchCubesCollider(true);
        uiController.SetSubtitleActive(false);
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
        StartCoroutine(PlayPhase3RepeatSubtitleWithAudio());
    }

    public IEnumerator PlayHexagonSubtitleWithAudio()
    {
        yield return new WaitForSeconds(Constants.BIGWIN_ANIMATION_DELAY);
        uiController.SetSubtitleActive(true);
        var audioDuration = soundManager.PlayPhase3InitShapesSubtitleAudio(2);
        characterController.PlayTalkingAnimationWithDuration(audioDuration);
        uiController.PlaySubtitles(phase3Hexagon, audioDuration);
        yield return new WaitForSeconds(audioDuration);
        StartCoroutine(PlayPhase3RepeatSubtitleWithAudio());
    }

    public IEnumerator PlayCuboidSubtitleWithAudio()
    {
        yield return new WaitForSeconds(Constants.BIGWIN_ANIMATION_DELAY);
        uiController.SetSubtitleActive(true);
        var audioDuration = soundManager.PlayPhase3InitShapesSubtitleAudio(3);
        characterController.PlayTalkingAnimationWithDuration(audioDuration);
        uiController.PlaySubtitles(phase3Cuboid, audioDuration);
        yield return new WaitForSeconds(audioDuration);
        StartCoroutine(PlayPhase3RepeatSubtitleWithAudio());
    }

    public IEnumerator PlayPhase3RepeatSubtitleWithAudio()
    {
        var audioDuration = soundManager.PlayPhase3InitShapesSubtitleAudio(4);
        characterController.PlayTalkingAnimationWithDuration(audioDuration);
        uiController.PlaySubtitles(phase3Repeat, audioDuration);
        yield return new WaitForSeconds(audioDuration);
        uiController.SetSubtitleActive(false);
    }
}
