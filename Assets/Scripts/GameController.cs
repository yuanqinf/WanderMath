using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private float cubeUpDistance = 0.8f;

    private ARPlacement arPlacement;
    private CharacterController characterPlacement;
    private PlacementIndicatorController placementController;
    private SoundManager soundManager;
    private UiController uiController;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        arPlacement = FindObjectOfType<ARPlacement>();
        characterPlacement = FindObjectOfType<CharacterController>();
        placementController = FindObjectOfType<PlacementIndicatorController>();
        soundManager = FindObjectOfType<SoundManager>();
        uiController = FindObjectOfType<UiController>();
    }

    private void Update()
    {
        // handles indicator and object placement
        if (placementController.GetIsLayoutPlaced())
        {
            placementController.TurnOffPlacementAndText();

        } else if (!placementController.GetIsLayoutPlaced())
        {
            placementController.UpdatePlacementAndPose(arCamera);
            PlaceObjectWhenTouched();
        }
    }

    /// <summary>
    /// Place character object when set
    /// </summary>
    private void PlaceObjectWhenTouched()
    {
        if (placementController.GetIsPlacementPoseValid() && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            characterPlacement.InitCharacterFirst(placementController.GetPlacementPose(), placementController.GetPlacementIndicatorLocation());
            placementController.SetLayoutPlaced(true);
        }
    }

    // part 3: finish building cube
    public float StartCompleteCubeSubtitleWithAudio()
    {
        var duration = soundManager.GetCompleteCubeSubtitleAudioDuration();
        StartCoroutine(CompleteCubeSubtitleWithAudio(duration));
        return duration;
    }

    IEnumerator CompleteCubeSubtitleWithAudio(float duration)
    {
        yield return new WaitForSeconds(1);
        uiController.SetSubtitleActive(true);
        uiController.SetCompleteCubeSubtitles();
        soundManager.PlayCompleteCubeACubeAudio();
        yield return new WaitForSeconds(duration);
        yield return new WaitForSeconds(1);
        uiController.SetSubtitleActive(false);
    }

    // part 2: selecting cube
    public float StartSelectSubtitleWithAudio()
    {
        var duration = soundManager.GetSelectSubtitleAudioDuration();
        StartCoroutine(SelectSubtitleWithAudio(duration));
        return duration;
    }

    IEnumerator SelectSubtitleWithAudio(float duration)
    {
        yield return new WaitForSeconds(1);
        uiController.SetSubtitleActive(true);
        uiController.SetNextSubtitleText();
        soundManager.PlaySelectACubeAudio();
        yield return new WaitForSeconds(duration);
        yield return new WaitForSeconds(1);
        uiController.SetSubtitleActive(false);
    }

    // part 1: initialize cube (TODO: to be changed)
    public void StartSubtitlesWithAudio()
    {
        StartCoroutine(InitialSubtitleWithAudio());
    }

    IEnumerator InitialSubtitleWithAudio()
    {
        var totalLen = soundManager.GetSubtitleAudioClipsLen();
        uiController.SetSubtitleActive(true);
        for (int i = 0; i < totalLen; i++)
        {
            yield return new WaitForSeconds(1);
            uiController.SetInitialSubtitleText(i);
            soundManager.PlayStartingSubtitleAudio(i);
            var audioDuration = soundManager.GetSubtitleAudioDuration(i);

            if (i == 3)
            {
                arPlacement.PlaceCubeInSky(audioDuration, cubeUpDistance);
            }
            if (i == 4)
            {
                // TODO: highlight the cube to be clicked
            }
            yield return new WaitForSeconds(audioDuration);
        }
        yield return new WaitForSeconds(1);
        uiController.SetSubtitleActive(false);
    }
}
