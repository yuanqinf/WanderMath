using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonController : MonoBehaviour
{
    [SerializeField]
    private float hexagonThresholdDegree = 55f;
    private float hexSetDegree = 90f;

    public void UpdateHexRotation(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        switch (touchedObject.name)
        {
            case "hexSquare1":
            case "hexSquare3":
                Debug.Log("square1: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexSquareDetection(touchedObject);
                }
                break;
            case "hexSquare2":
            case "hexSquare4":
                Debug.Log("square2: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexSquareDetection(touchedObject);
                }
                break;
            case "hexSquare5":
                Debug.Log("square5: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexSquareDetection(touchedObject);
                }
                break;
            case "hexSquare6":
                Debug.Log("square6: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexSquareDetection(touchedObject);
                }
                break;
            case "hexCylinder":
                Debug.Log("hexCylinder: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.y < initialRealWorldPosition.y)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    //touchedObject.transform.eulerAngles += new Vector3(Constants.ROTATION_DEGREE, 0, 0);
                    SnapHexagonDetection(touchedObject);
                }
                break;
        }
    }

    /// <summary>
    /// Handles snapping of hexagon shape.
    /// </summary>
    /// <param name="gameObject"></param>
    private void SnapHexagonDetection(GameObject gameObject)
    {
        if (gameObject.transform.localEulerAngles.x > hexagonThresholdDegree)
        {
            //if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
            SnapHexagonObject(gameObject);
        }
    }

    private void SnapHexagonObject(GameObject gameObject)
    {
        gameObject.transform.localEulerAngles = new Vector3(hexSetDegree, gameObject.transform.localEulerAngles.y, gameObject.transform.localEulerAngles.z);
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// Handles snapping of square shapes.
    /// </summary>
    /// <param name="gameObject"></param>
    private void SnapHexSquareDetection(GameObject gameObject)
    {
        if (gameObject.transform.eulerAngles.z > hexagonThresholdDegree)
        {
            //if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
            SnapHexObject(gameObject);
        }
    }

    private void SnapHexObject(GameObject gameObject)
    {
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, hexSetDegree);
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
    }
}
