using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperUtils : GenericClass
{
    /// <summary>
    /// Initialize object based on duration & distance it'll float from.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="upDistance"></param>
    public GameObject PlaceObjectInSky(GameObject gameObject, Vector3 position, Quaternion rotation, float duration, float upDistance)
    {
        var endPos = position + new Vector3(0.0f, 0.0f, 0.05f);
        var startPos = position + new Vector3(0.0f, 0.0f, 0.05f) + Vector3.up * upDistance;

        Quaternion cubeRot = Quaternion.Euler(rotation.eulerAngles);

        gameObject = Instantiate(
            gameObject,
            startPos,
            cubeRot
        );
        gameObject.SetActive(true);
        StartCoroutine(LerpMovement(startPos, endPos, duration, gameObject));
        return gameObject;
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

    /// <summary>
    /// Handle snapping for phase 3 objects and play audio, subtitle & animation accordingly.
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="numSnapped"></param>
    internal void HandlePhase3SnapEffect(string objectName, int numSnapped)
    {
        switch (objectName)
        {
            case Constants.ShapeNames.CUBE_WRONG:
                if (numSnapped == 5)
                {
                    StartCoroutine(cubeRotateControl.CompletePhase2WrongCubeSubtitleWithAudio());
                }
                else if (numSnapped < 5)
                {
                    SmallWinEffect(numSnapped);
                }
                break;
            case Constants.ShapeNames.CUBE_EASY:
            case Constants.ShapeNames.CUBE_MED:
            case Constants.ShapeNames.CUBE_MED2:
                if (numSnapped == 5)
                {
                    characterController.PlayBigWin();
                    soundManager.PlaySuccessSound();
                }
                else if (numSnapped < 5)
                {
                    SmallWinEffect(numSnapped);
                }
                break;
            case Constants.ShapeNames.CUBOID:
                if (numSnapped == 5)
                {
                    characterController.PlayBigWin();
                    soundManager.PlaySuccessSound();
                    StartCoroutine(shapesController.PlayCuboidSubtitleWithAudio());
                }
                else if (numSnapped < 5)
                {
                    SmallWinEffect(numSnapped);
                }
                break;
            case Constants.ShapeNames.HEXAGON:
                if (numSnapped == 7)
                {
                    characterController.PlayBigWin();
                    soundManager.PlaySuccessSound();
                    StartCoroutine(shapesController.PlayHexagonSubtitleWithAudio());
                }
                else if (numSnapped < 7)
                {
                    SmallWinEffect(numSnapped);
                }
                break;
            case Constants.ShapeNames.PYRAMID:
                if (numSnapped == 4)
                {
                    characterController.PlayBigWin();
                    soundManager.PlaySuccessSound();
                    StartCoroutine(shapesController.PlayPyramidSubtitleWithAudio());
                }
                else if (numSnapped < 4)
                {
                    SmallWinEffect(numSnapped);
                }
                break;
        }
    }

    private void SmallWinEffect(int num)
    {
        characterController.PlaySmallWin();
        switch(num)
        {
            case 1:
                soundManager.PlaySnapSound1();
                break;
            case 2:
                soundManager.PlaySnapSound2();
                break;
            case 3:
                soundManager.PlaySnapSound3();
                break;
            case 4:
                soundManager.PlaySnapSound4();
                break;
            case 5:
                soundManager.PlaySnapSound5();
                break;
            case 6:
                soundManager.PlaySnapSound6();
                break;
            case 7:
                soundManager.PlaySnapSound7();
                break;
            case 8:
                soundManager.PlaySnapSound8();
                break;
        }
    }
}
