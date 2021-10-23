using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : GenericClass
{
    [SerializeField]
    private GameObject arCharacterToSpawn;
    public Animation skatingAnimation;
    private Animator animator;

    private string introLine = "Oh, hi! I'm Finley. Nice to meet you!";
    private string activity2IntroLine = "Hey there! I'm gonna build Finley Park, my own personal skate park!";

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
        Debug.Log(placementPose.position);
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
        Debug.Log(endPos);
        Debug.Log("char pos: " + arCharacterToSpawn.transform.position);
        var duration = 5.0f;
        StartCoroutine(utils.LerpMovement(characterPos,
            endPos,
            duration,
            arCharacterToSpawn));
        Debug.Log("new pos: " + arCharacterToSpawn.transform.position);
        PlaySkating(duration);
        uiController.PlaySubtitles(activity2IntroLine, duration);

        return 1.0f;
    }

    public void PlaySkating(float duration)
    {
        StartCoroutine(SkatingDuration(duration));
    }
    IEnumerator SkatingDuration(float duration)
    {
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().isSkating = true;
        StartSkating();
        yield return new WaitForSeconds(duration);
        StopSkating();
        yield return new WaitForSeconds(3.0f);
        arCharacterToSpawn.GetComponentInChildren<CharacterLookAt>().isSkating = false;
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

    public void PlaySmallWin()
    {
        animator.SetTrigger("isSmallWin");
    }

    public void PlayBigWin()
    {
        animator.SetTrigger("isBigWin");
    }
}
