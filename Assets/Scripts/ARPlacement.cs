using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARPlacement : MonoBehaviour
{
    public GameObject arCubeToSpawn;
    public GameObject placementIndicator;
    public RectTransform sliderHandleTransform;
    public Camera arCamera;
    public float rotateDegreeFactor;

    private Pose placementPose; // temp to be removed

    private bool layoutPlaced = false;
    private GameObject touchedObject;
    private Vector2 initTouchPosition;

    private UiController uiController;
    private GameController gameController;
    private HelperUtils utils;

    //private GameObject arCube
    private GameObject arCharacter;
    private bool isPlane2Snapped = false;
    private bool isPlane3Snapped = false;
    private int snappedSides = 0;


    void Start()
    {
        uiController = FindObjectOfType<UiController>();
        gameController = FindObjectOfType<GameController>();
        utils = FindObjectOfType<HelperUtils>();
    }

    void Update()
    {
        // second part: click object to move towards you
        if (layoutPlaced && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            RaycastHit hitObject;
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hitObject))
                {
                    initTouchPosition = touch.position;
                    touchedObject = hitObject.transform.gameObject;
                    // move cube1 closer to player as a whole
                    // TODO: refactor code into GameController
                    if (touchedObject.tag == "cube1")
                    {
                        uiController.SetCursorActive(true);

                        // play audio & show subtitle
                        var duration = gameController.StartSelectSubtitleWithAudio();
                        // move object towards user
                        var startPos = touchedObject.transform.position;
                        var endPos = startPos + new Vector3(
                            0,
                            (arCamera.transform.position.y - startPos.y) / 2,
                            (arCamera.transform.position.z - startPos.z) / 4
                        );

                        touchedObject.GetComponent<BoxCollider>().enabled = false;
                        var emission = touchedObject.GetComponent<ParticleSystem>().emission;
                        emission.enabled = false;
                        StartCoroutine(utils.LerpMovement(startPos, endPos, duration, touchedObject));
                    }
                }
            }

            // third part: interact with cube to move
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 newTouchPosition = Input.GetTouch(0).position;
                uiController.SetCursorPosition(newTouchPosition);
                if (touchedObject != null)
                {
                    switch (touchedObject.name)
                    {
                        case "Plane1":
                            if (newTouchPosition.y > initTouchPosition.y)
                            {
                                //touchedObject.transform.parent.Rotate(new Vector3(1, 0, 0));
                            } else if (newTouchPosition.y < initTouchPosition.y)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(-1, 0, 0));
                            }
                            break;
                        case "Plane3":
                            if (newTouchPosition.y > initTouchPosition.y)
                            {
                                //touchedObject.transform.parent.Rotate(new Vector3(1, 0, 0));
                            }
                            else if (newTouchPosition.y < initTouchPosition.y)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(-1, 0, 0));
                            }
                            break;
                        case "Plane2":
                            if (newTouchPosition.x > initTouchPosition.x)
                            {
                                //touchedObject.transform.parent.Rotate(new Vector3(1, 0, 0));
                            }
                            else if (newTouchPosition.x < initTouchPosition.x)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(-1, 0, 0));
                            }
                            break;
                        case "Plane5":
                            if (newTouchPosition.x > initTouchPosition.x)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(-1, 0, 0));
                            }
                            else if (newTouchPosition.x < initTouchPosition.x)
                            {
                                //touchedObject.transform.parent.Rotate(new Vector3(1, 0, 0));
                            }
                            break;
                        case "Plane6":
                            if (newTouchPosition.y > initTouchPosition.y)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(-1, 0, 0));
                            }
                            else if (newTouchPosition.y < initTouchPosition.y)
                            {
                                //touchedObject.transform.parent.Rotate(new Vector3(1, 0, 0));
                            }
                            break;
                        default:
                            Debug.Log("objectname: " + touchedObject.name);
                            break;
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                touchedObject = null; // unselect object
            }
        }

        // slider UI

        //if (touchedObject != null && sliderHandleTransform.localPosition.y > 2 || sliderHandleTransform.localPosition.y < -2)
        //{
        //    touchedObject.transform.parent.Rotate(new Vector3(-sliderHandleTransform.localPosition.y * rotateDegreeFactor, 0, 0));
        //}

        // snap
        if (touchedObject != null)
        {
            switch (touchedObject.name)
            {
                case "Plane1":
                    Debug.Log("plane 1 angle: " + touchedObject.transform.parent.transform.eulerAngles);
                    if (isPlane3Snapped == false && touchedObject.transform.parent.transform.eulerAngles.x < 280 &&
                        touchedObject.transform.parent.transform.eulerAngles.x > 260)
                    {
                        Debug.Log("touched object 1 snap" + touchedObject.transform.parent.transform.eulerAngles);
                        snapObject(-90, touchedObject.transform.parent.transform.eulerAngles.y, 0);
                    } else if (isPlane2Snapped == false && touchedObject.transform.parent.transform.eulerAngles.y < 280 &&
                        touchedObject.transform.parent.transform.eulerAngles.y > 260)
                    {
                        Debug.Log("touched object 1 for plane y" + touchedObject.transform.parent.transform.eulerAngles);
                        snapObject(0, 270f, touchedObject.transform.parent.transform.eulerAngles.z);
                    } else if (isPlane3Snapped && (touchedObject.transform.parent.eulerAngles.x > 350 ||
                             touchedObject.transform.parent.eulerAngles.x < 10))
                    {
                        Debug.Log("touched object 1 after plane3" + touchedObject.transform.parent.transform.eulerAngles);
                        snapObject(0, touchedObject.transform.parent.transform.eulerAngles.y, 0);
                    }
                    break;
                case "Plane2":
                    if (touchedObject.transform.parent.transform.eulerAngles.x < 280 &&
                        touchedObject.transform.parent.transform.eulerAngles.x > 260)
                    {
                        Debug.Log("touched object 2 snap" + touchedObject.transform.parent.transform.eulerAngles);
                        snapObject(-90, touchedObject.transform.parent.transform.eulerAngles.y, 0);
                        isPlane2Snapped = true;
                    }
                    else if (touchedObject.transform.parent.transform.eulerAngles.y < 190 &&
                        touchedObject.transform.parent.transform.eulerAngles.y > 170)
                    {
                        Debug.Log("touched object 2 y snap" + touchedObject.transform.eulerAngles);
                        snapObject(0, 180f, touchedObject.transform.parent.transform.eulerAngles.z);
                        isPlane2Snapped = true;
                    }
                    break;
                case "Plane5":
                    if (touchedObject.transform.parent.transform.eulerAngles.x < 280 &&
                        touchedObject.transform.parent.transform.eulerAngles.x > 260)
                    {
                        Debug.Log("touched object 5 snap" + touchedObject.transform.parent.transform.eulerAngles);
                        snapObject(-90, touchedObject.transform.parent.transform.eulerAngles.y, 0);
                    }
                    break;
                case "Plane3":
                    if (touchedObject.transform.parent.transform.eulerAngles.x < 285 &&
                        touchedObject.transform.parent.transform.eulerAngles.x > 255)
                    {
                        Debug.Log("touched object 3 x snap" + touchedObject.transform.parent.transform.eulerAngles);
                        snapObject(-90, touchedObject.transform.parent.transform.eulerAngles.y, 0);
                        isPlane3Snapped = true;
                    }
                    else if (touchedObject.transform.parent.transform.eulerAngles.y < 100 &&
                              touchedObject.transform.parent.transform.eulerAngles.y > 80)
                    {
                        Debug.Log("touched object 3 y snap" + touchedObject.transform.eulerAngles);
                        snapObject(0, 90f, touchedObject.transform.parent.transform.eulerAngles.z);
                        isPlane3Snapped = true;
                    }
                    break;
                case "Plane6":
                    if (touchedObject.transform.parent.transform.eulerAngles.x < 280 &&
                        touchedObject.transform.parent.transform.eulerAngles.x > 260)
                    {
                        Debug.Log("touched object 6 x snap" + touchedObject.transform.parent.transform.eulerAngles);
                        snapObject(-90, touchedObject.transform.parent.transform.eulerAngles.y, 0);
                    } else if (touchedObject.transform.parent.transform.eulerAngles.y < 100 &&
                                touchedObject.transform.parent.transform.eulerAngles.y > 80)
                    {
                        Debug.Log("touched object 6 y snap" + touchedObject.transform.eulerAngles);
                        snapObject(0, 90f, touchedObject.transform.parent.transform.eulerAngles.z);
                    }
                    break;
            }
        }
    }

    private void snapObject(float x, float y, float z)
    {
        Debug.Log("snapped object is: " + touchedObject.name + " with local angle: " + touchedObject.transform.eulerAngles + " and parent angle: " + touchedObject.transform.parent.transform.eulerAngles);
        Vector3 newAngle = new Vector3(x, y, z);
        touchedObject.transform.parent.transform.eulerAngles = newAngle;
        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
        Debug.Log("angle to assign: " + newAngle + "new parent object rot angle: " + touchedObject.transform.parent.transform.eulerAngles);
        touchedObject = null; // unselect object
        snappedSides += 1;

        // completed cube & move to chinchilla
        if (snappedSides == 5)
        {
            // play audio & subtitle
            var duration = 5f;
            uiController.SetCursorActive(false);
            // move cube to character
            StartCoroutine(utils.LerpMovement(arCubeToSpawn.transform.position, arCharacter.transform.position, duration, arCubeToSpawn));
            snappedSides = 0;
        }
    }

    #region AR object placement code

    /// <summary>
    /// Initialize object based on duration & distance it'll float from.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="upDistance"></param>
    public void PlaceCubeInSky(float duration, float upDistance)
    {
        // cube to be placed in the sky and dropped down
        var endPos = placementPose.position + new Vector3(0.0f, 0.0f, 0.05f);
        var startPos = placementPose.position + new Vector3(0.0f, 0.0f, 0.05f) + Vector3.up * upDistance;
        // stop particle effect & collision
        arCubeToSpawn.GetComponent<BoxCollider>().enabled = false;
        var emission = arCubeToSpawn.GetComponent<ParticleSystem>().emission;

        Quaternion cubeRot = Quaternion.Euler(placementPose.rotation.eulerAngles);

        emission.enabled = false;
        arCubeToSpawn = Instantiate(
            arCubeToSpawn,
            startPos,
            cubeRot
        );
        StartCoroutine(utils.LerpMovement(startPos, endPos, duration, arCubeToSpawn));
        StartCoroutine(AddCubeEffect(duration + 4.5f));
    }

    IEnumerator AddCubeEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        // add cube effect and enable box collisder
        arCubeToSpawn.GetComponent<BoxCollider>().enabled = true;
        var emission = arCubeToSpawn.GetComponent<ParticleSystem>().emission;
        emission.enabled = true;
    }
   
    #endregion finish ar object placement
}
