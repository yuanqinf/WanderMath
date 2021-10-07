using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : GenericClass
{
    [SerializeField]
    private GameObject arCharacterToSpawn;

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
        return StartFirstLine();
    }

    private float StartFirstLine()
    {
        var duration = soundManager.PlayCharacterInitClip();
        uiController.PlaySubtitles(introLine, duration);
        return duration;
    }

    public Vector3 GetArCharacterPosition()
    {
        return arCharacterToSpawn.transform.position;
    }
}
