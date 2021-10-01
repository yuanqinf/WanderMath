using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // for debugging placement position
    public Pose placementPose;

    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private float cubeUpDistance = 0.8f;

    private ARPlacement arPlacement;
    private CharacterController characterController;
    private PlacementIndicatorController placementController;
    private BirthdayCardController birthdayCardController;
    private SoundManager soundManager;
    private UiController uiController;
    private string gamePhase = "";

    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        arPlacement = FindObjectOfType<ARPlacement>();
        characterController = FindObjectOfType<CharacterController>();
        placementController = FindObjectOfType<PlacementIndicatorController>();
        birthdayCardController = FindObjectOfType<BirthdayCardController>();
        soundManager = FindObjectOfType<SoundManager>();
        uiController = FindObjectOfType<UiController>();

        // for testing purposes
        //placementController.TurnOffPlacementAndText();
    }

    private void Update()
    {
        placementPose = placementController.GetPlacementPose();
        // handles indicator and object placement
        if (!placementController.GetIsLayoutPlaced())
        {
            placementController.UpdatePlacementAndPose(arCamera);
            PlaceObjectWhenTouched();
        }

        switch (gamePhase)
        {
            // birthday card stage
            case "phase0":
                // 1. phase0 subtitle TODO: replace with audio duration
                var duration = birthdayCardController.PlayBirthdayCardInitWithSubtitles();
                // 2. initialize birthday card falling with tutorial
                birthdayCardController.InitializeBirthdayCard(placementController.GetPlacementPose(), duration);
                Debug.Log("setting gamephase");
                gamePhase = "waiting";
                break;
            // handles first cube
            case "phase1":
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Place character object and starting audio when set
    /// </summary>
    private void PlaceObjectWhenTouched()
    {
        if (placementController.GetIsPlacementPoseValid() && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            float duration = characterController.InitCharacterFirst(placementController.GetPlacementPose(), placementController.GetPlacementIndicatorLocation());
            placementController.TurnOffPlacementAndText();
            StartCoroutine(WaitAudioFinish(duration));
        }
    }

    IEnumerator WaitAudioFinish(float duration)
    {
        yield return new WaitForSeconds(duration);
        gamePhase = "phase0";
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
