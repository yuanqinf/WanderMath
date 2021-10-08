using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // for debugging placement position
    public Pose placementPose;
    public string gamePhase = "setup";

    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private GameObject successEffect;

    private CharacterController characterController;
    private PlacementIndicatorController placementController;
    private BirthdayCardController birthdayCardController;
    private CubeRotateControl cubeController;
    private ShapesController shapesController;
    private UiController uiController;
    private GameObject lastSelectedShape = null;

    public bool touchEnabled = true;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        characterController = FindObjectOfType<CharacterController>();
        cubeController = FindObjectOfType<CubeRotateControl>();
        placementController = FindObjectOfType<PlacementIndicatorController>();
        birthdayCardController = FindObjectOfType<BirthdayCardController>();
        shapesController = FindObjectOfType<ShapesController>();
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
                // 1. phase0 subtitle, audio, animation
                var duration = birthdayCardController.PlayBirthdayCardInitWithSubtitles();
                characterController.PlayTalkingAnimationWithDuration(duration);
                // 2. initialize birthday card falling with tutorial
                birthdayCardController.InitializeBirthdayCard(placementPose, duration);
                gamePhase = "waiting";
                break;
            // handles first cube
            case "phase1":
                // instantiate
                DestroyImmediate(birthdayCardController.BirthdayCard, true);
                cubeController.StartPhase1(placementPose);
                Debug.Log("start phase 1 now!!!!!!!!!!!!");
                gamePhase = "waiting";
                break;
            // handles multiple cubes
            case "phase2":
                // instantiate one cube and move on
                DestroyImmediate(cubeController.cubeEasy, true);
                cubeController.StartPhase2(placementPose);
                gamePhase = "waiting";
                break;
            // handles multiple other shapes
            case "phase3":
                DestroyImmediate(cubeController.cubeMed, true);
                DestroyImmediate(cubeController.cubeMed2, true);
                DestroyImmediate(cubeController.cubeWrong, true);
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
        var duration = 10f;
        StartCoroutine(SelectSubtitleWithAudio(duration));
        return duration;
    }

    IEnumerator SelectSubtitleWithAudio(float duration)
    {
        yield return new WaitForSeconds(1);
        uiController.SetSubtitleActive(true);
        uiController.SetNextSubtitleText();
        yield return new WaitForSeconds(duration);
        yield return new WaitForSeconds(1);
        uiController.SetSubtitleActive(false);
    }

    // handle cube selection outline
    public void handleOutline(GameObject touchedObject)
    {
        // handle outline here
        if (this.lastSelectedShape != null && lastSelectedShape != touchedObject.transform.root.gameObject)
        {
            Debug.Log("outing is being deactivated");
            this.lastSelectedShape.GetComponent<Outline>().enabled = false;
        }
        if (touchedObject.transform.root.GetComponent<Outline>() != null)
        {
            Debug.Log("outing is being activated");
            touchedObject.transform.root.GetComponent<Outline>().enabled = true;
        }
        this.lastSelectedShape = touchedObject.transform.root.gameObject;
    }

    public void playSuccessEffect(GameObject shape)
    {
        var particleEffect = Instantiate(successEffect, shape.transform.root.transform);
        Destroy(particleEffect, 2f);
    }
}
