using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game2Manager : Singleton<Game2Manager>
{
    [SerializeField]
    private GameObject phase0Object;
    [SerializeField]
    private GameObject phase1Object;
    private DotsManager dotsManager;
    private CharacterController characterController;
    private ARDrawManager arDrawManager;
    private Game2SoundManager g2SoundManager;

    public Vector3 phase1JumpStart;
    public Vector3 phase1JumpEnd;

    public Vector3 rampStartPoint;
    public Vector3 rampEndPoint;
    public float rampHeight;

    public Dictionary<string, Vector3> objectLocations = new Dictionary<string, Vector3>();

    private string gamePhase = "waiting";

    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        dotsManager = FindObjectOfType<DotsManager>();
        arDrawManager = FindObjectOfType<ARDrawManager>();
        characterController = FindObjectOfType<CharacterController>();
        g2SoundManager = FindObjectOfType<Game2SoundManager>();
    }

    private void Update()
    {
        switch (gamePhase)
        {
            case Constants.GamePhase.PHASE0:
                StartPhase0();
                gamePhase = Constants.GamePhase.WAITING;
                break;
            case Constants.GamePhase.PHASE1:
                StartPhase1();
                gamePhase = Constants.GamePhase.WAITING;
                break;
            case Constants.GamePhase.PHASE2:
                StartPhase2();
                gamePhase = Constants.GamePhase.WAITING;
                break;
            case Constants.GamePhase.PHASE3:
                StartPhase3();
                gamePhase = Constants.GamePhase.WAITING;
                break;
            default:
                break;
        }
    }

    public void SetGamePhase(string gamePhase)
    {
        this.gamePhase = gamePhase;
        arDrawManager.GamePhase = gamePhase;
    }

    #region phase0 related
    private void StartPhase0()
    {
        StartCoroutine(WaitBeforePhase0Dots(9.5f));
    }

    IEnumerator WaitBeforePhase0Dots(float duration)
    {
        yield return new WaitForSeconds(duration);
        dotsManager.InstantiatePhase0Dots();
    }

    /// <summary>
    /// Code to end phase 0;
    /// </summary>
    public void EndPhase0()
    {
        g2SoundManager.PlayGoodSoundEffect();

        ARDebugManager.Instance.LogInfo("Endphase0 is called");
        // replace points of dots with prefab
        Vector3 midPoint = new Vector3(0, 0, 0);
        foreach(GameObject dot in dotsManager.dots) {
            midPoint += dot.transform.position;
        }
        midPoint /= 2.0f;
        Debug.Log("first point: " + dotsManager.dots[0].transform.position);
        Debug.Log("second point: " + dotsManager.dots[1].transform.position);
        Debug.Log("mid point: " + midPoint);

        phase0Object = Instantiate(phase0Object, midPoint, dotsManager.dots[0].transform.rotation);

        objectLocations.Add(Constants.Objects.RailingStartPoint, dotsManager.dots[1].transform.position);
        objectLocations.Add(Constants.Objects.RailingEndPoint, dotsManager.dots[0].transform.position);
        dotsManager.ClearDots();

        // TODO: add animation & play sound effect
        //g2SoundManager.PlayGoodSoundEffect();

        StartCoroutine(PlayPhase0EndAnimationAndAudio());
    }

    IEnumerator PlayPhase0EndAnimationAndAudio()
    {
        yield return new WaitForSeconds(1.0f); // wait for good effect sound
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE0End); // play right on
        characterController.PlayTalkingAnimationWithDuration(2.0f);
        yield return new WaitForSeconds(2.0f);
        // animate towards railing
        characterController.SkateOnRailing(objectLocations[Constants.Objects.RailingStartPoint], objectLocations[Constants.Objects.RailingEndPoint]);
        g2SoundManager.PlaySkatingSoundForTime(10.5f);
        yield return new WaitForSeconds(10.5f);
        Destroy(phase0Object);
        SetGamePhase(Constants.GamePhase.PHASE1);
    }
    #endregion

    #region phase1 related
    public void EndPhase1()
    {
        g2SoundManager.PlayGoodSoundEffect();

        ARDebugManager.Instance.LogInfo("Endphase1 is called");

        StartCoroutine(PlayPhase1EndAnimationAndAudio());
    }

    IEnumerator PlayPhase1EndAnimationAndAudio()
    {
        yield return new WaitForSeconds(1.0f); // wait for good effect sound
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE1End); // play right on
        characterController.PlayTalkingAnimationWithDuration(6.8f);
        yield return new WaitForSeconds(6.8f);
        // animate towards jumping
        characterController.SkateOnRailing(phase1JumpStart, phase1JumpEnd);
        g2SoundManager.PlaySkatingSoundForTime(10.5f);
        yield return new WaitForSeconds(10.5f);
        var phase1Rect = GameObject.FindGameObjectWithTag("phase1Rect");
        Destroy(phase1Rect);
        arDrawManager.ClearLines();
        SetGamePhase(Constants.GamePhase.PHASE2);
    }

    private void StartPhase1()
    {
        dotsManager.InstantiatePhase1Dots();
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE1Start);
        characterController.PlayTalkingAnimationWithDuration(7f);
    }
    #endregion

    #region phase2 related
    private void StartPhase2()
    {
        dotsManager.InstantiatePhase2Dots();
        //g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE2Start);
        //characterController.PlayTalkingAnimationWithDuration(5.5f + 6.2f);
    }
    public void StartPhase2Mid()
    {
        dotsManager.ClearDots();
        //g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE2Mid);
        //characterController.PlayTalkingAnimationWithDuration(2.6f + 5.3f + 5.3f + 7.6f);
    }
    public void StartPhase2End()
    {
        dotsManager.ClearDots();
        //g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE2End);
        //characterController.PlayTalkingAnimationWithDuration(4.4f + 8.2f + 8f + 6.7f);
        StartCoroutine(Phase2EndingAnimation());
    }
    IEnumerator Phase2EndingAnimation()
    {
        //yield return new WaitForSeconds(4.4f + 8.2f + 8f + 6.7f + 1f);
        arDrawManager.ClearLines();
        characterController.SkateOnRamp(rampStartPoint, rampEndPoint, rampHeight);
        Debug.Log("startPoint: " + rampStartPoint);
        Debug.Log("rampEndPoint: " + rampEndPoint);
        Debug.Log("rampHeight: " + rampHeight);
        yield return new WaitForSeconds(10f);
        SetGamePhase(Constants.GamePhase.PHASE3);
        arDrawManager.DestoryRamp();
    }
    #endregion

    #region phase3 related
    private void StartPhase3()
    {
        // destroy things
        dotsManager.InstantiatePhase3Dots();
    }
    #endregion
}
