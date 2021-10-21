using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DotsManager : Singleton<DotsManager>
{
    public bool isDotsPlaced { get; set; }
    public GameObject dot;
    public GameObject plane;
    [SerializeField]
    private Camera arCamera;
    private PlacementIndicatorController placementController;
    private ARDrawManager arDrawManager;
    public Pose placementPose;
    public List<GameObject> dots = new List<GameObject>();
    public List<GameObject> others = new List<GameObject>();
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
                gamePhase = Constants.GamePhase.PHASE1;
                arDrawManager.GamePhase = gamePhase;
                InstantiateOthersWithAnchor(plane, placementPose.position, placementPose.rotation);
                placementController.TurnOffPlacementAndText();
            }
        }
        else
        {
            arDrawManager.DrawOnTouch();
            switch (gamePhase)
            {
                case Constants.GamePhase.PHASE0:
                    InstantiatePhase0Dots();
                    gamePhase = "waiting";
                    break;
                case Constants.GamePhase.PHASE1:
                    InstantiatePhase1Dots();
                    gamePhase = "waiting";
                    break;
                case Constants.GamePhase.PHASE2:
                    gamePhase = "waiting";
                    break;
                case Constants.GamePhase.PHASE3:
                    gamePhase = "waiting";
                    break;
                default:
                    break;
            }
        }
    }

    private void InstantiatePhase0Dots()
    {
        Vector3 cornerPos1 = placementPose.position + (placementPose.right * Constants.ONE_FEET);
        Vector3 cornerPos2 = placementPose.position + (placementPose.right * -Constants.ONE_FEET);
        InstantiateDotsWithAnchor(dot, cornerPos1, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos2, placementPose.rotation);
    }

    private void InstantiatePhase1Dots()
    {
        Vector3 cornerPos1 = placementPose.position
            + (placementPose.forward * Constants.ONE_FEET) + (placementPose.right * Constants.ONE_FEET);
        Vector3 cornerPos2 = placementPose.position
            + (placementPose.forward * Constants.ONE_FEET) + (placementPose.right * -Constants.ONE_FEET);
        Vector3 cornerPos3 = placementPose.position
            + (placementPose.forward * -Constants.ONE_FEET) + (placementPose.right * Constants.ONE_FEET);
        Vector3 cornerPos4 = placementPose.position
            + (placementPose.forward * -Constants.ONE_FEET) + (placementPose.right * -Constants.ONE_FEET);
        InstantiateDotsWithAnchor(dot, cornerPos1, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos2, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos3, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos4, placementPose.rotation);

        //ActivatePhase1Cube();
    }

    private void ActivatePhase1Cube()
    {
        flatRectangle = Instantiate(flatRectangle, placementPose.position, placementPose.rotation);
    }


    #region Deleting objects
    public void ClearAllObjects()
    {
        ClearDots();
        arDrawManager.ClearLines();
        foreach(GameObject other in others)
        {
            Destroy(other);
        }
    }

    public void ClearDots()
    {
        foreach (GameObject dot in dots)
        {
            Destroy(dot);
        }
    }
    #endregion

    #region Instantiate objects
    private void InstantiateOthersWithAnchor(GameObject prefab, Vector3 pos, Quaternion rotation)
    {
        others.Add(InstantiateWithAnchor(prefab, pos, rotation));
    }

    private void InstantiateDotsWithAnchor(GameObject prefab, Vector3 pos, Quaternion rotation)
    {
        dots.Add(InstantiateWithAnchor(prefab, pos, rotation));
    }
    private GameObject InstantiateWithAnchor(GameObject prefab, Vector3 pos, Quaternion rotation)
    {
        var instance = Instantiate(prefab, pos, rotation);
        if (instance.GetComponent<ARAnchor>() == null)
        {
            instance.AddComponent<ARAnchor>();
        }
        return instance;
    }
    #endregion
}
