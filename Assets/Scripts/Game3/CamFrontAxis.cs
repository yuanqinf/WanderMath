using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CamFrontAxis : MonoBehaviour
{
    [SerializeField]
    GameObject ARCam;

    [SerializeField]
    ARSessionOrigin m_SessionOrigin;

    private Vector3 m_BallCameraOffset = new Vector3(0f, -0.2f, 0.8f);


    // Start is called before the first frame update
    void Start()
    {
        m_SessionOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        ARCam = m_SessionOrigin.transform.Find("AR Camera").gameObject;
        transform.parent = ARCam.transform;
        SetAxis();
        this.gameObject.SetActive(false);
    }


    public void SetAxis()
    {
        Vector3 axiusPos = ARCam.transform.position + ARCam.transform.forward * m_BallCameraOffset.z + ARCam.transform.up * m_BallCameraOffset.y;
        transform.position = axiusPos;
    }
}
