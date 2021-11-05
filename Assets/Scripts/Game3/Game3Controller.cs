using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game3Controller : GenericClass
{
    private string gamePhase = Constants.GamePhase.SETUP;
    private string currGamePhase = Constants.GamePhase.SETUP;
    private PlacementIndicatorController placementController;
    private CharacterController characterController;
    private CannonControl cannonController;
    private Game3SoundManager game3SoundManager;
    public Pose placementPose;
    public GameObject slingshotObj;
    public GameObject jointLeftCenter;
    public GameObject jointRightCenter;

    public GameObject balloonObj;
    public GameObject axisObj;
    public GameObject door;

    public GameObject carnivalBooth;
    private GameObject numbers;
    private GameObject phase0Layout;
    private GameObject phase1Layout;
    private GameObject phase2Layout;
    private GameObject phase3Layout;
    private int targetHit;
    [SerializeField]
    private Material initialMat;
    [SerializeField]
    private Material selectedMat;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        characterController = FindObjectOfType<CharacterController>();
        placementController = FindObjectOfType<PlacementIndicatorController>();
        game3SoundManager = FindObjectOfType<Game3SoundManager>();
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

                        SetGamePhase(Constants.GamePhase.PHASE0);
                        cannonController = FindObjectOfType<CannonControl>();
                        phase0Layout = carnivalBooth.transform.Find("boothAndCannon/Phase0").gameObject;
                        phase1Layout = carnivalBooth.transform.Find("boothAndCannon/Phase1").gameObject;
                        phase2Layout = carnivalBooth.transform.Find("boothAndCannon/Phase2").gameObject;
                        phase3Layout = carnivalBooth.transform.Find("boothAndCannon/Phase3").gameObject;
                        characterController.InitCharacterGame3(placementPose, placementController.GetPlacementIndicatorLocation());
                    }   
                }
                break;
            case Constants.GamePhase.PHASE0:
                game3SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE0Start);
                characterController.PlayTalkingAnimationWithDuration(6.5f + 6.2f + 9.3f + 3.2f);
                numbers = carnivalBooth.transform.Find("boothAndCannon/Phase0/numbers").gameObject;
                SetPhaseLayout(Constants.GamePhase.PHASE0);
                SetXMatPosition(0, 0);
                SetXCollider(true);
                SetYCollider(false);
                targetHit = 0;
                gamePhase = Constants.GamePhase.WAITING;
                break;
            case Constants.GamePhase.PHASE1:
                game3SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE1Start);
                characterController.PlayTalkingAnimationWithDuration(6.4f + 4.6f);
                numbers = carnivalBooth.transform.Find("boothAndCannon/Phase1/numbers").gameObject;
                SetPhaseLayout(Constants.GamePhase.PHASE1);
                // reset materials
                ResetNumbersMat();
                cannonController.ResetCannonPosition();
                SetYMatPosition(0, 0);
                SetXCollider(false);
                SetYCollider(true);
                targetHit = 0;
                gamePhase = Constants.GamePhase.WAITING;
                break;
            case Constants.GamePhase.PHASE2:
                game3SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE2Start);
                characterController.PlayTalkingAnimationWithDuration(6.1f + 3.6f + 5.0f + 5.9f);
                numbers = carnivalBooth.transform.Find("boothAndCannon/Phase2/numbers").gameObject;
                SetPhaseLayout(Constants.GamePhase.PHASE2);
                ResetNumbersMat();
                cannonController.ResetCannonPosition();
                SetXMatPosition(0, 0);
                SetXCollider(true);
                SetYCollider(true);
                targetHit = 0;
                gamePhase = Constants.GamePhase.WAITING;
                break;
            case Constants.GamePhase.PHASE3:
                game3SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE3Start);
                characterController.PlayTalkingAnimationWithDuration(7.6f + 7.9f + 2.6f);
                numbers = carnivalBooth.transform.Find("boothAndCannon/Phase3/numbers").gameObject;
                SetPhaseLayout(Constants.GamePhase.PHASE3);
                ResetNumbersMat();
                cannonController.ResetCannonPosition();
                SetXMatPosition(0, 0);
                SetXCollider(true);
                SetYCollider(true);
                targetHit = 0;
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
                phase3Layout.SetActive(false);
                break;
            case Constants.GamePhase.PHASE1:
                phase0Layout.SetActive(false);
                phase1Layout.SetActive(true);
                phase2Layout.SetActive(false);
                phase3Layout.SetActive(false);
                break;
            case Constants.GamePhase.PHASE2:
                phase0Layout.SetActive(false);
                phase1Layout.SetActive(false);
                phase2Layout.SetActive(true);
                phase3Layout.SetActive(false);
                break;
            case Constants.GamePhase.PHASE3:
                phase0Layout.SetActive(false);
                phase1Layout.SetActive(false);
                phase2Layout.SetActive(false);
                phase3Layout.SetActive(true);
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

    private void ResetNumbersMat()
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

    private void StartPhase0End()
    {
        StartCoroutine(PlayPhaseEnding(Constants.GamePhase.PHASE1, Constants.VoiceOvers.PHASE0End, 2.1f));
    }
    private void StartPhase1End()
    {
        StartCoroutine(PlayPhaseEnding(Constants.GamePhase.PHASE2, Constants.VoiceOvers.PHASE1End, 3.3f));
    }
    private void StartPhase2End()
    {
        StartCoroutine(PlayPhaseEnding(Constants.GamePhase.PHASE3, Constants.VoiceOvers.PHASE2End, 2.1f));
    }
    private void StartPhase3End()
    {
        StartCoroutine(PlayPhaseEnding(Constants.GamePhase.ENDING, Constants.VoiceOvers.PHASE3End, 6.0f + 4.1f));
    }
    IEnumerator PlayPhaseEnding(string phase, string voiceover, float duration)
    {
        game3SoundManager.PlayVoiceovers(voiceover);
        characterController.PlayTalkingAnimationWithDuration(duration);
        yield return new WaitForSeconds(duration);
        SetGamePhase(phase);
    }

    public void IncreaseTargetHit()
    {
        this.targetHit++;
        if (currGamePhase == Constants.GamePhase.PHASE0 && targetHit == 2)
        {
            StartPhase0End();
        }
        else if (currGamePhase == Constants.GamePhase.PHASE1 && targetHit == 2)
        {
            StartPhase1End();
        }
        else if (currGamePhase == Constants.GamePhase.PHASE2 && targetHit == 5)
        {
            StartPhase2End();
        }
        else if (currGamePhase == Constants.GamePhase.PHASE3 && targetHit == 5)
        {
            StartPhase3End();
        }
    }
    private void SetGamePhase(string gamePhase)
    {
        this.gamePhase = gamePhase;
        this.currGamePhase = gamePhase;
    }
}
