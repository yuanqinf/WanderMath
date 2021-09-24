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

    public Canvas UiCanvas;

    private ARRaycastManager aRRaycastManager;
    private Pose PlacementPose;
    private bool layoutPlaced = false;
    private bool placementPoseIsValid = false;
    private GameObject touchedObject;
    private Vector2 initTouchPosition;
    private HandleSnapControl handleSnapControl;


    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        handleSnapControl = sliderHandleTransform.gameObject.GetComponentInParent<HandleSnapControl>();
    }

    void Update()
    {
        if (!layoutPlaced)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ARPlaceLayout();
            }
        }

        if (layoutPlaced && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            RaycastHit hitObject;
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.current.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hitObject))
                {
                    initTouchPosition = touch.position;
                    touchedObject = hitObject.transform.gameObject;
                    // move cube1 closer to player as a whole
                    if (touchedObject.tag == "cube1")
                    {
                        var startPos = touchedObject.transform.position;
                        var endPos = startPos + new Vector3(
                            0,
                            (Camera.main.transform.position.y - startPos.y) / 2,
                            (Camera.main.transform.position.z - startPos.z) / 4
                        );
                        var timeTakenToMove = 5.0f;

                        touchedObject.GetComponent<BoxCollider>().enabled = false;
                        StartCoroutine(LerpMovement(startPos, endPos, timeTakenToMove, touchedObject));
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 newTouchPosition = Input.GetTouch(0).position;
                Debug.Log("newTouchPosition: " + newTouchPosition.x + " , " + newTouchPosition.y);
                if (touchedObject != null)
                {
                    Debug.Log(touchedObject.name);
                    switch (touchedObject.name)
                    {
                        case "Plane1": case "Plane3":
                            if (newTouchPosition.y > initTouchPosition.y)
                            {
                                touchedObject.transform.parent.Rotate(new Vector3(1, 0, 0));
                            } else if (newTouchPosition.y < initTouchPosition.y)
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
                //touchedObject = null; // unselect object
                //float curAngle = touchedObject.transform.parent.GetComponent<Transform>().localRotation.eulerAngles.x;
                //Debug.Log("unselected!!! : " + curAngle);
                //if (curAngle > 80 && curAngle < 100)
                //{
                //    touchedObject.transform.parent.Rotate(new Vector3(180, 0, 0));
                //}
            }
        }

        if (sliderHandleTransform.localPosition.y > 2 || sliderHandleTransform.localPosition.y < -2)
        {
            touchedObject.transform.parent.Rotate(new Vector3(-sliderHandleTransform.localPosition.y * rotateDegreeFactor, 0, 0));
        }

        // snap
        if (touchedObject.transform.parent.GetComponent<Transform>().rotation.eulerAngles.x < 280 &&
            touchedObject.transform.parent.GetComponent<Transform>().rotation.eulerAngles.x > 260)
        {
            Vector3 newAngle = new Vector3(-90, touchedObject.transform.eulerAngles.y, touchedObject.transform.eulerAngles.z);
            touchedObject.transform.parent.transform.eulerAngles = newAngle;
            touchedObject.transform.GetComponent<BoxCollider>().enabled = false;


            //float curAngle = touchedObject.transform.parent.GetComponent<Transform>().localRotation.eulerAngles.x;
            //if (curAngle > 180)
            //{
            //    curAngle -= 360;
            //}

            //Debug.Log("touchedObject curAngle: " + curAngle);
            //if (curAngle > 50)
            //{
            //    Debug.Log("snap now!!! >50");

            //    touchedObject.transform.parent.Rotate((90 - touchedObject.transform.localRotation.eulerAngles.x), 0, 0);
            //}

            //if (curAngle < -50)
            //{
            //    Debug.Log("snap now!!! < -50");
            //    touchedObject.transform.parent.Rotate((-90 - touchedObject.transform.localRotation.eulerAngles.x), 0, 0);
            //}
            //handleSnapControl.canSnap = false;
        }
    }

    #region AR object placement code
    // Enable or disable placement tracker graphics
    void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && layoutPlaced == false)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    // Activate the tracker when a horizontal plane is tracked
    void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
        }
    }

    private void ARPlaceLayout()
    {
        // things to place when initialized
        // to be placed at the corner
        arCharacterToSpawn = Instantiate(
            arCharacterToSpawn,
            PlacementPose.position + new Vector3(-0.5f, 0.0f, -0.01f),
            arCharacterToSpawn.transform.rotation
        );
        //arCharacterToSpawn.transform.Rotate(0.0f, 180.0f, 0.0f);
        // cube to be placed in the sky and dropped down
        var movementDistance = 0.25f; // distance to move down (1.0f is 1 meter)

        var endPos = PlacementPose.position + new Vector3(0.0f, 0.0f, 0.05f);
        var startPos = PlacementPose.position + new Vector3(0.0f, 0.0f, 0.05f) + Vector3.up * movementDistance;
        arCubeToSpawn = Instantiate(
            arCubeToSpawn,
            startPos,
            PlacementPose.rotation
        );

        placementIndicator.SetActive(false);
        layoutPlaced = true;

        var startingLerpTime = 3.0f; // duration to leap (10s)
        StartCoroutine(LerpMovement(startPos, endPos, startingLerpTime, arCubeToSpawn));
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
