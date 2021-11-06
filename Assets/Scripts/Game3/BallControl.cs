using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Rigidbody))]
public class BallControl : MonoBehaviour
{
    //This is the force of the throw
    private float m_ThrowForce = 350;

    //X and Y damping factors for the throw direction (0,0) means straight line
    public float m_ThrowDirectionX = 0.0f;
    public float m_ThrowDirectionY = 0.0f;

    //Offset of the ball's position in relation to camera's position
    private Vector3 m_BallCameraOffset = new Vector3(0f, -0.03f, 0.8f);

    //The following variables contain the state of the current throw
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 initPosition;
    private float startTime;
    private float endTime;
    private bool isPressed = false;

    private Rigidbody rb;

    private CannonControl cannonController;

    // Start is called before the first frame update
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        initPosition = this.transform.position;
        cannonController = FindObjectOfType<CannonControl>();
        //m_SessionOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        //ARCam = m_SessionOrigin.transform.Find("AR Camera").gameObject;
        //transform.parent = ARCam.transform;
        ResetBall();
        //this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hitObject)) {
                if (touch.phase == TouchPhase.Moved)
                {
                    //if (hitObject.transform.tag == "balloon")
                    //{
                    //    hitObject.transform.position = new Vector3(hitObject.point.x, transform.position.y, hitObject.point.z);
                    //}
                }

                if (hitObject.transform.tag == Constants.Tags.ShootBtn)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        Debug.Log("pressing!");
                        startPosition = touch.position;
                        startTime = Time.time;
                        isPressed = false;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        Debug.Log("shoot!");
                        endTime = Time.time;
                        cannonController.FireCannon();
                        isPressed = true;
                    }
                }
            }
        }

        if (isPressed)
        {
            rb.mass = 1;
            rb.useGravity = false;

            rb.AddForce(-transform.forward * m_ThrowForce);
            // TODO: add endposition
            startTime = 0.0f;
            startPosition = new Vector3(0, 0, 0);
            isPressed = false;
        }

        if (Time.time - endTime >= 1.5 && Time.time - endTime <= 2)
        {
            ResetBall();
        }
    }

    private void ResetBall()
    {
        rb.mass = 0;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        endTime = 0.0f;

        transform.position = this.transform.parent.position;
    }
}
