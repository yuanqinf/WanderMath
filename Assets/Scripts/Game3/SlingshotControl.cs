using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SlingshotControl : MonoBehaviour
{
    [SerializeField]
    GameObject ARCam;

    private Camera arCamera;
    private Vector3 initialRealWorldPosition;

    [SerializeField]
    ARSessionOrigin m_SessionOrigin;

    private Vector3 m_SlingshotCameraOffset = new Vector3(0f, -0.22f, 0.8f);

    private bool isReadyToShoot = false;

    public GameObject rubberJointLeft;
    public GameObject rubberJointRight;

    private Vector2 newTouchPosition;

    void Start()
    {
        m_SessionOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        ARCam = m_SessionOrigin.transform.Find("AR Camera").gameObject;
        arCamera = Camera.main;
        transform.parent = ARCam.transform;
        SetPos();
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hitObject))
                {
                    if (hitObject.transform.tag == "slingshot_rubber")
                    {
                        initialRealWorldPosition = hitObject.point;
                        isReadyToShoot = true;
                        Debug.Log("touch begin");
                        // turn of move slingshot collider when pulling rubber
                        this.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }

            }
            else if (touch.phase == TouchPhase.Moved && isReadyToShoot)
            {
                if (isReadyToShoot)
                {
                    newTouchPosition = Input.GetTouch(0).position;
                    Ray ray = arCamera.ScreenPointToRay(newTouchPosition);
                    RaycastHit rayLocation;
                    if (Physics.Raycast(ray, out rayLocation))
                    {
                        Vector3 newRealWorldPosition = rayLocation.point;
                        if (newRealWorldPosition.z < initialRealWorldPosition.z && rubberJointLeft.transform.localPosition.z >= -40f)
                        {
                            rubberJointLeft.transform.Translate(new Vector3(0, 0, -0.02f));
                            rubberJointRight.transform.Translate(new Vector3(0, 0, -0.02f));
                        }
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (isReadyToShoot)
                {
                    SetPos();
                }
                isReadyToShoot = false;
                // reset rubber pos
                rubberJointLeft.transform.localPosition = new Vector3(-20f, 0f, 0f);
                rubberJointRight.transform.localPosition = new Vector3(-20f, 0f, 0f);
                this.gameObject.GetComponent<BoxCollider>().enabled = true;
            }

            Ray ray_slingshotmain = Camera.main.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray_slingshotmain, out RaycastHit hitObject_main))
            {
                if (touch.phase == TouchPhase.Moved && !isReadyToShoot)
                {
                    if (hitObject_main.transform.tag == "slingshot_main")
                    {
                        hitObject_main.transform.position = new Vector3(hitObject_main.point.x, transform.position.y, hitObject_main.point.z);
                    }
                }
            }
        }
    }

    public void SetPos()
    {
        Vector3 slingshotPosBasedCam = ARCam.transform.position + ARCam.transform.forward * m_SlingshotCameraOffset.z + ARCam.transform.up * m_SlingshotCameraOffset.y;
        transform.position = slingshotPosBasedCam;
    }
}