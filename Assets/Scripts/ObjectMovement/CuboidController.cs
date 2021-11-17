using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboidController : GenericClass
{
    private float cuboidSetDegree = 90f;
    public int numSnapped = 0;

    public void UpdateCuboidRotation(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        //cubeRotateControl.handleOutline(touchedObject);
        Debug.Log(touchedObject.name + " : " + initialRealWorldPosition.ToString("N4") + " newWorld: " + newRealWorldPosition.ToString("N4"));
        switch (touchedObject.name)
        {
            case "cuboid1":
                if (newRealWorldPosition.x > initialRealWorldPosition.x || newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "cuboid2":
                if (newRealWorldPosition.x < initialRealWorldPosition.x || newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "cuboid3":
                if (newRealWorldPosition.z < initialRealWorldPosition.z || newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "cuboid4":
                if (newRealWorldPosition.z > initialRealWorldPosition.z || newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            // special one as a child
            case "cuboid5":
                if (newRealWorldPosition.x > initialRealWorldPosition.x || newRealWorldPosition.z > initialRealWorldPosition.z || newRealWorldPosition.y < initialRealWorldPosition.y)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject, true);
                }
                break;
        }
        if (numSnapped == 5)
        {
            //touchedObject.transform.root.GetComponent<Outline>().enabled = false;
            gameController.playSuccessEffect(touchedObject);
            gameController.createCuboidGiftBox(touchedObject);
        }
    }

    private void SnapDetection(GameObject gameObject, bool isSecondLayer = false)
    {
        if (isSecondLayer)
        {
            if (gameObject.transform.localEulerAngles.z > Constants.ROTATION_THRESHOLD)
            {
                SnapObject(gameObject, true);
            }
        }
        else
        {
            if (gameObject.transform.eulerAngles.z > Constants.ROTATION_THRESHOLD)
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
        objectMovementController.ResetGameObject();
        utils.HandlePhase3SnapEffect(Constants.ShapeNames.CUBOID, numSnapped);
    }
}
