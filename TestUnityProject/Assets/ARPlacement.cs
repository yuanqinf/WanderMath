using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{

    public GameObject arCupToSpawn;
    public GameObject arBottleToSpawn;
    public GameObject placementIndicator;
    public GameObject wallModel;
    public GameObject shelfModel;
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
        // things to place related to cabinet
        wallModel = Instantiate(wallModel, PlacementPose.position + new Vector3(0.0f, 0.5f, 1.0f), PlacementPose.rotation);
        arCupToSpawn = Instantiate(arCupToSpawn, PlacementPose.position + new Vector3(0.0f, 1.0f, 1.0f), PlacementPose.rotation);

        // things to place related to shelf
        shelfModel = Instantiate(shelfModel, PlacementPose.position + new Vector3(0.0f, 0.5f, -1.0f), PlacementPose.rotation);
        arBottleToSpawn = Instantiate(arBottleToSpawn, PlacementPose.position + new Vector3(0.0f, 1.0f, -1.0f), PlacementPose.rotation);

        placementIndicator.SetActive(false);
        layoutPlaced = true;
    }

    void ARPlaceObject()
    {
        // place more object
    }


}