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
        
        if (isBeginningObjectSpawned == true && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                transform.position = new Vector3(transform.position.x + touch.deltaPosition.x * objectMovingSpeed, transform.position.y, transform.position.z + touch.deltaPosition.y * objectMovingSpeed);
            }
            //touchPosition = touch.position;

            //if (touch.phase == TouchPhase.Began)
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(touch.position);
            //    RaycastHit hitObject;
            //    if (Physics.Raycast(ray, out hitObject, 5.0f))
            //    {
            //        placedObject = hitObject.transform.GetComponent<GameObject>();

            //        MeshRenderer meshRenderer = placedObject.GetComponent<MeshRenderer>();
            //        meshRenderer.material.color = new Color(1, 0, 0, 1);
            //        if (hitObject.transform.tag.Contains("bottle"))
            //        {
            //            isTouchHolding = true;
            //        }
            //    }
            //}
            //if (touch.phase == TouchPhase.Ended)
            //{
            //    isTouchHolding = false;
            //}
            //if (isTouchHolding)
            //{
            //    //touchPosition = touch.position;
            //    placedObject.transform.position = touch.position;
            //    //placedObject.transform.rotation = touch.rotation;
            //}
        }
        //if (isTouchHolding)
        //{
        //    placedObject.transform.position = hitPose.position;
        //    placedObject.transform.rotation = hitPose.rotation;
        //}
    }

    private void PlaceObjectBeginning()
    {
        // things to place related to cabinet
        Instantiate(cabinetToPlace, placementPose.position + new Vector3(0.0f, 0.5f, 0.5f), placementPose.rotation);
        Instantiate(cup, placementPose.position + new Vector3(0.0f, 1.0f, 0.5f), placementPose.rotation);

        // things to place related to shelf
        Instantiate(shelfToPlace, placementPose.position + new Vector3(0.0f, 0.4f, -0.4f), placementPose.rotation);
        Instantiate(bottle, placementPose.position + new Vector3(0.0f, 0.8f, -0.4f), placementPose.rotation);

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
