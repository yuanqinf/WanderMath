using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class PlacementIndicatorController : MonoBehaviour
{
    [SerializeField]
    private GameObject placementIndicator;
    [SerializeField]
    private GameObject preStartText;

    private Pose placementPose; // describe position of 3D object in space
    private bool isLayoutPlaced = false;
    private bool isPlacementPoseValid = false;

    private ARRaycastManager aRRaycastManager;

    // Start is called before the first frame update
    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        preStartText.SetActive(false);
    }

    // Getters
    public bool GetIsLayoutPlaced()
    {
        return isLayoutPlaced;
    }

    public Pose GetPlacementPose()
    {
        return placementPose;
    }

    public bool GetIsPlacementPoseValid()
    {
        return isPlacementPoseValid;
    }

    public void UpdatePlacementAndPose(Camera camera)
    {
        UpdatePlacementPose(camera);
        UpdatePlacementIndicator();
    }

    public Transform GetPlacementIndicatorLocation()
    {
        return placementIndicator.transform;
    }

    // Setters
    public void SetLayoutPlaced(bool isActive)
    {
        isLayoutPlaced = isActive;
    }

    /// <summary>
    /// Switch off starting text and placement indicator
    /// </summary>
    public void TurnOffPlacementAndText()
    {
        placementIndicator.SetActive(false);
        preStartText.SetActive(false);
    }

    // Activate the tracker when a horizontal plane is tracked
    private void UpdatePlacementPose(Camera camera)
    {
        var screenCenter = camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        isPlacementPoseValid = hits.Count > 0;
        if (isPlacementPoseValid)
        {
            placementPose = hits[0].pose; // update position
            var cameraForward = camera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    // Enable or disable placement tracker graphics
    private void UpdatePlacementIndicator()
    {
        if (isPlacementPoseValid && isLayoutPlaced == false)
        {
            preStartText.SetActive(false);
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            preStartText.SetActive(true);
            placementIndicator.SetActive(false);
        }
    }
}
