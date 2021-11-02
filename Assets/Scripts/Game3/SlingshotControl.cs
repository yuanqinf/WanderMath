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

    private Vector3 rubberJointLeftOriginPos;
    private Vector3 rubberJointRightOriginPos;

    // Start is called before the first frame update
    void Start()
    {

        m_SessionOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        ARCam = m_SessionOrigin.transform.Find("AR Camera").gameObject;
        arCamera = Camera.main;
        transform.parent = ARCam.transform;
        SetPos();
        //this.gameObject.SetActive(false);
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
                        rubberJointLeftOriginPos = rubberJointLeft.transform.position;
                        rubberJointRightOriginPos = rubberJointRight.transform.position;
                    }
                }

            }
            else if (touch.phase == TouchPhase.Moved && isReadyToShoot)
            {
                Vector2 newTouchPosition = Input.GetTouch(0).position;
                Ray ray = arCamera.ScreenPointToRay(newTouchPosition);
                RaycastHit rayLocation;
                if (Physics.Raycast(ray, out rayLocation))
                {
                    Vector3 newRealWorldPosition = rayLocation.point;
                    if (newRealWorldPosition.z < initialRealWorldPosition.z)
                    {
                        rubberJointLeft.transform.Translate(new Vector3(0, 0, -0.01f));
                        rubberJointRight.transform.Translate(new Vector3(0, 0, -0.01f));
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isReadyToShoot = false;
                // reset rubber pos
                rubberJointLeft.transform.position = rubberJointLeftOriginPos;
                rubberJointRight.transform.position = rubberJointRightOriginPos;
            }
        }
    }

    public void SetPos()
    {
        Vector3 axiusPos = ARCam.transform.position + ARCam.transform.forward * m_SlingshotCameraOffset.z + ARCam.transform.up * m_SlingshotCameraOffset.y;
        transform.position = axiusPos;
    }
}