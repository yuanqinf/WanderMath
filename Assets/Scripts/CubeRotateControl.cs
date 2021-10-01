using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotateControl : MonoBehaviour
{

    public Vector3 testV3 = new Vector3(20, 20, 20);

    public void rotateFace(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        if (touchedObject.name == "NetFace_1")
        {
            if (newRealWorldPosition.z > initialRealWorldPosition.z)
            {
                touchedObject.transform.Rotate(new Vector3(0, 0, 5));
                if (touchedObject.transform.eulerAngles.z > 80)
                {
                    Debug.Log("SNAP!!!1");
                    snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                }
            }
        }
        if (touchedObject.name == "NetFace_2")
        {
            if (newRealWorldPosition.z < initialRealWorldPosition.z)
            {
                touchedObject.transform.Rotate(new Vector3(0, 0, 5));
                if (touchedObject.transform.eulerAngles.z > 80)
                {
                    Debug.Log("SNAP!!!2");
                    snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                }
            }
        }
        if (touchedObject.name == "NetFace_3")
        {
            if (newRealWorldPosition.x > initialRealWorldPosition.x)
            {
                touchedObject.transform.Rotate(new Vector3(0, 0, 5));
                if (touchedObject.transform.eulerAngles.z > 80)
                {
                    Debug.Log("SNAP!!!3");
                    snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                }
            }
        }
        if (touchedObject.name == "NetFace_4")
        {
            if (newRealWorldPosition.x < initialRealWorldPosition.x)
            {
                touchedObject.transform.Rotate(new Vector3(0, 0, 5));
                if (touchedObject.transform.eulerAngles.z > 80)
                {
                    Debug.Log("SNAP!!!4");
                    snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                }
            }
        }
        if (touchedObject.name == "NetFace_5")
        {
            if (newRealWorldPosition.x < initialRealWorldPosition.x)
            {
                touchedObject.transform.Rotate(new Vector3(0, 0, 5));
                if (touchedObject.transform.eulerAngles.z > 80)
                {
                    Debug.Log("SNAP!!!5");
                    snapObject(touchedObject, touchedObject.transform.eulerAngles.x, touchedObject.transform.eulerAngles.y, 90);
                }
            }
        }
    }

    private void snapObject(GameObject touchedObject, float x, float y, float z)
    {
        Debug.Log("snapped object is: " + touchedObject.name + " with local angle: " + touchedObject.transform.eulerAngles + " and parent angle: " + touchedObject.transform.parent.transform.eulerAngles);
        Vector3 newAngle = new Vector3(x, y, z);
        touchedObject.transform.transform.eulerAngles = newAngle;
        touchedObject.transform.GetComponent<BoxCollider>().enabled = false;
    }
}
