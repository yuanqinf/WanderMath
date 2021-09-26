using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARPlacement : MonoBehaviour
{

    public GameObject arCubeToSpawn;
    public GameObject arCharacterToSpawn;
    public GameObject placementIndicator;
    public RectTransform sliderHandleTransform;
    public Camera arCamera;
    public float rotateDegreeFactor;

    private ARRaycastManager aRRaycastManager;
    private Pose placementPose; // describe position of 3D object in space
    private bool layoutPlaced = false;
    private bool placementPoseIsValid = false;
    private GameObject touchedObject;
    private Vector2 initTouchPosition;
    private UiController uiController;
    private GameController gameController;

    private bool isPlane2Snapped = false;
    private bool isPlane3Snapped = false;
    private int snappedSides = 0;


    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        uiController = FindObjectOfType<UiController>();
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        // first part: object placement
        if (!layoutPlaced)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceCharacterObject();
                StartSubtitles();
                placementIndicator.SetActive(false);
                uiController.SetPreStartTextActive(false); // remove preStart text
                layoutPlaced = true;
            }
        }

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
                        StartCoroutine(LerpMovement(startPos, endPos, duration, touchedObject));
                    }
                }
            }

            // third part: interact with cube to move
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 newTouchPosition = Input.GetTouch(0).position;
                if (touchedObject != null)
                {
                    switch (touchedObject.name)
                    {
                        case "Plane1":
                            if (newTouchPosition.y > initTouchPosition.y)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(1, 0, 0));
                            } else if (newTouchPosition.y < initTouchPosition.y)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(-1, 0, 0));
                            }
                            break;
                        case "Plane3":
                            if (newTouchPosition.y > initTouchPosition.y)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(1, 0, 0));
                            }
                            else if (newTouchPosition.y < initTouchPosition.y)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(-1, 0, 0));
                            }
                            break;
                        case "Plane2":
                            if (newTouchPosition.x > initTouchPosition.x)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(1, 0, 0));
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
                                touchedObject.transform.parent.Rotate(new Vector3(1, 0, 0));
                            }
                            break;
                        case "Plane6":
                            if (newTouchPosition.y > initTouchPosition.y)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(-1, 0, 0));
                            }
                            else if (newTouchPosition.y < initTouchPosition.y)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(1, 0, 0));
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
                //float curAngle = touchedObject.transform.parent.GetComponent<Transform>().localRotation.eulerAngles.x;
                //Debug.Log("unselected!!! : " + curAngle);
                //if (curAngle > 80 && curAngle < 100)
                //{
                //    touchedObject.transform.parent.Rotate(new Vector3(180, 0, 0));
                //}
            }
        }

        if (touchedObject != null && sliderHandleTransform.localPosition.y > 2 || sliderHandleTransform.localPosition.y < -2)
        {
            touchedObject.transform.parent.Rotate(new Vector3(-sliderHandleTransform.localPosition.y * rotateDegreeFactor, 0, 0));
        }

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
            var duration = gameController.StartCompleteCubeSubtitleWithAudio();
            // move cube to character
            StartCoroutine(LerpMovement(arCubeToSpawn.transform.position, arCharacterToSpawn.transform.position, duration, arCubeToSpawn));
            snappedSides = 0;
        }
    }

    #region AR object placement code
    // Enable or disable placement tracker graphics
    void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && layoutPlaced == false)
        {
            uiController.SetPreStartTextActive(false); // remove preStart text
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            uiController.SetPreStartTextActive(true); // enable preStart text
            placementIndicator.SetActive(false);
        }
    }

    // Activate the tracker when a horizontal plane is tracked
    void UpdatePlacementPose()
    {
        var screenCenter = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose; // update position
            var cameraForward = arCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

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
        emission.enabled = false;
        arCubeToSpawn = Instantiate(
            arCubeToSpawn,
            startPos,
            arCubeToSpawn.transform.rotation // placementPose.rotation *
        );
        StartCoroutine(LerpMovement(startPos, endPos, duration, arCubeToSpawn));
        StartCoroutine(AddCubeEffect(duration));
    }

    IEnumerator AddCubeEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        // add cube effect and enable box collisder
        arCubeToSpawn.GetComponent<BoxCollider>().enabled = true;
        var emission = arCubeToSpawn.GetComponent<ParticleSystem>().emission;
        emission.enabled = true;
    }

    private void PlaceCharacterObject()
    {
        // to be placed at the corner
        Debug.Log("placement Pose: " + placementPose.rotation);
        arCharacterToSpawn = Instantiate(
            arCharacterToSpawn,
            placementPose.position + new Vector3(-0.5f, 0.0f, -0.01f),
            arCharacterToSpawn.transform.rotation // placementPose.rotation *
        );
    }

    private void StartSubtitles()
    {
        gameController.StartSubtitlesWithAudio();
    }

    /// <summary>
    /// Move object from a starting point to an ending point within a lerpTime
    /// </summary>
    /// <param name="startPos">initial pos of object</param>
    /// <param name="endPos">final position of object</param>
    /// <param name="lerpTime">time taken for object to move into position</param>
    /// <param name="gameObject">game object to be moving</param>
    /// <returns></returns>
    IEnumerator LerpMovement(Vector3 startPos, Vector3 endPos, float lerpTime, GameObject gameObject)
    {
        float timeElapsed = 0;

        while (timeElapsed < lerpTime)
        {
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / lerpTime);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
    }
    #endregion finish ar object placement
}
