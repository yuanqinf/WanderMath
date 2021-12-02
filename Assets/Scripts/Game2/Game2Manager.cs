using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game2Manager : Singleton<Game2Manager>
{
    [SerializeField]
    private GameObject phase0Object;
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

    public string lastGamePhase = "";

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
                lastGamePhase = "phase0";

                break;
            case Constants.GamePhase.PHASE1:
                StartPhase1();
                gamePhase = Constants.GamePhase.WAITING;
                lastGamePhase = "phase1";
                break;
            case Constants.GamePhase.PHASE2:
                StartPhase2();
                gamePhase = Constants.GamePhase.WAITING;
                lastGamePhase = "phase2";
                break;
            case Constants.GamePhase.PHASE3:
                StartPhase3();
                gamePhase = Constants.GamePhase.WAITING;
                lastGamePhase = "phase3";
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
        var preJumpPos = phase0Object.transform.Find("preJumpPoint").position;
        characterController.SkateOnRailing(objectLocations[Constants.Objects.RailingStartPoint], objectLocations[Constants.Objects.RailingEndPoint], preJumpPos);
        g2SoundManager.PlaySkatingSoundForTime(10.5f);
        yield return new WaitForSeconds(10.5f);
        Destroy(phase0Object);
        SetGamePhase(Constants.GamePhase.PHASE1);
    }
    #endregion

    #region phase1 related
    private void StartPhase1()
    {
        dotsManager.InstantiatePhase1Dots();
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE1Start);
        characterController.PlayTalkingAnimationWithDuration(7f);
    }

    public void StartPhase1Mid(GameObject concreteUIDisplay)
    {
        StartCoroutine(SetGamePhase1Mid(concreteUIDisplay));
    }
    IEnumerator SetGamePhase1Mid(GameObject concreteUIDisplay)
    {
        Debug.Log("setting game2 phase1 mid");
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE1Mid);
        characterController.PlayTalkingAnimationWithDuration(7.5f + 3.8f + 5.0f);
        yield return new WaitForSeconds(7.5f);
        var uiNumberControl = GameObject.FindGameObjectWithTag("phase1Rect").GetComponent<UINumberControl>();
        uiNumberControl.SetVolDisplay(0);
        arDrawManager.ClearLines();
        arDrawManager.concreteUIFill.fillAmount = 0;
        concreteUIDisplay.SetActive(true);
        concreteUIDisplay.transform.Find("VolUI").GetComponent<TextMeshProUGUI>().text = "Vol: 0 ft<sup>3</sup>";
    }
    public void EndPhase1()
    {
        g2SoundManager.PlayGoodSoundEffect();
        StartCoroutine(PlayPhase1EndAnimationAndAudio());
    }
    IEnumerator PlayPhase1EndAnimationAndAudio()
    {
        yield return new WaitForSeconds(1.0f); // wait for good effect sound
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE1End); // play right on
        characterController.PlayTalkingAnimationWithDuration(6.8f + 1.6f);
        yield return new WaitForSeconds(6.8f + 1.6f);
        // animate towards jumping
        var flatRectangle = GameObject.FindGameObjectWithTag("phase1Rect");
        var startPos = flatRectangle.transform.Find("point1").transform.position;
        var preJumpPos = flatRectangle.transform.Find("point2").transform.position;
        var jumpPos = flatRectangle.transform.Find("point3").transform.position;
        var landPos = flatRectangle.transform.Find("point4").transform.position;
        characterController.SkateOnCubeNew(startPos, preJumpPos, jumpPos, landPos);
        g2SoundManager.PlaySkatingSoundForTime(10.5f);
        yield return new WaitForSeconds(10.5f);
        g2SoundManager.PlayPhase1Perfect();
        characterController.PlayTalkingAnimationWithDuration(3.0f);
        yield return new WaitForSeconds(3.0f);
        var phase1Rect = GameObject.FindGameObjectWithTag("phase1Rect");
        Destroy(phase1Rect);
        arDrawManager.ClearLines();
        SetGamePhase(Constants.GamePhase.PHASE2);
    }
    #endregion

    #region phase2 related
    private void StartPhase2()
    {
        dotsManager.InstantiatePhase2Dots();
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE2Start);
        characterController.PlayTalkingAnimationWithDuration(5.5f + 6.2f);
    }
    public void StartPhase2Mid(int maxLength)
    {
        dotsManager.ClearDots();
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE2Mid);
        characterController.PlayTalkingAnimationWithDuration(2.6f + 5.3f + 5.3f + 7.6f);
        StartCoroutine(DeletePhase2Lines(maxLength));
    }

    IEnumerator DeletePhase2Lines(int maxLength)
    {
        yield return new WaitForSeconds(2.6f); // 2.6f - rectangle, 5.3f - length * width line
        arDrawManager.ClearLines();
        var uiNumberControl = GameObject.FindGameObjectWithTag("ramp").GetComponent<UINumberControl>();
        // activate length & width
        uiNumberControl.SetLengthDisplay($"Length: {maxLength} ft");
        uiNumberControl.SetWidthDisplay("Width: 1 ft");
        yield return new WaitForSeconds(5.3f + 7.6f);
        // turn off length & width
        uiNumberControl.SetLengthDisplay("");
        uiNumberControl.SetWidthDisplay("");
        // change text to volume
        uiNumberControl.SetVolDisplay(0);
        // activate concrete text
        arDrawManager.concreteUIDisplay.SetActive(true);
        arDrawManager.concreteVolDisplay.text = "Vol: 0 ft<sup>3</sup>";
        arDrawManager.concreteUIFill.fillAmount = 0;
        // enable collider
        arDrawManager.SetRampEdgeCollider(true);
    }

    public void StartPhase2EndNew(List<Vector3> animePts, string lowSlopeLoc)
    {
        dotsManager.ClearDots();
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE2End);
        characterController.PlayTalkingAnimationWithDuration(4.4f + 8.2f + 8f + 1.7f);
        StartCoroutine(Phase2EndingAnimationNew(animePts, lowSlopeLoc));
    }

    private IEnumerator Phase2EndingAnimationNew(List<Vector3> animePts, string lowSlopeLoc)
    {
        yield return new WaitForSeconds(4.4f + 8.2f + 8f + 1.7f + 1f);
        arDrawManager.ClearLines();
        characterController.SkateOnRampNew(animePts, lowSlopeLoc);
        var skatingTime = (animePts.Count + 1) * 2.0f;
        g2SoundManager.PlaySkatingSoundForTime(skatingTime);
        yield return new WaitForSeconds(skatingTime);
        // play best ramp ever line after animation
        g2SoundManager.PlayBestRampEver();
        characterController.PlayTalkingAnimationWithDuration(3.9f);
        yield return new WaitForSeconds(4f);
        SetGamePhase(Constants.GamePhase.PHASE3);
        arDrawManager.DestoryRampAndReferences();
    }
    #endregion

    #region phase3 related
    private void StartPhase3()
    {
        dotsManager.InstantiatePhase3Dots();
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE3Start);
        characterController.PlayTalkingAnimationWithDuration(4.6f + 3.5f + 4.9f + 8f);
    }

    public void StartPhase3End(List<AnimationPoint> animationPoints)
    {
        StartCoroutine(Phase3EndingAnimation(animationPoints));
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE3End);
        characterController.PlayTalkingAnimationWithDuration(3.3f);
    }
    IEnumerator Phase3EndingAnimation(List<AnimationPoint> animationPoints)
    {
        yield return new WaitForSeconds(3.3f);
        for (int i = 0; i < animationPoints.Count; i++)
        {
            var point = animationPoints[i];
            if (i == 0)
            {
                characterController.StartSkating();
                yield return new WaitForSeconds(1.4f);
            }
            if (point.isJump)
            {
                //Debug.Log($"play jumping animation. {point.height}");
                var points = point.animPts;
                characterController.SkateOnCubeNew(points[0], points[1], points[2], points[3], true);
                g2SoundManager.PlaySkatingSoundForTime(6.0f);
                yield return new WaitForSeconds(6.0f);
            }
            else
            {
                //Debug.Log($"play ramp animation: {point.height}");
                characterController.SkateOnRampNew(point.animPts, point.rampLoc, true);
                var skatingTime = (point.animPts.Count + 1) * 2.0f;
                g2SoundManager.PlaySkatingSoundForTime(skatingTime);
                yield return new WaitForSeconds(skatingTime);
            }
            if (i == animationPoints.Count - 1)
            {
                characterController.StopSkating();
                yield return new WaitForSeconds(2.7f);
            }
        }
        yield return new WaitForSeconds(0.5f);
        g2SoundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE3Additional);
        characterController.PlayTalkingAnimationWithDuration(3.8f + 6.5f);
        yield return new WaitForSeconds(3.8f + 6.5f + 2.0f);
        SceneManager.LoadScene(Constants.Scenes.MainMenu);
    }

    #endregion

    #region helper functions
    public void PlayWrongDiagonalWithAnimation()
    {
        characterController.PlayTalkingAnimationWithDuration(5.5f);
        g2SoundManager.PlayWrongDiagonal();
    }
    public void PlayWrongLinesWithAnimation()
    {
        characterController.PlayTalkingAnimationWithDuration(4.2f);
        g2SoundManager.PlayWrongLines();
    }
    public void PlayRepeatedLinesWithAnimation()
    {
        characterController.PlayTalkingAnimationWithDuration(2.5f);
        g2SoundManager.PlayRepeatedLines();
    }
    #endregion

    public void resetPhase()
    {
        if(lastGamePhase == "phase1")
        {
            DestroyImmediate(GameObject.FindGameObjectWithTag("phase1Rect"), true);
        }

        if(lastGamePhase == "phase2")
        {
            ARDrawManager.Instance.DestoryRampAndReferences();
        }

        if(lastGamePhase == "phase3")
        {
            foreach (GameObject ramp in GameObject.FindGameObjectsWithTag("ramp"))
            {
                DestroyImmediate(ramp, true);
            }
            ARDrawManager.Instance.ClearRampRefereces();
        }

        foreach (GameObject dot in GameObject.FindGameObjectsWithTag("dot"))
        {
            DestroyImmediate(dot, true);
        }

        ARDrawManager.Instance.ResetEdgeListsAndDict();

        ARDrawManager.Instance.ClearLines();
        dotsManager.dots.Clear();

        ARDrawManager.Instance.concreteUIFill.fillAmount = 0;
        ARDrawManager.Instance.concreteUIDisplay.SetActive(false);

        SetGamePhase(lastGamePhase);
    }
}
