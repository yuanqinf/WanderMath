using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class InitializeArObject : MonoBehaviour
{
    public GameObject objectToSpawn;
    public GameObject placementIndicator;
    public Camera arCamera;

    private Pose placementPose;
    private ARRaycastManager aRRaycastManager;
    private bool isPlacementPoseValid = false;
    private bool isObjectPlaced = false;

    private GameObject touchedObject;
    private Vector3 initialRealWorldPosition;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        arCamera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        if (!isObjectPlaced)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
            if (isPlacementPoseValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Instantiate(objectToSpawn, placementPose.position, placementPose.rotation);
                isObjectPlaced = true;
                placementIndicator.SetActive(false);
            }
        }
        else if (isObjectPlaced && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            RaycastHit hitObject;
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hitObject))
                {
                    initialRealWorldPosition = hitObject.point;
                    //Debug.Log("hit position vector3: " + hitObject.point);
                    // arCamera.ScreenToWorldPoint(new Vector3(initTouchPosition.x, initTouchPosition.y, arCamera.nearClipPlane)) // doesnt work, always the same value
                    touchedObject = hitObject.transform.gameObject;
                    //Debug.Log("touchedObject location: " + touchedObject.transform.position);
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 newTouchPosition = Input.GetTouch(0).position;
                Ray ray = arCamera.ScreenPointToRay(newTouchPosition);
                RaycastHit rayLocation;
                if (Physics.Raycast(ray, out rayLocation))
                {
                    Vector3 newRealWorldPosition = rayLocation.point;

                    //uiController.SetCursorPosition(newTouchPosition);
                    if (touchedObject != null)
                    {
                        //Debug.Log("this is touched object name: " + touchedObject.name);
                        switch (touchedObject.tag)
                        {
                            case "pyramid":
                                UpdatePyramid(newRealWorldPosition);
                                break;
                            case "hexagon":
                                UpdateHex(newRealWorldPosition);
                                break;
                            default:
                                //Debug.Log("objectname: " + touchedObject.name);
                                break;
                        }

                        Debug.Log("initialRealWorldPosition: " + initialRealWorldPosition);
                        Debug.Log("newRealWorldPosition: " + newRealWorldPosition);
                    }
                }
            }
        }
    }

    private void UpdateHex(Vector3 newRealWorldPosition)
    {
        switch (touchedObject.name)
        {
            case "hexSquare1":
                Debug.Log("square1: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexDetection(touchedObject);
                }
                break;
            case "hexSquare2":
                Debug.Log("square2: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexDetection(touchedObject);
                }
                break;
            case "hexSquare3":
                Debug.Log("square3: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexDetection(touchedObject);
                }
                break;
            case "hexSquare4":
                Debug.Log("square4: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexDetection(touchedObject);
                }
                break;
            case "hexSquare5":
                Debug.Log("square5: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexDetection(touchedObject);
                }
                break;
            case "hexSquare6":
                Debug.Log("square6: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
                    SnapHexDetection(touchedObject);
                }
                break;
            case "hexCylinder":
                Debug.Log("hexCylinder: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.y < initialRealWorldPosition.y)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    //touchedObject.transform.eulerAngles += new Vector3(Constants.ROTATION_DEGREE, 0, 0);
                    SnapHexHexDetection(touchedObject);
                }
                break;
        }
    }

    private void UpdatePyramid(Vector3 newRealWorldPosition)
    {
        switch (touchedObject.name)
        {
            case "isoTriangle1":
                Debug.Log("touching isoTriangle1!");
                Debug.Log("1: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                if (newRealWorldPosition.z > initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
            case "isoTriangle2":
                Debug.Log("2: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                Debug.Log("touching isoTriangle2!");
                if (newRealWorldPosition.z < initialRealWorldPosition.z)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
            case "isoTriangle3":
                Debug.Log("3: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                Debug.Log("touching isoTriangle3!");
                if (newRealWorldPosition.x > initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
            case "isoTriangle4":
                Debug.Log("4: " + initialRealWorldPosition + " newWorld: " + newRealWorldPosition);
                Debug.Log("touching isoTriangle4!");
                if (newRealWorldPosition.x < initialRealWorldPosition.x)
                {
                    touchedObject.transform.Rotate(new Vector3(Constants.ROTATION_DEGREE, 0, 0));
                    SnapDetection(touchedObject);
                }
                break;
        }
    }

    private void SnapHexHexDetection(GameObject gameObject)
    {
        Debug.Log("new rotation angle is: " + gameObject.transform.rotation.eulerAngles);
        Debug.Log("new rotation transofrm angle is: " + gameObject.transform.eulerAngles);
        Debug.Log("new rotation transform is: " + gameObject.transform.rotation * Vector3.forward);
        if (gameObject.transform.eulerAngles.x > 55f + 90f)
        {
            //if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
            //SnapHexHexObject(gameObject);
            //gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void SnapHexHexObject(GameObject gameObject)
    {
        var hexSetDegree = 179f;
        gameObject.transform.eulerAngles = new Vector3(hexSetDegree, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        this.touchedObject = null;
    }

    private void SnapHexDetection(GameObject gameObject)
    {
        if (gameObject.transform.eulerAngles.z > 65f)
        {
            //if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
            SnapHexObject(gameObject);
            gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void SnapHexObject(GameObject gameObject)
    {
        var hexSetDegree = 90f;
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, hexSetDegree);
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        this.touchedObject = null;
    }

    private void SnapDetection(GameObject gameObject)
    {
        if (gameObject.transform.eulerAngles.z > 85f)
        {
            //if (touchedObject.transform.GetComponent<BoxCollider>().enabled) curCubeSnappedSides++;
            SnapObject(gameObject);
            gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void SnapObject(GameObject gameObject)
    {
        var pyramidSetDegree = 116f;
        gameObject.transform.eulerAngles = new Vector3(pyramidSetDegree, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        this.touchedObject = null;
    }

    private void UpdatePlacementIndicator()
    {
        if (isPlacementPoseValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        isPlacementPoseValid = hits.Count > 0;
        if (isPlacementPoseValid)
        {
            placementPose = hits[0].pose; // update position
            var cameraForward = arCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
