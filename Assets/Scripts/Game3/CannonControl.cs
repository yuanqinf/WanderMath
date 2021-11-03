using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonControl : MonoBehaviour
{
    private Camera arCamera;
    private Vector3 initialRealWorldPosition;
    private bool isReadyToMove;
    private bool isMovingRight;

    private bool isReadyToRotate;
    private bool isRotatingUp;
    private Vector2 newTouchPosition;
    public GameObject muzzle;

    public float muzzleAngle = 8f;

    // Start is called before the first frame update
    void Start()
    {
        arCamera = Camera.main;
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
                    if (hitObject.transform.tag == "cannon")
                    {
                        initialRealWorldPosition = hitObject.point;
                        isReadyToMove = true;
                        Debug.Log("move begin");
                    }

                    if (hitObject.transform.tag == "muzzle")
                    {
                        initialRealWorldPosition = hitObject.point;
                        isReadyToRotate = true;
                        Debug.Log("rotate begin");
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (isReadyToMove)
                {
                    newTouchPosition = Input.GetTouch(0).position;
                    Ray ray = arCamera.ScreenPointToRay(newTouchPosition);
                    RaycastHit rayLocation;
                    if (Physics.Raycast(ray, out rayLocation))
                    {
                        Vector3 newRealWorldPosition = rayLocation.point;
                        if (newRealWorldPosition.x < initialRealWorldPosition.x)
                        {
                            Debug.Log("moving left");
                            isMovingRight = false;

                        }
                        else if (newRealWorldPosition.x > initialRealWorldPosition.x)
                        {
                            Debug.Log("moving right");
                            isMovingRight = true;
                        }
                    }
                }
                if (isReadyToRotate)
                {
                    newTouchPosition = Input.GetTouch(0).position;
                    Ray ray = arCamera.ScreenPointToRay(newTouchPosition);
                    RaycastHit rayLocation;
                    if (Physics.Raycast(ray, out rayLocation))
                    {
                        Vector3 newRealWorldPosition = rayLocation.point;
                        if (newRealWorldPosition.y < initialRealWorldPosition.y)
                        {
                            Debug.Log("moving down");
                            isRotatingUp = false;
                        }
                        else if (newRealWorldPosition.y > initialRealWorldPosition.y)
                        {
                            Debug.Log("moving up");
                            isRotatingUp = true;
                        }
                    }
                }

            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (isReadyToMove)
                {
                    Debug.Log("should move now");
                    if (isMovingRight)
                    {
                        this.transform.localPosition += (new Vector3(0.11f, 0f, 0f));
                    }
                    else
                    {
                        this.transform.localPosition += (new Vector3(-0.11f, 0f, 0f));
                    }
                    isReadyToMove = false;
                }
                if (isReadyToRotate)
                {
                    Debug.Log("should rotate now");

                    if (isRotatingUp)
                    {
                        muzzle.transform.Rotate(new Vector3(muzzleAngle, 0f, 0f));
                    }
                    else
                    {
                        muzzle.transform.Rotate(new Vector3(-muzzleAngle, 0f, 0f));
                    }
                    isReadyToRotate = false;
                }
            }
        }
    }
}
