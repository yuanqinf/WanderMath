using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboidController : GenericClass
{
    [SerializeField]
    private float cuboidThresholdDegree = 50f;
    private float cuboidSetDegree = 90f;
    private int numSnapped = 0;

    public void UpdateCuboidRotation(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        switch (touchedObject.name)
        {
            case "cuboid1":
                Debug.Log("1: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "cuboid2":
                Debug.Log("2: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "cuboid3":
                Debug.Log("3: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "cuboid4":
                Debug.Log("4: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "cuboid5":
                Debug.Log("5: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject, true);
                }
                break;
        }
    }

    private void SnapDetection(GameObject gameObject, bool isSecondLayer = false)
    {
        if (isSecondLayer)
        {
            if (gameObject.transform.localEulerAngles.z > cuboidThresholdDegree)
            {
                SnapObject(gameObject, true);
            }
        }
        else
        {
            if (gameObject.transform.eulerAngles.z > cuboidThresholdDegree)
            {
                //if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
                SnapObject(gameObject);
            }
        }
    }

    private void SnapObject(GameObject gameObject, bool isSecondLevel = false)
    {
        numSnapped++;

        if (isSecondLevel)
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, cuboidSetDegree);
        } else
        {
            gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, cuboidSetDegree);
        }
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        utils.HandlePhase3SnapEffect(Constants.ShapeNames.CUBOID, numSnapped);
    }
}
