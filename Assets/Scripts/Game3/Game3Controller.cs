using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game3Controller : GenericClass
{
    private string gamePhase = Constants.GamePhase.SETUP;
    private PlacementIndicatorController placementController;
    public Pose placementPose;
    public GameObject slingshotObj;
    public GameObject jointLeftCenter;
    public GameObject jointRightCenter;

    public GameObject balloonObj;
    public GameObject axisObj;
    public GameObject door;

    public GameObject carnivalBooth;
    private CharacterController characterController;
    private GameObject numbers;
    private GameObject phase0Layout;
    private GameObject phase1Layout;
    private GameObject phase2Layout;
    [SerializeField]
    private Material initialMat;
    [SerializeField]
    private Material selectedMat;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        characterController = FindObjectOfType<CharacterController>();
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
                        Vector3 rot = placementPose.rotation.eulerAngles;
                        rot = new Vector3(rot.x, rot.y + 177, rot.z);
                        var newRot = Quaternion.Euler(rot);
                        carnivalBooth = Instantiate(carnivalBooth, placementPose.position + (placementPose.forward * 8), newRot);
                        carnivalBooth.name = "booth";

                        gamePhase = Constants.GamePhase.PHASE0;
                        phase0Layout = carnivalBooth.transform.Find("boothAndCannon/Phase0").gameObject;
                        phase1Layout = carnivalBooth.transform.Find("boothAndCannon/Phase1").gameObject;
                        phase2Layout = carnivalBooth.transform.Find("boothAndCannon/Phase2").gameObject;
                        // init slingshot
                        //slingshotObj.SetActive(true);
                        //balloonObj.SetActive(true);
                        //axisObj.SetActive(true);
                        characterController.InitCharacterGame3(placementPose, placementController.GetPlacementIndicatorLocation());
                    }   
                }
                break;
            case Constants.GamePhase.PHASE0:
                numbers = carnivalBooth.transform.Find("boothAndCannon/Phase0/numbers").gameObject;
                SetPhaseLayout(Constants.GamePhase.PHASE0);
                SetXMatPosition(0, 0);
                SetXCollider(true);
                SetYCollider(false);
                gamePhase = Constants.GamePhase.WAITING;
                break;
            case Constants.GamePhase.PHASE1:
                numbers = carnivalBooth.transform.Find("boothAndCannon/Phase1/numbers").gameObject;
                SetPhaseLayout(Constants.GamePhase.PHASE1);
                // reset materials
                ResetNumbers();
                SetXMatPosition(0, 0);
                SetXCollider(false);
                SetYCollider(true);
                gamePhase = Constants.GamePhase.WAITING;
                break;
            case Constants.GamePhase.PHASE2:
                numbers = carnivalBooth.transform.Find("boothAndCannon/Phase2/numbers").gameObject;
                SetPhaseLayout(Constants.GamePhase.PHASE2);
                ResetNumbers();
                SetXMatPosition(5, 0);
                SetXCollider(true);
                SetYCollider(true);
                gamePhase = Constants.GamePhase.WAITING;
                break;
        }
    }
    private void SetPhaseLayout(string phase)
    {
        switch(phase)
        {
            case Constants.GamePhase.PHASE0:
                phase0Layout.SetActive(true);
                phase1Layout.SetActive(false);
                phase2Layout.SetActive(false);
                break;
            case Constants.GamePhase.PHASE1:
                phase0Layout.SetActive(false);
                phase1Layout.SetActive(true);
                phase2Layout.SetActive(false);
                break;
            case Constants.GamePhase.PHASE2:
                phase0Layout.SetActive(false);
                phase1Layout.SetActive(false);
                phase2Layout.SetActive(true);
                break;
        }
    }
    private void SetXCollider(bool isActive)
    {
        carnivalBooth.transform.Find("boothAndCannon/cannon_GRP/cannon_base").GetComponent<BoxCollider>().enabled = isActive;
    }
    private void SetYCollider(bool isActive)
    {
        carnivalBooth.transform.Find("boothAndCannon/cannon_GRP/cannon_base/cannon").GetComponent<BoxCollider>().enabled = isActive;
    }

    private void ResetNumbers()
    {
        foreach (Transform child in numbers.transform)
        {
            child.gameObject.GetComponent<MeshRenderer>().material = initialMat;
        }
    }
    public void SetXMatPosition(int num, int prevNum)
    {
        numbers.transform.Find($"horizontal_{prevNum}").GetComponent<MeshRenderer>().material = initialMat;
        numbers.transform.Find($"horizontal_{num}").GetComponent<MeshRenderer>().material = selectedMat;
    }
    public void SetYMatPosition(int num, int prevNum)
    {
        numbers.transform.Find($"vertical_{prevNum}").GetComponent<MeshRenderer>().material = initialMat;
        numbers.transform.Find($"vertical_{num}").GetComponent<MeshRenderer>().material = selectedMat;
    }
}
