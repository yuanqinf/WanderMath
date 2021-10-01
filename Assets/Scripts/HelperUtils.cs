using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperUtils : MonoBehaviour
{
    /// <summary>
    /// Initialize object based on duration & distance it'll float from.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="upDistance"></param>
    public void PlaceObjectInSky(GameObject gameObject, Vector3 position, Quaternion rotation, float duration, float upDistance)
    {
        var endPos = position + new Vector3(0.0f, 0.0f, 0.05f);
        var startPos = position + new Vector3(0.0f, 0.0f, 0.05f) + Vector3.up * upDistance;

        Quaternion cubeRot = Quaternion.Euler(rotation.eulerAngles);

        gameObject = Instantiate(
            gameObject,
            startPos,
            cubeRot
        );
        StartCoroutine(LerpMovement(startPos, endPos, duration, gameObject));
    }

    /// <summary>
    /// Move object from a starting point to an ending point within a lerpTime
    /// </summary>
    /// <param name="startPos">initial pos of object</param>
    /// <param name="endPos">final position of object</param>
    /// <param name="lerpTime">time taken for object to move into position</param>
    /// <param name="gameObject">game object to be moving</param>
    /// <returns></returns>
    public IEnumerator LerpMovement(Vector3 startPos, Vector3 endPos, float lerpTime, GameObject gameObject)
    {
        float timeElapsed = 0;

        while (timeElapsed < lerpTime)
        {
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / lerpTime);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
    }

    public IEnumerator DelayTime(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

}
