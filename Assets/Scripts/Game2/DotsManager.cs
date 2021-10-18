using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsManager : Singleton<DotsManager>
{
    public bool isDotsPlaced = false;
    public GameObject dot;
    public GameObject plane;
    [SerializeField]
    private Camera arCamera;
    private PlacementIndicatorController placementController;
    public Pose placementPose;

    private void Start()
    {
        placementController = FindObjectOfType<PlacementIndicatorController>();
    }

    private void Update()
    {
        if (!isDotsPlaced)
        {
            placementPose = placementController.UpdatePlacementAndPose(arCamera, placementPose);
            if (placementController.GetIsPlacementPoseValid() && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                InstantiatePhase0Dots();
                isDotsPlaced = true;
            }
        }
    }

    public void InstantiatePhase0Dots()
    {
        Vector3 cornerPos1 = placementPose.position
            + (placementPose.forward * 0.3f) + (placementPose.right * 0.3f);
        Vector3 cornerPos2 = placementPose.position
            + (placementPose.forward * 0.3f) + (placementPose.right * -0.3f);
        Vector3 cornerPos3 = placementPose.position
            + (placementPose.forward * -0.3f) + (placementPose.right * 0.3f);
        Vector3 cornerPos4 = placementPose.position
            + (placementPose.forward * -0.3f) + (placementPose.right * -0.3f);
        Instantiate(plane, placementPose.position, placementPose.rotation);
        Instantiate(dot, cornerPos1, placementPose.rotation);
        Instantiate(dot, cornerPos2, placementPose.rotation);
        Instantiate(dot, cornerPos3, placementPose.rotation);
        Instantiate(dot, cornerPos4, placementPose.rotation);
        placementController.TurnOffPlacementAndText();
    }
}
