using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // for debugging placement position
    public Pose placementPose;
    public string gamePhase = "setup";

    public string lastGamePhase = "";

    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private GameObject successEffect;
    [SerializeField]
    private GameObject wrongEffect;

    public GameObject giftBox;
    public GameObject cuboidGiftBox;
    public GameObject hexGiftBox;
    public GameObject pyGiftBox;

    private CharacterController characterController;
    private PlacementIndicatorController placementController;
    private BirthdayCardController birthdayCardController;
    private CubeRotateControl cubeController;
    private ShapesController shapesController;
    private UiController uiController;
    private SoundManager soundManager;
    //private GameObject lastSelectedShape = null;

    public bool touchEnabled = true;

    public GameObject helperText;
    public float waitTime = 1f;
    public bool showedHelper = false;

    private Animator helperTextAnimator;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        characterController = FindObjectOfType<CharacterController>();
        cubeController = FindObjectOfType<CubeRotateControl>();
        placementController = FindObjectOfType<PlacementIndicatorController>();
        birthdayCardController = FindObjectOfType<BirthdayCardController>();
        shapesController = FindObjectOfType<ShapesController>();
        uiController = FindObjectOfType<UiController>();
        soundManager = FindObjectOfType<SoundManager>();
        helperTextAnimator = helperText.GetComponent<Animator>();
    }

    private void Update()
    {
        switch (gamePhase)
        {
            // setting up stage
            case Constants.GamePhase.SETUP:
                if (!placementController.GetIsLayoutPlaced())
                {
                    placementPose = placementController.UpdatePlacementAndPose(arCamera, placementPose);
                    if (placementController.GetIsPlacementPoseValid() && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        var audioDuration = PlaceObjectAndAudio();
                        SetGamePhaseWithDelay(Constants.GamePhase.PHASE0, audioDuration);
                        // TODO: change this back to phase0
                    }
                }
                break;
            // birthday card stage
            case Constants.GamePhase.PHASE0:
                // 1. phase0 subtitle, audio, animation
                var duration = birthdayCardController.PlayBirthdayCardInitWithSubtitles();
                characterController.PlayTalkingAnimationWithDuration(duration);
                // 2. initialize birthday card falling with tutorial
                birthdayCardController.InitializeBirthdayCard(placementPose, duration);
                gamePhase = Constants.GamePhase.WAITING;
                lastGamePhase = Constants.GamePhase.PHASE0;
                break;
            // handles first cube
            case Constants.GamePhase.PHASE1:
                // instantiate
                //DestroyImmediate(birthdayCardController.BirthdayCard, true);
                DeactivateObjectWithParticle(GameObject.FindGameObjectWithTag(Constants.Tags.BirthdayCard));
                cubeController.StartPhase1(placementPose);
                gamePhase = Constants.GamePhase.WAITING;
                lastGamePhase = Constants.GamePhase.PHASE1;
                break;
            // handles multiple cubes
            case Constants.GamePhase.PHASE2:
                // instantiate one cube and move on
                DeactivateObjectWithParticle(GameObject.FindGameObjectWithTag(Constants.Tags.CubeMain));
                cubeController.StartPhase2(placementPose);
                gamePhase = Constants.GamePhase.WAITING;
                lastGamePhase = Constants.GamePhase.PHASE2;
                break;
            // handles multiple other shapes
            case Constants.GamePhase.PHASE3:
                foreach (GameObject cube in GameObject.FindGameObjectsWithTag(Constants.Tags.CubeMain))
                {
                    DeactivateObjectWithParticle(cube);
                }
                shapesController.StartPhase3(placementPose);
                gamePhase = Constants.GamePhase.WAITING;
                lastGamePhase = Constants.GamePhase.PHASE3;
                break;
            default:
                break;
        }
    }

    private void DeactivateObjectWithParticle(GameObject obj)
    {
        obj.transform.Find("SpawnParticleEffect/ParticleEffect").GetComponent<ParticleSystem>().Play();
        Destroy(obj, 0.8f);
    }

    /// <summary>
    /// Place character object and starting audio when set
    /// </summary>
    private float PlaceObjectAndAudio()
    {
        float duration = characterController.InitCharacterAndAudioGame1(placementPose, placementController.GetPlacementIndicatorLocation());
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

    public void PlayEndingAnimation()
    {
        StartCoroutine(EndingAnimationWithSound());
    }
    private IEnumerator EndingAnimationWithSound()
    {
        float duration = soundManager.PlayPhase3Ending();
        uiController.PlaySubtitles("Come on, let's go to the party together.", duration);
        characterController.PlaySkatingForward(5);
        yield return new WaitForSeconds(duration + 0.5f);
        FindObjectOfType<MainGM>().ShowCutScene();
    }

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
    //public void handleOutline(GameObject touchedObject)
    //{
    //    // handle outline here
    //    if (this.lastSelectedShape != null && lastSelectedShape != touchedObject.transform.root.gameObject)
    //    {
    //        Debug.Log("outing is being deactivated");
    //        this.lastSelectedShape.GetComponent<Outline>().enabled = false;
    //    }
    //    if (touchedObject.transform.root.GetComponent<Outline>() != null)
    //    {
    //        Debug.Log("outing is being activated");
    //        touchedObject.transform.root.GetComponent<Outline>().enabled = true;
    //    }
    //    this.lastSelectedShape = touchedObject.transform.root.gameObject;
    //}

    public void playSuccessEffect(GameObject shape)
    {
        var particleEffect = Instantiate(successEffect, shape.transform.root.transform);
        Destroy(particleEffect, 2f);
    }

    public void playWrongEffect(GameObject shape)
    {
        var particleEffect = Instantiate(wrongEffect, shape.transform.root.transform);
        Destroy(particleEffect, 2f);
    }

    public void createGiftBox(GameObject shape)
    {
        Instantiate(giftBox, shape.transform.root.transform);
    }

    public void createCuboidGiftBox(GameObject shape)
    {
        Instantiate(cuboidGiftBox, shape.transform.root.transform);
    }

    public void createHexGiftBox(GameObject shape)
    {
        Instantiate(hexGiftBox, shape.transform.root.transform);
    }

    public void createPyGiftBox(GameObject shape)
    {
        Instantiate(pyGiftBox, shape.transform.root.transform);
    }

    public void ResetPhase()
    {
        if (lastGamePhase == "phase0")
        {
            DestroyImmediate(GameObject.FindGameObjectWithTag(Constants.Tags.BirthdayCard), true);
        }
        if (lastGamePhase == "phase1")
        {
            DestroyImmediate(GameObject.FindGameObjectWithTag("cube_main"), true);
        }
        if (lastGamePhase == "phase2")
        {
            foreach (GameObject cube in GameObject.FindGameObjectsWithTag("cube_main"))
            {
                DestroyImmediate(cube, true);
            }
        }
        if (lastGamePhase == "phase3")
        {
            foreach (GameObject cube in GameObject.FindGameObjectsWithTag("cube_main"))
            {
                DestroyImmediate(cube, true);
            }
            shapesController.numShapesCompleted = 0;
        }
        gamePhase = lastGamePhase;
    }

    public void showHelperText()
    {
        waitTime = 2f;
        soundManager.PlayHelperTextSound();
        StartCoroutine(showHelperTextFunc());
    }

    IEnumerator showHelperTextFunc()
    {
        helperTextAnimator.SetTrigger("show");
        yield return new WaitForSeconds(5f);
        helperTextAnimator.SetTrigger("hide");
    }
}
