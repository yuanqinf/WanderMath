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
    private BirthdayCardController birthdayCardController;
    private CharacterController characterController;
    private GameController gameController;
    private HelperUtils utils;
    private SoundManager soundManager;
    private CubeRotateControl cubeControl;

    void Start()
    {
        birthdayCardController = FindObjectOfType<BirthdayCardController>();
        characterController = FindObjectOfType<CharacterController>();
        gameController = FindObjectOfType<GameController>();
        soundManager = FindObjectOfType<SoundManager>();
        utils = FindObjectOfType<HelperUtils>();
        cubeControl = FindObjectOfType<CubeRotateControl>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (isObjectMovementEnabled && Input.touchCount > 0)

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
                        switch (touchedObject.tag)
                        {
                            case "birthdaycard":
                                birthdayCardController.UpdateCardMovement(touchedObject, newRealWorldPosition, initialRealWorldPosition);
                                break;
                            case "cube_easy":
                                cubeControl.rotateFace(touchedObject, newRealWorldPosition, initialRealWorldPosition);
                                break;
                            case "cube_wrong":
                                cubeControl.selectWrongCube();
                                touchedObject = null;
                                break;
                            case "cube_main":
                                cubeControl.selectCorrectCube(touchedObject);
                                touchedObject = null;
                                break;
                            default:
                                //Debug.Log("objectname: " + touchedObject.name);
                                break;
                        }

                        Debug.Log("initialRealWorldPosition: " + initialRealWorldPosition);
                        Debug.Log("newRealWorldPosition: " + newRealWorldPosition);
                    }
                }
            }
        }
    }

    public void SetObjectMovementEnabled(bool isActive)
    {
        isObjectMovementEnabled = isActive;
    }
}
