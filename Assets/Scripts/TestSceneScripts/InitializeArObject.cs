using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class InitializeArObject : GenericClass
{
    public GameObject objectToSpawn;
    public GameObject placementIndicator;
    public Camera arCamera;

    private Pose placementPose;
    private ARRaycastManager aRRaycastManager;
    private bool isPlacementPoseValid = false;
    private bool isObjectPlaced = false;

    private GameObject touchedObject;
    private Vector3 initialRealWorldPosition;
    private CuboidController cuboidController;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        arCamera = FindObjectOfType<Camera>();
        cuboidController = FindObjectOfType<CuboidController>();
    }

    private void Update()
    {
        if (!isObjectPlaced)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
            if (isPlacementPoseValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //Quaternion.Euler(new Vector3(placementPose.rotation.x, placementPose.rotation.y + 180, placementPose.rotation.z))
                Instantiate(objectToSpawn, placementPose.position, placementPose.rotation);
                isObjectPlaced = true;
                placementIndicator.SetActive(false);
            }
        }
        else if (isObjectPlaced && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            RaycastHit hitObject;
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hitObject))
                {
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
                            case "cuboid":
                                cuboidController.UpdateCuboidRotation(touchedObject, newRealWorldPosition, initialRealWorldPosition);
                                this.touchedObject = null;
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

    #region only used for placement
    private void UpdatePlacementIndicator()
    {
        if (isPlacementPoseValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        isPlacementPoseValid = hits.Count > 0;
        if (isPlacementPoseValid)
        {
            placementPose = hits[0].pose; // update position
            var cameraForward = arCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
    #endregion
}
