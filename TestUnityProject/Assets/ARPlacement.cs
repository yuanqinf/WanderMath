using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{

    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    public GameObject wallModel;
    private GameObject spawnedObject;
    private ARRaycastManager aRRaycastManager;
    private bool layoutPlaced = false;
    private bool placementPoseIsValid = false;
    private Pose PlacementPose;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    // need to update placement indicator, placement pose and spawn 
    void Update()
    {
        if (!layoutPlaced && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ARPlaceLayout();
        }

        UpdatePlacementPose();
        UpdatePlacementIndicator();


    }
    void UpdatePlacementIndicator()
    {
        if (spawnedObject == null && placementPoseIsValid)
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
        
        GameObject wall1 = Instantiate(wallModel, PlacementPose.position + new Vector3(0.0f, 0.0f, 0.5f), PlacementPose.rotation);
        GameObject wall2 = Instantiate(wallModel, PlacementPose.position + new Vector3(0.0f, 0.0f, -0.5f),  PlacementPose.rotation);
        GameObject wall3 = Instantiate(wallModel, PlacementPose.position + new Vector3(0.5f, 0.0f, 0.0f), PlacementPose.rotation);
        GameObject wall4 = Instantiate(wallModel, PlacementPose.position + new Vector3(-0.5f, 0.0f, 0.0f), PlacementPose.rotation);
        wall3.transform.Rotate(0f, 90f, 0f);
        wall4.transform.Rotate(0f, 90f, 0f);
        spawnedObject = Instantiate(arObjectToSpawn, PlacementPose.position, PlacementPose.rotation);
        layoutPlaced = true;
        placementIndicator.SetActive(false);
    }

    void ARPlaceObject()
    {
        // place more object
    }


}