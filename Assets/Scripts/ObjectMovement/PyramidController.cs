using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyramidController : GenericClass
{
    [SerializeField]
    private float pyramidThresholdDegree = 75f;
    private float pyramidSetDegree = 116f;
    private int numSnapped = 0;

    public void UpdatePyramidRotation(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        switch (touchedObject.name)
        {
            case "isoTriangle1":
                Debug.Log("1: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
            case "isoTriangle2":
                Debug.Log("2: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
            case "isoTriangle3":
                Debug.Log("3: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
            case "isoTriangle4":
                Debug.Log("4: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
        }
    }

    private void SnapDetection(GameObject gameObject)
    {
        if (gameObject.transform.eulerAngles.z > pyramidThresholdDegree)
        {
            SnapObject(gameObject);
        }
    }

    private void SnapObject(GameObject gameObject)
    {
        numSnapped++;
        gameObject.transform.eulerAngles = new Vector3(pyramidSetDegree, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        utils.HandlePhase3SnapEffect(Constants.ShapeNames.PYRAMID, numSnapped);
    }
}