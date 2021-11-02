using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : GenericClass
{
    [SerializeField]
    private GameObject arCharacterToSpawn;
    public Animation skatingAnimation;
    private Animator animator;
    private float stopSkatingAnimationLen = 2.7f;
    private float skateJumpAnimationLen = 1.3f;
    private float idleToSkateAnimationLen = 1.4f;

    private string introLine = "Oh, hi! I'm Finley. Nice to meet you!";

    public float InitCharacterAndAudio(Pose placementPose, Transform placementPos)
    {
        // to be placed at the corner
        Debug.Log("placement Pose: " + placementPose.rotation);

        Vector3 rot = placementPose.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);

        Vector3 characterPos = placementPose.position
            + (placementPos.forward * 0.4f) + (-placementPos.right * 0.4f);

        Quaternion characterRot = Quaternion.Euler(rot);

        arCharacterToSpawn = Instantiate(
            arCharacterToSpawn, characterPos, characterRot
        );
        animator = arCharacterToSpawn.GetComponent<Animator>();

        return StartFirstLine();
    }

    public float InitCharacterSkatingAndAudio(Pose placementPose)
    {
        Vector3 rot = placementPose.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);

        Vector3 characterPos = placementPose.position
            + (placementPose.forward * 2.5f) + (-placementPose.right * 0.4f);
        Debug.Log(characterPos);
        arCharacterToSpawn = Instantiate(
            arCharacterToSpawn, characterPos, Quaternion.Euler(rot)
        );
        animator = arCharacterToSpawn.GetComponent<Animator>();
        Vector3 endPos = characterPos - placementPose.forward * 1.5f;
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

    public void SkateOnCube(Vector3 pointToJumpUp, Vector3 pointToJumpDown, float height = 0.4f, bool isPhase3 = false)
    {
        StartCoroutine(SkatingOnCubeMove(pointToJumpUp, pointToJumpDown, height, isPhase3));
    }

    IEnumerator SkatingOnCubeMove(Vector3 pointToJumpUp, Vector3 pointToJumpDown, float height, bool isPhase3)
    {
        var initialPos = arCharacterToSpawn.transform.position;
        // 1. start skating & wait for idle to skate
        if (!isPhase3)
        {
            StartSkating();
            yield return new WaitForSeconds(idleToSkateAnimationLen);
        }
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(Camera.main.transform.position);
        // move to start position
        //yield return new WaitForSeconds(0.25f); // Important to add a delay to calculate the distance after the spinning is complete
        var skateToRailingDuration = 2.0f;

        pointToJumpUp += new Vector3(0.15f, 0, 0);
        pointToJumpDown -= new Vector3(0.15f, 0, 0);

        StartCoroutine(utils.LerpMovement(initialPos, pointToJumpUp, skateToRailingDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateToRailingDuration);
        // mid jump point

        Vector3 midPointOfTwoPos = new Vector3();
        midPointOfTwoPos.x = pointToJumpUp.x + (pointToJumpDown.x - pointToJumpUp.x) / 2;
        midPointOfTwoPos.y = pointToJumpUp.y + (pointToJumpDown.y - pointToJumpUp.y) / 2 + height;
        midPointOfTwoPos.z = pointToJumpUp.z + (pointToJumpDown.z - pointToJumpUp.z) / 2;
        Debug.Log($"jump up point: {midPointOfTwoPos.y} and height: {height}");

        // jump up
        SkateJump();
        StartCoroutine(utils.LerpMovement(pointToJumpUp, midPointOfTwoPos, skateJumpAnimationLen / 2, arCharacterToSpawn));
        yield return new WaitForSeconds(skateJumpAnimationLen / 2);
        StartCoroutine(utils.LerpMovement(midPointOfTwoPos, pointToJumpDown, skateJumpAnimationLen / 2, arCharacterToSpawn));
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(initialPos);
        yield return new WaitForSeconds(skateJumpAnimationLen / 2);

        // skate back to position
        StartCoroutine(utils.LerpMovement(pointToJumpDown, initialPos, skateToRailingDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateToRailingDuration);
        // stop skating
        if (!isPhase3)
        {
            StopSkating();
        }
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().StopSkating();
    }

    public void SkateOnRailing(Vector3 pointToJumpUp, Vector3 pointToJumpDown)
    {
        StartCoroutine(SkatingOnRailingMove(pointToJumpUp, pointToJumpDown));
    }
    IEnumerator SkatingOnRailingMove(Vector3 pointToJumpUp, Vector3 pointToJumpDown)
    {
        var initialPos = arCharacterToSpawn.transform.position;
        var railingHeight = new Vector3(0, 0.1f, 0);
        // 1. start skating & wait for idle to skate
        StartSkating();
        yield return new WaitForSeconds(idleToSkateAnimationLen);
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(Camera.main.transform.position);
        // move to start position
        //yield return new WaitForSeconds(0.25f); // Important to add a delay to calculate the distance after the spinning is complete
        var skateToRailingDuration = 2.0f;
        //var skateboardPosTemp = arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().GetSkateBoardPos() - arCharacterToSpawn.transform.position;
        //var skateboardPosDiff = new Vector3(skateboardPosTemp.x, 0, skateboardPosTemp.z);
        StartCoroutine(utils.LerpMovement(initialPos, pointToJumpUp, skateToRailingDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateToRailingDuration);
        // jump up
        SkateJump();
        StartCoroutine(utils.LerpMovement(pointToJumpUp, pointToJumpUp + railingHeight, skateJumpAnimationLen, arCharacterToSpawn));
        yield return new WaitForSeconds(skateJumpAnimationLen);
        // skate on railing
        var skateOnRailingDuration = 2.5f;
        StartCoroutine(utils.LerpMovement(pointToJumpUp + railingHeight, pointToJumpDown + railingHeight, skateOnRailingDuration, arCharacterToSpawn));
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(pointToJumpDown + railingHeight);
        yield return new WaitForSeconds(skateOnRailingDuration);
        // jump down
        SkateJump();
        StartCoroutine(utils.LerpMovement(pointToJumpDown + railingHeight, pointToJumpDown, skateJumpAnimationLen, arCharacterToSpawn));
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(initialPos);
        yield return new WaitForSeconds(skateJumpAnimationLen);

        // skate back to position
        StartCoroutine(utils.LerpMovement(pointToJumpDown, initialPos, skateToRailingDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateToRailingDuration);
        // stop skating
        StopSkating();
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().StopSkating();
    }

    public void SkateOnRamp(Vector3 startingPoint, Vector3 endingPoint, float height, bool isPhase3 = false)
    {
        StartCoroutine(SkatingOnRamp(startingPoint, endingPoint, height, isPhase3));
    }

    IEnumerator SkatingOnRamp(Vector3 startingPoint, Vector3 endingPoint, float height, bool isPhase3)
    {
        var initialPos = arCharacterToSpawn.transform.position;
        // 1. start skating to ramp starting
        if (!isPhase3)
        {
            StartSkating();
            yield return new WaitForSeconds(idleToSkateAnimationLen);
        }
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(Camera.main.transform.position);
        var skateToRampDuration = 2.0f;
        StartCoroutine(utils.LerpMovement(initialPos, startingPoint, skateToRampDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateToRampDuration);
        // 2. skate to ramp endpoint with height
        var skateOnRampDuration = 2.5f;
        var rampEndPoint = endingPoint + new Vector3(0, height, 0);
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(rampEndPoint);
        StartCoroutine(utils.LerpMovement(startingPoint, rampEndPoint, skateOnRampDuration, arCharacterToSpawn));
        yield return new WaitForSeconds(skateOnRampDuration);
        // 3. jump down to ramp lowest point
        SkateJump();
        StartCoroutine(utils.LerpMovement(rampEndPoint, endingPoint, skateJumpAnimationLen, arCharacterToSpawn));
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(initialPos);
        yield return new WaitForSeconds(skateJumpAnimationLen);
        // 4. skate back to initialPos
        StartCoroutine(utils.LerpMovement(endingPoint, initialPos, skateToRampDuration, arCharacterToSpawn));
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().SkateDirection(initialPos);
        yield return new WaitForSeconds(skateToRampDuration);

        if (!isPhase3)
        {
            StopSkating();
        }
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().StopSkating();
    }

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
        StartCoroutine(PlayTalkingAnimationWithinDuration(duration));
    }

    IEnumerator PlayTalkingAnimationWithinDuration(float duration)
    {
        animator.SetBool("isTalking", true);
        yield return new WaitForSeconds(duration);
        animator.SetBool("isTalking", false);
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
