using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonController : GenericClass
{
    private float hexSetDegree = 90f;
    public int numSnapped = 0;

    public void UpdateHexRotation(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        //cubeRotateControl.handleOutline(touchedObject);
        Debug.Log(touchedObject.name + " : " + initialRealWorldPosition.ToString("N4") + " newWorld: " + newRealWorldPosition.ToString("N4"));
        switch (touchedObject.name)
        {
            case "hexSquare3":
            case "hexSquare1":
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexSquareDetection(touchedObject);
                }
                break;
            case "hexSquare2":
            case "hexSquare4":
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexSquareDetection(touchedObject);
                }
                break;
            case "hexSquare5":
                if (newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexSquareDetection(touchedObject);
                }
                break;
            case "hexSquare6":
                if (newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexSquareDetection(touchedObject);
                }
                break;
            case "hexCylinder":
                if (newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    //touchedObject.transform.eulerAngles += new Vector3(Constants.ROTATION_DEGREE, 0, 0);
                    SnapHexagonDetection(touchedObject);
                }
                break;
        }
        if (numSnapped == 7)
        {
            //touchedObject.transform.root.GetComponent<Outline>().enabled = false;
            gameController.playSuccessEffect(touchedObject);
            gameController.createHexGiftBox(touchedObject);
        }
    }

    /// <summary>
    /// Handles snapping of hexagon shape.
    /// </summary>
    /// <param name="gameObject"></param>
    private void SnapHexagonDetection(GameObject gameObject)
    {
        if (gameObject.transform.localEulerAngles.x > Constants.ROTATION_THRESHOLD)
        {
            SnapHexagonObject(gameObject);
        }
    }

    private void SnapHexagonObject(GameObject gameObject)
    {
        numSnapped++;
        gameObject.transform.localEulerAngles = new Vector3(hexSetDegree, gameObject.transform.localEulerAngles.y, gameObject.transform.localEulerAngles.z);
        objectMovementController.ResetGameObject();
        utils.HandlePhase3SnapEffect(Constants.ShapeNames.HEXAGON, numSnapped);
    }

    /// <summary>
    /// Handles snapping of square shapes.
    /// </summary>
    /// <param name="gameObject"></param>
    private void SnapHexSquareDetection(GameObject gameObject)
    {
        if (gameObject.transform.eulerAngles.z > Constants.ROTATION_THRESHOLD)
        {
            SnapHexObject(gameObject);
        }
    }

    private void SnapHexObject(GameObject gameObject)
    {
        numSnapped++;
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, hexSetDegree);
        objectMovementController.ResetGameObject();
        utils.HandlePhase3SnapEffect(Constants.ShapeNames.HEXAGON, numSnapped);
    }
}
