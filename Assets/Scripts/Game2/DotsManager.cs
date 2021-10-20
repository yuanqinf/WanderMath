using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DotsManager : Singleton<DotsManager>
{
    public bool isDotsPlaced = false;
    public GameObject dot;
    public GameObject plane;
    [SerializeField]
    private Camera arCamera;
    private PlacementIndicatorController placementController;
    private ARDrawManager arDrawManager;
    public Pose placementPose;
    public List<GameObject> dots = new List<GameObject>();
    public string gamePhase = "waiting";

    public GameObject flatRectangle;

    private void Start()
    {
        placementController = FindObjectOfType<PlacementIndicatorController>();
        arDrawManager = FindObjectOfType<ARDrawManager>();
    }

    private void Update()
    {
        if (!isDotsPlaced)
        {
            placementPose = placementController.UpdatePlacementAndPose(arCamera, placementPose);
            if (placementController.GetIsPlacementPoseValid() && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                isDotsPlaced = true;
                // change this to determine which phase to go to
                gamePhase = "phase1";
            }
        }
        else
        {
            arDrawManager.DrawOnTouch();
            switch (gamePhase)
            {
                case "phase0":
                    InstantiatePhase0Dots();
                    gamePhase = "waiting";
                    break;
                case "phase1":
                    InstantiatePhase1Dots();
                    gamePhase = "waiting";
                    break;
                case "phase2":
                    gamePhase = "waiting";
                    break;
                case "phase3":
                    gamePhase = "waiting";
                    break;
                default:
                    break;
            }
        }
    }

    private void InstantiatePhase0Dots()
    {
        Vector3 cornerPos1 = placementPose.position
            + (placementPose.forward * 0.3f) + (placementPose.right * 0.3f);
        Vector3 cornerPos2 = placementPose.position
            + (placementPose.forward * 0.3f) + (placementPose.right * -0.3f);
        InstantiateWithAnchor(plane, placementPose.position, placementPose.rotation);
        InstantiateWithAnchor(dot, cornerPos1, placementPose.rotation);
        InstantiateWithAnchor(dot, cornerPos2, placementPose.rotation);
        placementController.TurnOffPlacementAndText();
    }

    private void InstantiatePhase1Dots()
    {
        Vector3 cornerPos1 = placementPose.position
            + (placementPose.forward * 0.3f) + (placementPose.right * 0.3f);
        Vector3 cornerPos2 = placementPose.position
            + (placementPose.forward * 0.3f) + (placementPose.right * -0.3f);
        Vector3 cornerPos3 = placementPose.position
            + (placementPose.forward * -0.3f) + (placementPose.right * 0.3f);
        Vector3 cornerPos4 = placementPose.position
            + (placementPose.forward * -0.3f) + (placementPose.right * -0.3f);
        InstantiateWithAnchor(plane, placementPose.position, placementPose.rotation);
        InstantiateWithAnchor(dot, cornerPos1, placementPose.rotation);
        InstantiateWithAnchor(dot, cornerPos2, placementPose.rotation);
        InstantiateWithAnchor(dot, cornerPos3, placementPose.rotation);
        InstantiateWithAnchor(dot, cornerPos4, placementPose.rotation);
        placementController.TurnOffPlacementAndText();

        flatRectangle = Instantiate(flatRectangle, placementPose.position, placementPose.rotation);
    }

    public void ClearDots()
    {
        isDotsPlaced = false;
        placementController.SetLayoutPlaced(false);
        foreach (GameObject dot in dots)
        {
            Destroy(dot);
        }
    }

    private void InstantiateWithAnchor(GameObject prefab, Vector3 pos, Quaternion rotation)
    {
        var instance = Instantiate(prefab, pos, rotation);
        if (instance.GetComponent<ARAnchor>() == null)
        {
            instance.AddComponent<ARAnchor>();
        }
        dots.Add(instance);
    }

    public void CreateRectangle(HashSet<GameObject> connectedDots)
    {

    }
}
