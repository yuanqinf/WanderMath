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
    private Game2SoundManager g2SoundManager;
    private Game2Manager g2Manager;

    public Pose placementPose;
    public List<GameObject> dots = new List<GameObject>();
    public List<GameObject> others = new List<GameObject>();

    public GameObject flatRectangle;

    public Vector3[,] phase1DotsMatrix;

    private void Start()
    {
        placementController = FindObjectOfType<PlacementIndicatorController>();
        arDrawManager = FindObjectOfType<ARDrawManager>();
        g2SoundManager = FindObjectOfType<Game2SoundManager>();
        g2Manager = FindObjectOfType<Game2Manager>();
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
                var tempPhase = Constants.GamePhase.PHASE0;
                g2Manager.SetGamePhase(tempPhase);
                arDrawManager.GamePhase = tempPhase;
                InstantiateOthersWithAnchor(plane, placementPose.position, placementPose.rotation);
                placementController.TurnOffPlacementAndText();
            }
        }
        else
        {
            arDrawManager.DrawOnTouch();
        }
    }

    public void InstantiatePhase0Dots()
    {
        Vector3 cornerPos1 = placementPose.position + (placementPose.right * Constants.ONE_FEET);
        Vector3 cornerPos2 = placementPose.position + (placementPose.right * -Constants.ONE_FEET);
        InstantiateDotsWithAnchor(dot, cornerPos1, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos2, placementPose.rotation);
    }

    public void InstantiatePhase1Dots()
    {
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE1Start);
        StartCoroutine(SetGamePhase1());
        //ActivatePhase1Cube();
    }

    IEnumerator SetGamePhase1()
    {
        yield return new WaitForSeconds(7f);
        // top left
        Vector3 cornerPos2 = placementPose.position
            + (placementPose.forward * Constants.ONE_FEET) + (placementPose.right * -Constants.ONE_FEET);
        // top right
        Vector3 cornerPos1 = placementPose.position
            + (placementPose.forward * Constants.ONE_FEET) + (placementPose.right * Constants.ONE_FEET);
        // bot left
        Vector3 cornerPos4 = placementPose.position
            + (placementPose.forward * -Constants.ONE_FEET) + (placementPose.right * -Constants.ONE_FEET);
        // bot right
        Vector3 cornerPos3 = placementPose.position
            + (placementPose.forward * -Constants.ONE_FEET) + (placementPose.right * Constants.ONE_FEET);
        phase1DotsMatrix = new Vector3[,] { { cornerPos2, cornerPos1 }, { cornerPos4, cornerPos3} };
        InstantiateDotsWithAnchor(dot, cornerPos1, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos2, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos3, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos4, placementPose.rotation);
    }

    public void ActivatePhase1Cube()
    {
        ClearDots();
        //arDrawManager.ClearLines();
        g2SoundManager.playFinishDrawingAudio();
        flatRectangle = Instantiate(flatRectangle, placementPose.position, placementPose.rotation);

        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE1Mid);
    }

    public void finishGame2Phase1()
    {
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE1End);
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
        //foreach (GameObject dot in dots)
        //{
        //    Destroy(dot);
        //}
        GameObject[] dotObjects = GameObject.FindGameObjectsWithTag("dot");
        foreach (GameObject dotObj in dotObjects)
        {
            Destroy(dotObj);
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
