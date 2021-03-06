using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeWrong : GenericClass
{
    public int numSnapped = 0;

    public void RotateWrongFace(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        gameController.waitTime -= Time.deltaTime;
        if (gameController.waitTime <= 0 && !gameController.showedHelper)
        {
            gameController.showedHelper = true;
            gameController.showHelperText();
        }

        cubeRotateControl.handleSelected(touchedObject);
        switch (touchedObject.name)
        {
            case "NetFace_1":
                if (newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "NetFace_2":
                if (newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    ChildSnapDetection(touchedObject);
                }
                break;
            case "NetFace_3":
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "NetFace_4":
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapDetection(touchedObject);
                }
                break;
            case "NetFace_5":
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    ChildSnapDetection(touchedObject);
                }
                break;
        }
        if (numSnapped == 5)
        {
            //touchedObject.transform.root.GetComponent<Outline>().enabled = false;
            gameController.playWrongEffect(touchedObject);
            soundManager.PlayWrongSound();
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
        objectMovementController.ResetGameObject();
        utils.HandlePhase3SnapEffect(Constants.ShapeNames.CUBE_WRONG, numSnapped);
    }

    private void SnapDetection(GameObject gameObject)
    {
        if (gameObject.transform.eulerAngles.z > Constants.ROTATION_THRESHOLD)
        {
            SnapObject(gameObject);
        }
    }

    public void SnapObject(GameObject gameObject)
    {
        numSnapped++;
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 90);
        objectMovementController.ResetGameObject();
        utils.HandlePhase3SnapEffect(Constants.ShapeNames.CUBE_WRONG, numSnapped);
        if (numSnapped == 5)
        {
            //touchedObject.transform.root.GetComponent<Outline>().enabled = false;
            gameController.playWrongEffect(gameObject);
            soundManager.PlayWrongSound();
        }
    }
}
