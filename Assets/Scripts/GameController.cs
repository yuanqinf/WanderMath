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
                        // TODO: change this back to phase0
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
                DestroyImmediate(birthdayCardController.BirthdayCard);
                cubeController.StartPhase1(placementPose);
                gamePhase = "waiting";
                break;
            // handles multiple cubes
            case "phase2":
                // instantiate one cube and move on
                DestroyImmediate(cubeController.cubeEasy);
                cubeController.StartPhase2(placementPose);
                gamePhase = "waiting";
                break;
            // handles multiple other shapes
            case "phase3":
                DestroyImmediate(cubeController.cubeMed);
                DestroyImmediate(cubeController.cubeMed2);
                DestroyImmediate(cubeController.cubeWrong);
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
        float duration = characterController.InitCharacterAndAudio(placementPose, placementController.GetPlacementIndicatorLocation());
        placementController.TurnOffPlacementAndText();
        return duration;
    }

    #region setting game phases
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
    #endregion

    public Vector3 GetArCharacterPosition()
    {
        return characterController.GetArCharacterPosition();
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
}
