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
    private CubeRotateControl cubeController;
    private ShapesController shapesController;
    private SoundManager soundManager;
    private UiController uiController;
    private string gamePhase = "setup";

    public bool touchEnabled = true;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        arPlacement = FindObjectOfType<ARPlacement>();
        characterController = FindObjectOfType<CharacterController>();
        cubeController = FindObjectOfType<CubeRotateControl>();
        placementController = FindObjectOfType<PlacementIndicatorController>();
        birthdayCardController = FindObjectOfType<BirthdayCardController>();
        shapesController = FindObjectOfType<ShapesController>();
        soundManager = FindObjectOfType<SoundManager>();
        uiController = FindObjectOfType<UiController>();

        // for testing purposes
        //placementController.TurnOffPlacementAndText();
    }

    private void Update()
    {
        switch (gamePhase)
        {
            // setting up stage
            case "setup":
                if (!placementController.GetIsLayoutPlaced())
                {
                    placementPose = placementController.UpdatePlacementAndPose(arCamera, placementPose);
                    if (placementController.GetIsPlacementPoseValid() && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        var audioDuration = PlaceObjectAndAudio();
                        SetGamePhaseWithDelay("phase0", audioDuration);
                    }
                }
                break;
            // birthday card stage
            case "phase0":
                // 1. phase0 subtitle
                var duration = birthdayCardController.PlayBirthdayCardInitWithSubtitles();
                // 2. initialize birthday card falling with tutorial
                birthdayCardController.InitializeBirthdayCard(placementPose, duration);
                gamePhase = "waiting";
                break;
            // handles first cube
            case "phase1":
                // instantiate
                Destroy(birthdayCardController.BirthdayCard);
                cubeController.StartPhase1(placementPose);
                gamePhase = "waiting";
                break;
            // handles multiple cubes
            case "phase2":
                // instantiate
                // success one cube and move on
                Destroy(cubeController.cubeEasy);
                cubeController.StartPhase2(placementPose);
                gamePhase = "waiting";
                break;
            case "phase3":
                shapesController.StartPhase3(placementPose);
                gamePhase = "waiting";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Place character object and starting audio when set
    /// </summary>
    private float PlaceObjectAndAudio()
    {
        float duration = characterController.InitCharacterFirst(placementPose, placementController.GetPlacementIndicatorLocation());
        placementController.TurnOffPlacementAndText();
        return duration;
    }

    /// <summary>
    /// Sets the gamephase with a delay duration.
    /// </summary>
    /// <param name="phaseName"></param>
    /// <param name="duration"></param>
    public void SetGamePhaseWithDelay(string phaseName, float duration)
    {
        StartCoroutine(SetGamePhase(phaseName, duration));
    }

    IEnumerator SetGamePhase(string phaseName, float duration)
    {
        yield return new WaitForSeconds(duration);
        gamePhase = phaseName;
    }


    public void SetGamePhase(string phaseName)
    {
        gamePhase = phaseName;
    }

    public Vector3 GetArCharacterPosition()
    {
        return characterController.GetArCharacterPosition();
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
