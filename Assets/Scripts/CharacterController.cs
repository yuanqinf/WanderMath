using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CharacterController : GenericClass
{
    [SerializeField]
    private GameObject arCharacterToSpawn;
    public Animation skatingAnimation;
    private Animator animator;
    private float stopSkatingAnimationLen = 2.7f;
    private float skateJumpAnimationLen = 1.3f;
    private float idleToSkateAnimationLen = 1.4f;
    private bool isTalking = false;

    private string introLine = "Oh, hi! I'm Finley. Nice to meet you!";

    public float InitCharacterAndAudioGame1(Pose placementPose, Transform placementPos)
    {
        // to be placed at the corner
        Debug.Log("placement Pose: " + placementPose.rotation);

        Vector3 rot = placementPose.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);

        Vector3 characterPos = placementPose.position
            + (placementPos.forward * 0.4f) + (-placementPos.right * 0.2f);

        Quaternion characterRot = Quaternion.Euler(rot);

        arCharacterToSpawn = Instantiate(
            arCharacterToSpawn, characterPos, characterRot
        );
        arCharacterToSpawn.transform.localScale *= 0.6f;
        animator = arCharacterToSpawn.GetComponent<Animator>();
        return StartFirstLine();
    }

    public void InitCharacterGame3(GameObject carnivalBooth)
    {
        arCharacterToSpawn = carnivalBooth.transform.Find("Finley").gameObject;
        animator = arCharacterToSpawn.GetComponent<Animator>();
    }

    /// <summary>
    /// Finley initialization for activity2.
    /// </summary>
    /// <param name="placementPose"></param>
    /// <returns></returns>
    public float InitCharacterSkatingAndAudioGame2(Pose placementPose)
    {
        Vector3 rot = placementPose.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);

        Vector3 characterPos = placementPose.position
            + (placementPose.forward * 1.1f) + (-placementPose.right * 0.4f);
        Debug.Log(characterPos);
        arCharacterToSpawn = Instantiate(
            arCharacterToSpawn, characterPos, Quaternion.Euler(rot)
        );
        // rescale finley to be smaller
        arCharacterToSpawn.transform.localScale *= 0.5f;
        animator = arCharacterToSpawn.GetComponent<Animator>();
        Vector3 endPos = characterPos - placementPose.forward;
        // audio & animation
        var phase0AudioLen = 6.0f;
        StartCoroutine(utils.LerpMovement(characterPos, endPos, phase0AudioLen - stopSkatingAnimationLen, arCharacterToSpawn));
        PlaySkatingForward(phase0AudioLen);
        g2soundManager.PlayVoiceovers(Constants.VoiceOvers.PHASE0Start);
        var talkingDuration = 7.0f;
        StartCoroutine(WaitBeforeTalking(phase0AudioLen, talkingDuration));

        return phase0AudioLen + 3.5f;
    }

    IEnumerator WaitBeforeTalking(float waitDuration, float talkingDuration)
    {
        yield return new WaitForSeconds(waitDuration);
        PlayTalkingAnimationWithDuration(talkingDuration);
    }

    public void SkateOnCubeNew(Vector3 startPos, Vector3 preJumpPos, Vector3 jumpPos, Vector3 landPos, bool isPhase3 = false)
    {
        StartCoroutine(SkatingOnCubeMoveNew(startPos, preJumpPos, jumpPos, landPos, isPhase3));
    }

    IEnumerator SkatingOnCubeMoveNew(Vector3 startPos, Vector3 preJumpPos, Vector3 jumpPos, Vector3 landPos, bool isPhase3)
    {
        var initialPos = arCharacterToSpawn.transform.position;
        if (!isPhase3)
        {
            StartSkating();
            yield return new WaitForSeconds(idleToSkateAnimationLen);
        }
        // move to start position
        //yield return new WaitForSeconds(0.25f); // Important to add a delay to calculate the distance after the spinning is complete

        // skate to the front
        Debug.Log($"startPos: {startPos.ToString("F4")}");
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(startPos);
        var skateToRailingDuration = 1.5f;
        StartCoroutine(utils.LerpMovement(initialPos, startPos, skateToRailingDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateToRailingDuration);
        // skate to pre jump position
        Debug.Log($"preJumpPos: {preJumpPos.ToString("F4")}");
        var skateToPreJumpPosDuration = 1.5f;
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(preJumpPos);
        StartCoroutine(utils.LerpMovement(startPos, preJumpPos, skateToPreJumpPosDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateToPreJumpPosDuration);

        // jump up
        SkateJump();
        Debug.Log($"jumpPos: {jumpPos.ToString("F4")}");
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(landPos);
        StartCoroutine(utils.LerpMovement(preJumpPos, jumpPos, skateJumpAnimationLen / 2, arCharacterToSpawn));
        yield return new WaitForSeconds(skateJumpAnimationLen / 2);
        Debug.Log($"landPos: {landPos.ToString("F4")}");
        StartCoroutine(utils.LerpMovement(jumpPos, landPos, skateJumpAnimationLen / 2, arCharacterToSpawn));
        yield return new WaitForSeconds(skateJumpAnimationLen / 2);

        // skate back to position
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(initialPos);
        StartCoroutine(utils.LerpMovement(landPos, initialPos, skateToRailingDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateToRailingDuration);
        // stop skating
        if (!isPhase3)
        {
            StopSkating();
        }
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().StopSkating();
    }

    public void SkateOnRailing(Vector3 pointToJumpUp, Vector3 pointToJumpDown, Vector3 preJumpPos)
    {
        StartCoroutine(SkatingOnRailingMove(pointToJumpUp, pointToJumpDown, preJumpPos));
    }
    IEnumerator SkatingOnRailingMove(Vector3 pointToJumpUp, Vector3 pointToJumpDown, Vector3 preJumpPos)
    {
        var initialPos = arCharacterToSpawn.transform.position;
        var railingHeight = new Vector3(0, 0.11f, 0);
        var offset = new Vector3(0, 0, 0.25f);
        // 1. start skating & wait for idle to skate
        StartSkating();
        yield return new WaitForSeconds(idleToSkateAnimationLen);
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(preJumpPos);
        // move to start position
        //yield return new WaitForSeconds(0.25f); // Important to add a delay to calculate the distance after the spinning is complete
        var skateToRailingDuration = 1.5f;
        //var skateboardPosTemp = arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().GetSkateBoardPos() - arCharacterToSpawn.transform.position;
        //var skateboardPosDiff = new Vector3(skateboardPosTemp.x, 0, skateboardPosTemp.z);
        StartCoroutine(utils.LerpMovement(initialPos, preJumpPos, skateToRailingDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateToRailingDuration);
        // jump up
        SkateJump();
        StartCoroutine(utils.LerpMovement(preJumpPos, pointToJumpUp + railingHeight, skateJumpAnimationLen, arCharacterToSpawn));
        yield return new WaitForSeconds(skateJumpAnimationLen);
        // skate on railing
        var skateOnRailingDuration = 2.5f;
        StartCoroutine(utils.LerpMovement(pointToJumpUp + railingHeight, pointToJumpDown + railingHeight, skateOnRailingDuration, arCharacterToSpawn));
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(pointToJumpDown + railingHeight);
        yield return new WaitForSeconds(skateOnRailingDuration);
        // jump down
        SkateJump();
        StartCoroutine(utils.LerpMovement(pointToJumpDown + railingHeight, pointToJumpDown + offset, skateJumpAnimationLen, arCharacterToSpawn));
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(initialPos);
        yield return new WaitForSeconds(skateJumpAnimationLen);

        // skate back to position
        StartCoroutine(utils.LerpMovement(pointToJumpDown + offset, initialPos, skateToRailingDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateToRailingDuration);
        // stop skating
        StopSkating();
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().StopSkating();
    }

    public void SkateOnRampNew(List<Vector3> animePts, string lowSlopeLoc, bool isPhase3 = false)
    {
        StartCoroutine(SkatingOnRampNew(animePts, lowSlopeLoc, isPhase3));
    }

    IEnumerator SkatingOnRampNew(List<Vector3> animePts, string lowSlopeLoc, bool isPhase3)
    {
        var initialPos = arCharacterToSpawn.transform.position;
        // 1. start skating to ramp starting
        if (!isPhase3)
        {
            StartSkating();
            yield return new WaitForSeconds(idleToSkateAnimationLen);
        }
        var skateToRampDuration = 2.0f;

        int i = 0;
        // skate to multiple points, skate up and complete
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(animePts[i]);
        StartCoroutine(utils.LerpMovement(initialPos, animePts[i], skateToRampDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateToRampDuration);
        for (i = 1; i < animePts.Count; i++)
        {
            arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(animePts[i]);
            if (lowSlopeLoc.Equals("right") && i == 4)
            {
                SkateJump();
            }
            else if (!lowSlopeLoc.Equals("right") && i == 3)
            {
                SkateJump();
            }
            StartCoroutine(utils.LerpMovement(animePts[i - 1], animePts[i], skateToRampDuration, arCharacterToSpawn));
            yield return new WaitForSeconds(skateToRampDuration);
        }
        // 4. skate back to initialPos
        StartCoroutine(utils.LerpMovement(animePts[i - 1], initialPos, skateToRampDuration, arCharacterToSpawn));
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(initialPos);
        yield return new WaitForSeconds(skateToRampDuration);

        if (!isPhase3)
        {
            StopSkating();
        }
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().StopSkating();
    }

    #region activity3 animations
    public void PlayShakeWater()
    {
        animator.SetTrigger("isShakingWater");
    }

    #endregion

    #region skating animation
    public void PlaySkatingForward(float duration)
    {
        StartCoroutine(SkatingDuration(duration));
    }
    IEnumerator SkatingDuration(float duration)
    {
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(Camera.main.transform.position);
        StartSkating();
        yield return new WaitForSeconds(duration - stopSkatingAnimationLen);
        StopSkating();
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().StopSkating();

        yield return new WaitForSeconds(stopSkatingAnimationLen);
    }

    public void StartSkating()
    {
        animator.SetBool("isSkating", true);
    }
    public void StopSkating()
    {
        animator.SetBool("isSkating", false);
    }
    public void SkateJump()
    {
        animator.SetTrigger("jump");
    }
    #endregion

    private float StartFirstLine()
    {
        var duration = soundManager.PlayCharacterInitClip();
        PlayTalkingAnimationWithDuration(duration);
        uiController.PlaySubtitles(introLine, duration);
        return duration;
    }

    public Vector3 GetArCharacterPosition()
    {
        return arCharacterToSpawn.transform.position;
    }

    #region talking
    public void PlayTalkingAnimationWithDuration(float duration)
    {
        if (!isTalking)
        {
            StartCoroutine(PlayTalkingAnimationWithinDuration(duration));
            isTalking = true;
        }
    }

    IEnumerator PlayTalkingAnimationWithinDuration(float duration)
    {
        animator.SetBool("isTalking", true);
        yield return new WaitForSeconds(duration);
        animator.SetBool("isTalking", false);
        isTalking = false;
    }
    #endregion

    #region skating
    public void PlaySkatingWithGiftsAnimationWithDuration(float duration, Vector3 endingPos)
    {
        StartCoroutine(PlaySkatingWithGiftsAnimationWithinDuration(duration, endingPos));
    }

    IEnumerator PlaySkatingWithGiftsAnimationWithinDuration(float duration, Vector3 endingPos)
    {
        animator.SetBool("isSkatingWithGifts", true);
        yield return new WaitForSeconds(idleToSkateAnimationLen);
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(Camera.main.transform.position);
        yield return new WaitForSeconds(1.5f);
        arCharacterToSpawn.GetComponent<FinleyAction>().G3Stuffs.SetActive(true);
        StartCoroutine(utils.LerpMovement(arCharacterToSpawn.transform.position, endingPos, duration, arCharacterToSpawn));
        yield return new WaitForSeconds(duration);
    }
    #endregion

    public void PlaySmallWin()
    {
        animator.SetTrigger("isSmallWin");
    }

    public void PlayBigWin()
    {
        animator.SetTrigger("isBigWin");
    }
}
