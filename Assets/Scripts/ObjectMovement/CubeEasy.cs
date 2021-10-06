using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEasy : GenericClass
{
    private int numSnapped = 0;

    public void RotateEasyFace(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        //handleOutline(touchedObject);
        switch (touchedObject.name)
        {
            case "NetFace_1":
                Debug.Log("touching netface 1!!!!!!!");
                if (newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "NetFace_2":
                if (newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "NetFace_3":
                Debug.Log("touching netface 3!!!!!!!");
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "NetFace_4":
                Debug.Log("touching netface 4!!!!!!!");
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "NetFace_5":
                Debug.Log("touching netface 5!!!!!!!");
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    ChildSnapDetection(touchedObject);
                }
                break;
        }
    }

    private void ChildSnapDetection(GameObject gameObject)
    {
        if (gameObject.transform.localEulerAngles.z > Constants.ROTATION_THRESHOLD)
        {
            ChildSnapObject(gameObject);
        }
    }

    private void ChildSnapObject(GameObject gameObject)
    {
        numSnapped++;
        gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y, 90);
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
    }

    private void SnapDetection(GameObject gameObject)
    {
        if (gameObject.transform.eulerAngles.z > Constants.ROTATION_THRESHOLD)
        {
            SnapObject(gameObject);
        }
    }

    private void SnapObject(GameObject gameObject)
    {
        numSnapped++;
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 90);
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        //utils.HandlePhase3SnapEffect(Constants.ShapeNames.PYRAMID, numSnapped);
    }
}