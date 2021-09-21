using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{

    public GameObject arCubeToSpawn;
    public GameObject arCharacterToSpawn;
    public GameObject placementIndicator;

    private GameObject spawnedObject;
    private ARRaycastManager aRRaycastManager;
    private Pose PlacementPose;
    private bool layoutPlaced = false;
    private bool placementPoseIsValid = false;
    private float objectMovingSpeed = 0.01f;

    private bool IsSelected = false;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
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
            var touchPosition = touch.position;
            Debug.Log("touch position is: " + touchPosition);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.current.ScreenPointToRay(touch.position);

                var hits = new List<ARRaycastHit>();
                aRRaycastManager.Raycast(touch.position, hits, TrackableType.All);

                if (hits.Count > 0)
                {
                    GameObject gameObject = hits[0].trackable.GetComponent<GameObject>();
                    Destroy(gameObject);
                }
                if (Physics.Raycast(ray, out RaycastHit hitObject))
                {
                    GameObject gameObject = hitObject.transform.GetComponent<GameObject>();
                    Debug.Log("Game Object is: " + gameObject);
                    Debug.Log("hitObject is: " + hitObject.collider);
                    Destroy(gameObject);

                    ARObjectPlacement placementObject = hitObject.transform.GetComponent<ARObjectPlacement>();
                    Debug.Log("Object is: " + placementObject);
                }
            }

            //if (touch.phase == TouchPhase.Moved)
            //{
            //    Debug.Log("moving touch: " + touch);

            //    arCharacterToSpawn.transform.position = new Vector3(
            //        arCharacterToSpawn.transform.position.x + touch.deltaPosition.x * objectMovingSpeed,
            //        arCharacterToSpawn.transform.position.y,
            //        arCharacterToSpawn.transform.position.z + touch.deltaPosition.y * objectMovingSpeed
            //    );
            //}
        }
    }

    void UpdatePlacementIndicator()
    {
        if (spawnedObject == null && placementPoseIsValid && layoutPlaced == false)
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
        arCharacterToSpawn = Instantiate(arCharacterToSpawn, PlacementPose.position + new Vector3(0.0f, 0.0f, -0.05f), PlacementPose.rotation);
        // to be placed in the sky and dropped down
        arCubeToSpawn = Instantiate(arCubeToSpawn, PlacementPose.position + new Vector3(0.0f, 0.0f, 0.05f), PlacementPose.rotation);

        placementIndicator.SetActive(false);
        layoutPlaced = true;
    }
}
