using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game3Controller : GenericClass
{
    private string gamePhase = "setup";
    private PlacementIndicatorController placementController;
    public Pose placementPose;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        placementController = FindObjectOfType<PlacementIndicatorController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gamePhase)
        {
            // setting up stage
            case "setup":
                if (!placementController.GetIsLayoutPlaced())
                {
                    placementPose = placementController.UpdatePlacementAndPose(Camera.main, placementPose);
                    if (placementController.GetIsPlacementPoseValid() && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        //var audioDuration = PlaceObjectAndAudio();
                        //SetGamePhaseWithDelay("phase0", audioDuration);
                        // TODO: change this back to phase0
                    }
                }
                break;
        }
    }
}
