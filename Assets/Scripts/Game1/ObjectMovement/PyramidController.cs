using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyramidController : GenericClass
{
    private float pyramidThresholdDegree = Constants.ROTATION_THRESHOLD + 15f;
    private float pyramidSetDegree = 90 + 26f;
    public int numSnapped = 0;

    public void UpdatePyramidRotation(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        gameController.waitTime -= Time.deltaTime;
        if (gameController.waitTime <= 0 && !gameController.showedHelper)
        {
            gameController.showedHelper = true;
            gameController.showHelperText();
        }

        cubeRotateControl.handleSelected(touchedObject);
        Debug.Log(touchedObject.name + " : " + initialRealWorldPosition.ToString("N4") + " newWorld: " + newRealWorldPosition.ToString("N4"));
        switch (touchedObject.name)
        {
            case "isoTriangle1":
                if (newRealWorldPosition.z > initialRealWorldPosition.z || newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
            case "isoTriangle2":
                if (newRealWorldPosition.z < initialRealWorldPosition.z || newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
            case "isoTriangle3":
                if (newRealWorldPosition.x > initialRealWorldPosition.x || newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
            case "isoTriangle4":
                if (newRealWorldPosition.x < initialRealWorldPosition.x || newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
        }
        if (numSnapped == 4)
        {
            //touchedObject.transform.root.GetComponent<Outline>().enabled = false;
            gameController.playSuccessEffect(touchedObject);
            gameController.createPyGiftBox(touchedObject);
        }
    }

    private void SnapDetection(GameObject gameObject)
    {
        if (gameObject.transform.eulerAngles.x > pyramidThresholdDegree)
        {
            SnapObject(gameObject);
        }
    }

    public void SnapObject(GameObject gameObject)
    {
        numSnapped++;
        gameObject.transform.eulerAngles = new Vector3(pyramidSetDegree, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
        objectMovementController.ResetGameObject();
        utils.HandlePhase3SnapEffect(Constants.ShapeNames.PYRAMID, numSnapped);
        if (numSnapped == 4)
        {
            //touchedObject.transform.root.GetComponent<Outline>().enabled = false;
            gameController.playSuccessEffect(gameObject);
            gameController.createPyGiftBox(gameObject);
        }
    }
}
