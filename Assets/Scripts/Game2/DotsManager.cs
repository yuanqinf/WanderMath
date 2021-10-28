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
    private CharacterController characterController;

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
        characterController = FindObjectOfType<CharacterController>();
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

                g2Manager.SetGamePhase(Constants.GamePhase.PHASE0);
                characterController.InitCharacterSkatingAndAudio(placementPose);
                InstantiateOthersWithAnchor(plane, placementPose.position - new Vector3(0, 0.001f, 0), placementPose.rotation);
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
        Vector3 cornerPos1 = placementPose.position + (placementPose.right * Constants.HALF_FEET);
        Vector3 cornerPos2 = placementPose.position + (placementPose.right * -Constants.HALF_FEET);
        InstantiateDotsWithAnchor(dot, cornerPos1, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos2, placementPose.rotation);
    }

    public void InstantiatePhase1Dots()
    {
        StartCoroutine(SetGamePhase1());
        //ActivatePhase1Cube();
    }

    IEnumerator SetGamePhase1()
    {
        yield return new WaitForSeconds(7f);
        // top left
        Vector3 cornerPos2 = placementPose.position
            + (placementPose.forward * Constants.HALF_FEET) + (placementPose.right * -Constants.HALF_FEET);
        // top right
        Vector3 cornerPos1 = placementPose.position
            + (placementPose.forward * Constants.HALF_FEET) + (placementPose.right * Constants.HALF_FEET);
        // bot left
        Vector3 cornerPos4 = placementPose.position
            + (placementPose.forward * -Constants.HALF_FEET) + (placementPose.right * -Constants.HALF_FEET);
        // bot right
        Vector3 cornerPos3 = placementPose.position
            + (placementPose.forward * -Constants.HALF_FEET) + (placementPose.right * Constants.HALF_FEET);
        phase1DotsMatrix = new Vector3[,] { { cornerPos2, cornerPos1 }, { cornerPos4, cornerPos3} };
        InstantiateDotsWithAnchor(dot, cornerPos1, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos2, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos3, placementPose.rotation);
        InstantiateDotsWithAnchor(dot, cornerPos4, placementPose.rotation);
    }

    public void ActivatePhase1Cube()
    {
        flatRectangle = Instantiate(flatRectangle, placementPose.position, placementPose.rotation);
        flatRectangle.GetComponent<ShapeAnimationHelper>().shapeDots[0] = dots[2].transform.position;
        flatRectangle.GetComponent<ShapeAnimationHelper>().shapeDots[1] = dots[3].transform.position;
        flatRectangle.GetComponent<ShapeAnimationHelper>().shapeDots[2] = dots[4].transform.position;
        flatRectangle.GetComponent<ShapeAnimationHelper>().shapeDots[3] = dots[5].transform.position;
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE1Mid);
        ClearDots();
        //arDrawManager.ClearLines();
        float finishDrawingAudioLen = g2SoundManager.playFinishDrawingAudio();

        StartCoroutine(SetGamePhase1Mid(finishDrawingAudioLen));
    }

    IEnumerator SetGamePhase1Mid(float lenToWait)
    {
        Debug.Log("setting game2 phase1 mid");
        yield return new WaitForSeconds(lenToWait);

        arDrawManager.ClearLines();
    }

    public void InstantiatePhase2Dots()
    {
        StartCoroutine(SetGamePhase2Dots());
    }
    IEnumerator SetGamePhase2Dots()
    {
        yield return new WaitForSeconds(5.5f);

        // top left
        Vector3 topLeft = placementPose.position
            + (placementPose.forward * Constants.HALF_FEET) + (placementPose.right * -Constants.HALF_FEET);
        InitializeDots(topLeft, 2, 3);
    }

    public void InstantiatePhase3Dots()
    {
        StartCoroutine(SetGamePhase3Dots());
    }
    IEnumerator SetGamePhase3Dots()
    {
        yield return new WaitForSeconds(0.1f);
        Vector3 topLeft = placementPose.position
            + (placementPose.forward * -3 * Constants.HALF_FEET) + (placementPose.right * 3 * -Constants.HALF_FEET);
        InitializeDots(topLeft, 4, 5);
    }

    private void InitializeDots(Vector3 topLeft, int rows, int cols)
    {
        for (int i = 0; i < rows; i++)
        {
            var newLeft = topLeft + new Vector3(0, 0, i * Constants.ONE_FEET);
            for (int j = 0; j < cols; j++)
            {
                var newPos = newLeft + new Vector3(j * Constants.ONE_FEET, 0, 0);
                InstantiateDotsWithAnchor(dot, newPos, dot.transform.rotation);
            }
        }
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
        Debug.Log("destory dot now!!!!!!!");
        GameObject[] dotObjects = GameObject.FindGameObjectsWithTag("dot");
        foreach (var dotObj in dotObjects)
        {
            if(dotObj != null)
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
