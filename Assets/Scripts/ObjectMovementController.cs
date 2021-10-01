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

    void Start()
    {
        birthdayCardController = FindObjectOfType<BirthdayCardController>();
        characterController = FindObjectOfType<CharacterController>();
        gameController = FindObjectOfType<GameController>();
        soundManager = FindObjectOfType<SoundManager>();
        utils = FindObjectOfType<HelperUtils>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isObjectMovementEnabled && Input.touchCount > 0)
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
                    Debug.Log("hit position vector3: " + hitObject.point);
                    // arCamera.ScreenToWorldPoint(new Vector3(initTouchPosition.x, initTouchPosition.y, arCamera.nearClipPlane)) // doesnt work, always the same value
                    touchedObject = hitObject.transform.gameObject;
                    Debug.Log("touchedObject location: " + touchedObject.transform.position);
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
                        switch (touchedObject.name)
                        {
                            case "BirthdayCardToFold":
                                var initialRotation = birthdayCardController.GetInitialDegree();
                                Debug.Log(newRealWorldPosition + " : " + initialRealWorldPosition);
                                // in charge of moving
                                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                                {
                                    touchedObject.transform.Rotate(new Vector3(1.5f, 0, 0));
                                    birthdayCardController.SwitchOffAnimation();
                                }
                                // in charge of snapping logic
                                var eulerAngle = touchedObject.transform.eulerAngles;
                                Debug.Log("touched object angle: " + eulerAngle);
                                if (eulerAngle.y > 40 + initialRotation)
                                {
                                    soundManager.PlaySuccessSound();
                                    var duration = birthdayCardController.PlayBirthdayCardCompleteWithSubtitles();
                                    var completedBirthdayCard = birthdayCardController.GetCompletedBirthdayCard();
                                    snapObject(eulerAngle.x, 60 + initialRotation, eulerAngle.z, touchedObject, duration);
                                }
                                break;
                            default:
                                Debug.Log("objectname: " + touchedObject.name);
                                break;
                        }
                    }
                }
            }
        }
    }

    private void snapObject(float x, float y, float z, GameObject gameObject, float duration)
    {
        Debug.Log("snapped object is: " + touchedObject.name + " with local angle: " + touchedObject.transform.eulerAngles + " and parent angle: " + touchedObject.transform.parent.transform.eulerAngles);
        Vector3 newAngle = new Vector3(x, y, z);
        touchedObject.transform.eulerAngles = newAngle;
        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
        Debug.Log("angle to assign: " + newAngle + "new parent object rot angle: " + touchedObject.transform.eulerAngles);
        // play audio & subtitle
        //var duration = gameController.StartCompleteCubeSubtitleWithAudio();
        // move cube to character
        StartCoroutine(utils.LerpMovement(touchedObject.transform.position, characterController.GetArCharacterPosition(), duration, touchedObject.transform.parent.gameObject));
        touchedObject = null; // unselect object
    }

    public void SetObjectMovementEnabled(bool isActive)
    {
        isObjectMovementEnabled = isActive;
    }
}
