using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{

    public GameObject arCubeToSpawn;
    public GameObject arCharacterToSpawn;
    public GameObject placementIndicator;
    public Camera arCamera;
    public int rotateDegree = 1;

    private GameObject spawnedObject;
    private ARRaycastManager aRRaycastManager;
    private Pose PlacementPose;
    private bool layoutPlaced = false;
    private bool placementPoseIsValid = false;
    private GameObject touchedObject;
    private Vector2 initTouchPosition;

    private Vector2 prevPos;
    private bool IsSelected = false;
    private Vector3 currentEulerAngles;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    // need to update placement indicator, placement pose and spawn 
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
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hitObject))
                {
                    initTouchPosition = touch.position;
                    touchedObject = hitObject.transform.gameObject;

                    Debug.Log("initTouchPosition: " + initTouchPosition.y + " , " + initTouchPosition.x);
                    Debug.Log("name: " + hitObject.transform.name);
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 newTouchPosition = Input.GetTouch(0).position;
                Debug.Log("newTouchPosition: " + newTouchPosition.y + " , " + newTouchPosition.x);
                if(touchedObject != null)
                {
                    if(newTouchPosition.y > initTouchPosition.y || newTouchPosition.x > initTouchPosition.x)
                    {
                        Debug.Log("rotating upward");
                        touchedObject.transform.parent.Rotate(new Vector3(rotateDegree, 0, 0));
                    }
                    else
                    {
                        Debug.Log("rotating downward");
                        touchedObject.transform.parent.Rotate(new Vector3(-rotateDegree, 0, 0));
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                Debug.Log("unselected!!!");
                touchedObject = null;
            }
        }
    }

    void UpdatePlacementIndicator()
    {
        if (spawnedObject == null && placementPoseIsValid && layoutPlaced == false)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

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
        arCharacterToSpawn = Instantiate(arCharacterToSpawn, PlacementPose.position + new Vector3(0.0f, 0.0f, -0.05f), PlacementPose.rotation);
        // to be placed in the sky and dropped down
        arCubeToSpawn = Instantiate(arCubeToSpawn, PlacementPose.position + new Vector3(0.0f, 0.0f, 0.05f), PlacementPose.rotation);

        placementIndicator.SetActive(false);
        layoutPlaced = true;
    }
}
