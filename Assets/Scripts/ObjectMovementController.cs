using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovementController : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera;
    private GameObject touchedObject;
    private Vector3 initialRealWorldPosition;
    private Vector2 initTouchPosition;

    private bool isObjectMovementEnabled = false;
    private CharacterController characterController;
    private GameController gameController;
    private HelperUtils utils;
    private SoundManager soundManager;

    // object controllers
    private BirthdayCardController birthdayCardController;
    private CubeRotateControl cubeControl;
    private CubeEasy cubeEasy;
    private CubeMed cubeMed;
    private CubeMedTwo cubeMedTwo;
    private CubeWrong cubeWrong;
    private PyramidController pyramidController;
    private HexagonController hexagonController;
    private CuboidController cuboidController;

    void Start()
    {
        characterController = FindObjectOfType<CharacterController>();
        gameController = FindObjectOfType<GameController>();
        soundManager = FindObjectOfType<SoundManager>();
        utils = FindObjectOfType<HelperUtils>();
        birthdayCardController = FindObjectOfType<BirthdayCardController>();
        cubeControl = FindObjectOfType<CubeRotateControl>();
        pyramidController = FindObjectOfType<PyramidController>();
        hexagonController = FindObjectOfType<HexagonController>();
        cuboidController = FindObjectOfType<CuboidController>();
        cubeEasy = FindObjectOfType<CubeEasy>();
        cubeMed = FindObjectOfType<CubeMed>();
        cubeMedTwo = FindObjectOfType<CubeMedTwo>();
        cubeWrong = FindObjectOfType<CubeWrong>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (isObjectMovementEnabled && Input.touchCount > 0)

        if (!gameController.touchEnabled) {
            gameController.touchEnabled = true;
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            RaycastHit hitObject;
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hitObject))
                {
                    initTouchPosition = touch.position;
                    initialRealWorldPosition = hitObject.point;
                    //Debug.Log("hit position vector3: " + hitObject.point);
                    // arCamera.ScreenToWorldPoint(new Vector3(initTouchPosition.x, initTouchPosition.y, arCamera.nearClipPlane)) // doesnt work, always the same value
                    touchedObject = hitObject.transform.gameObject;
                    //Debug.Log("touchedObject location: " + touchedObject.transform.position);
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 newTouchPosition = Input.GetTouch(0).position;
                Ray ray = arCamera.ScreenPointToRay(newTouchPosition);
                RaycastHit rayLocation;
                if (Physics.Raycast(ray, out rayLocation))
                {
                    Vector3 newRealWorldPosition = rayLocation.point;
                
                    //uiController.SetCursorPosition(newTouchPosition);
                    if (touchedObject != null)
                    {
                        //Debug.Log("this is touched object name: " + touchedObject.name);

                        //Debug.Log("initialRealWorldPosition: " + initialRealWorldPosition);
                        //Debug.Log("newRealWorldPosition: " + newRealWorldPosition);
                        switch (touchedObject.tag)
                        {
                            case "birthdaycard":
                                birthdayCardController.UpdateCardMovement(touchedObject, newRealWorldPosition, initialRealWorldPosition);
                                break;
                            case "cube_easy":
                                cubeEasy.RotateEasyFace(touchedObject, newRealWorldPosition, initialRealWorldPosition);
                                break;
                            case "cube_med":
                                cubeMed.RotateMedFace(touchedObject, newRealWorldPosition, initialRealWorldPosition);
                                break;
                            case "cube_med2":
                                cubeMedTwo.RotateMedTwoFace(touchedObject, newRealWorldPosition, initialRealWorldPosition);
                                break;
                            case "cube_wrong":
                                cubeWrong.RotateWrongFace(touchedObject, newRealWorldPosition, initialRealWorldPosition);
                                break;
                            case "pyramid":
                                pyramidController.UpdatePyramidRotation(touchedObject, newRealWorldPosition, initialRealWorldPosition);
                                break;
                            case "hexagon":
                                hexagonController.UpdateHexRotation(touchedObject, newRealWorldPosition, initialRealWorldPosition);
                                break;
                            case "cuboid":
                                cuboidController.UpdateCuboidRotation(touchedObject, newRealWorldPosition, initialRealWorldPosition);
                                break;
                            default:
                                //Debug.Log("objectname: " + touchedObject.name);
                                break;
                        }
                    }
                }
            }
        }
    }

    public void ResetGameObject()
    {
        this.touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
        this.touchedObject = null;
    }

    public void SetObjectMovementEnabled(bool isActive)
    {
        isObjectMovementEnabled = isActive;
    }
}
