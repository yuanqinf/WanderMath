using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private GameObject arCharacterToSpawn;

    private string introLine = "Oh, hi! I'm Finley. Nice to meet you!";
    private UiController uiController;

    private void Start()
    {
        uiController = FindObjectOfType<UiController>();
    }

    public float InitCharacterFirst(Pose placementPose, Transform placementPos)
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
        // TODO: add audio here and update duration
        uiController.PlaySubtitles(introLine, 3.0f);
        return 3.0f;
    }
}
