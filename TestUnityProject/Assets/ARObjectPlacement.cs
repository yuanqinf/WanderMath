using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class ARObjectPlacement : MonoBehaviour
{
    public GameObject cabinetToPlace;
    public GameObject shelfToPlace;
    public GameObject bottle;
    public GameObject cup;
    public GameObject placementIndicator;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager raycastManager;
    private Pose placementPose;
    private Vector2 touchPosition = default;

    private bool placementPoseIsValid = false;
    private bool isBeginningObjectSpawned = false;
    private bool isTouchHolding = false;

    private float objectMovingSpeed;

    private GameObject placedObject;

    void Start()
    {
        //arOrigin = FindObjectOfType<ARSessionOrigin>();
        raycastManager = FindObjectOfType<ARRaycastManager>();
        objectMovingSpeed = 0.001f;
    }

    void Update()
    {
        if (isBeginningObjectSpawned == false)
        {
            UpdatePlacement();
            UpdatePlacementIndicator();
            // if currently touching screen at correct spot
            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObjectBeginning();

            }
        }
        
        //if (isBeginningObjectSpawned == true && Input.touchCount > 0)
        //{
        //    Touch touch = Input.GetTouch(0);
        //    if (touch.phase == TouchPhase.Moved)
        //    {
        //        bottle.transform.position = new Vector3(
        //            bottle.transform.position.x + touch.deltaPosition.x * objectMovingSpeed,
        //            bottle.transform.position.y,
        //            bottle.transform.position.z + touch.deltaPosition.y * objectMovingSpeed
        //        );
        //    }
        //}
    }

    private void PlaceObjectBeginning()
    {
        // things to place related to cabinet
        cabinetToPlace = Instantiate(cabinetToPlace, placementPose.position + new Vector3(0.0f, 0.5f, 0.5f), placementPose.rotation);
        cup = Instantiate(cup, placementPose.position + new Vector3(0.0f, 1.0f, 0.5f), placementPose.rotation);

        // things to place related to shelf
        shelfToPlace =Instantiate(shelfToPlace, placementPose.position + new Vector3(0.0f, 0.5f, -0.5f), placementPose.rotation);
        bottle = Instantiate(bottle, placementPose.position + new Vector3(0.0f, 1.0f, -0.5f), placementPose.rotation);

        placementIndicator.SetActive(false);
        isBeginningObjectSpawned = true; // prevent object from being placed anymore
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacement()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes); // cast origin to space to hit

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            // transform camera facing forward only
            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
