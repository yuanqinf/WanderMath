using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : GenericClass
{
    [SerializeField]
    private GameObject arCharacterToSpawn;
    private Animator animator;

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
