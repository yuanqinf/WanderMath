using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARPlacement : MonoBehaviour
{

    public GameObject arCubeToSpawn;
    public GameObject arCharacterToSpawn;
    public GameObject placementIndicator;
    public RectTransform sliderHandleTransform;
    public Camera arCamera;
    public float rotateDegreeFactor;

    private ARRaycastManager aRRaycastManager;
    private Pose PlacementPose;
    private bool layoutPlaced = false;
    private bool placementPoseIsValid = false;
    private GameObject touchedObject;
    private Vector2 initTouchPosition;
    private HandleSnapControl handleSnapControl;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        handleSnapControl = sliderHandleTransform.gameObject.GetComponentInParent<HandleSnapControl>();
    }

    // need to update placement indicator, placement pose and spawn 
    void Update()
    {
        if (!layoutPlaced)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ARPlaceLayout();
            }
        }

        if (layoutPlaced && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            RaycastHit hitObject;
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.current.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hitObject))
                {
                    initTouchPosition = touch.position;
                    touchedObject = hitObject.transform.gameObject;
                }
            }

            //if (touch.phase == TouchPhase.Moved)
            //{
            //    Vector2 newTouchPosition = Input.GetTouch(0).position;
            //    Debug.Log("newTouchPosition: " + newTouchPosition.y + " , " + newTouchPosition.x);
            //    if(touchedObject != null)
            //    {
            //        if(newTouchPosition.y > initTouchPosition.y || newTouchPosition.x > initTouchPosition.x)
            //        {
            //            Debug.Log("rotating upward");
            //            touchedObject.transform.parent.Rotate(new Vector3(-rotateDegree, 0, 0));
            //        }
            //        else
            //        {
            //            Debug.Log("rotating downward");
            //            touchedObject.transform.parent.Rotate(new Vector3(rotateDegree, 0, 0));
            //        }
            //    }
            //}

            //if (touch.phase == TouchPhase.Ended)
            //{
            //    float curAngle = touchedObject.transform.parent.GetComponent<Transform>().localRotation.eulerAngles.x;
            //    Debug.Log("unselected!!! : " + curAngle);
            //    if (curAngle > 80 && curAngle < 100)
            //    {
            //        touchedObject.transform.parent.Rotate(new Vector3(180, 0, 0));
            //    }
            //}
        }

        if(sliderHandleTransform.localPosition.y > 2 || sliderHandleTransform.localPosition.y < -2)
        {
            touchedObject.transform.parent.Rotate(new Vector3(-sliderHandleTransform.localPosition.y * rotateDegreeFactor, 0, 0));
        }

        // snap
        if (handleSnapControl.canSnap)
        {
            float curAngle = touchedObject.transform.parent.GetComponent<Transform>().localRotation.eulerAngles.x;
            if(curAngle > 180)
            {
                curAngle -= 360;
            }

            Debug.Log("touchedObject curAngle: " + curAngle);
            if (curAngle > 50)
            {
                Debug.Log("snap now!!! >50");

                touchedObject.transform.parent.Rotate((90 - touchedObject.transform.localRotation.eulerAngles.x), 0, 0);
            }

            if (curAngle < -50)
            {
                Debug.Log("snap now!!! < -50");
                touchedObject.transform.parent.Rotate((-90 - touchedObject.transform.localRotation.eulerAngles.x), 0, 0);
            }
            handleSnapControl.canSnap = false;
        }
    }

    void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && layoutPlaced == false)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
        }
    }

    private void ARPlaceLayout()
    {
        // things to place when initialized
        // to be placed at the corner
        arCharacterToSpawn = Instantiate(arCharacterToSpawn, PlacementPose.position + new Vector3(-0.5f, 0.0f, -0.01f), PlacementPose.rotation);
        // to be placed in the sky and dropped down
        arCubeToSpawn = Instantiate(arCubeToSpawn, PlacementPose.position + new Vector3(0.0f, 0.0f, 0.05f), PlacementPose.rotation);

        placementIndicator.SetActive(false);
        layoutPlaced = true;
    }
}
