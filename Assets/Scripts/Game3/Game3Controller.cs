using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game3Controller : GenericClass
{
    private string gamePhase = Constants.GamePhase.SETUP;
    private PlacementIndicatorController placementController;
    public Pose placementPose;
    public GameObject balloonObj;
    public GameObject axisObj;
    public GameObject door;

    private CharacterController characterController;


    void Start()
    {
        characterController = FindObjectOfType<CharacterController>();
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        placementController = FindObjectOfType<PlacementIndicatorController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gamePhase)
        {
            // setting up stage
            case Constants.GamePhase.SETUP:
                if (!placementController.GetIsLayoutPlaced())
                {
                    placementPose = placementController.UpdatePlacementAndPose(Camera.main, placementPose);
                    if (placementController.GetIsPlacementPoseValid() && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        //var audioDuration = PlaceObjectAndAudio();
                        //SetGamePhaseWithDelay("phase0", audioDuration);
                        // TODO: change this back to phase0
                        placementController.TurnOffPlacementAndText();
                        Instantiate(door, placementPose.position + (placementPose.forward * 1), placementPose.rotation);
                        gamePhase = Constants.GamePhase.PHASE0;
                    }   
                }
                break;
            case Constants.GamePhase.PHASE0:
                balloonObj.SetActive(true);
                axisObj.SetActive(true);
                gamePhase = Constants.GamePhase.WAITING;
                break;

        }
    }
}
