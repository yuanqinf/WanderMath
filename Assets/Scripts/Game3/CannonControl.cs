using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Cannon
{
    public int x;
    public int y;
    private GameObject cannonBase;
    private GameObject muzzle;
    private float muzzleAngle;

    public Cannon(int x, int y, GameObject cannonBase, GameObject muzzle)
    {
        this.x = x;
        this.y = y;
        this.cannonBase = cannonBase;
        this.muzzle = muzzle;
        this.muzzleAngle = 6f;
    }

    public void MoveXLeft()
    {
        if (x > 0)
        {
            cannonBase.transform.localPosition -= (new Vector3(0.11f, 0f, 0f));
            x--;
        }
        else
        {
            Debug.Log("play unable to move sound effect");
        }
    }
    public void MoveXRight()
    {
        if (x < 9)
        {
            cannonBase.transform.localPosition += (new Vector3(0.11f, 0f, 0f));
            x++;
        } else
        {
            Debug.Log("play unable to move sound effect");
        }
    }
    public void MoveYDown()
    {
        if (y > 0)
        {
            this.muzzle.transform.Rotate(new Vector3(muzzleAngle, 0f, 0f));
            y--;
        }
        else
        {
            Debug.Log("play unable to move sound effect");
        }
    }
    public void MoveYUp()
    {
        if (y < 9)
        {
            this.muzzle.transform.Rotate(new Vector3(-muzzleAngle, 0f, 0f));
            y++;
        }
        else
        {
            Debug.Log("play unable to move sound effect");
        }
    }
}

public class CannonControl : MonoBehaviour
{
    private Camera arCamera;
    private Vector3 initialRealWorldPosition;
    private bool isReadyToMove;
    private bool isMovingRight;
    private bool isMovingLeft;

    private bool isReadyToRotate;
    private bool isRotatingUp;
    private bool isRotatingDown;
    private Vector2 newTouchPosition;
    public GameObject muzzle;
    private Cannon cannonPosition;

    // Start is called before the first frame update
    void Start()
    {
        arCamera = Camera.main;
        cannonPosition = new Cannon(5, 0, this.transform.gameObject, muzzle); // start x in the middle
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
                    if (Physics.Raycast(ray, out RaycastHit rayLocation))
                    {
                        Vector3 newRealWorldPosition = rayLocation.point;
                        if (newRealWorldPosition.x < initialRealWorldPosition.x)
                        {
                            Debug.Log("moving left");
                            isMovingLeft = true;
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
                    if (Physics.Raycast(ray, out RaycastHit rayLocation))
                    {
                        Vector3 newRealWorldPosition = rayLocation.point;
                        if (newRealWorldPosition.y < initialRealWorldPosition.y)
                        {
                            Debug.Log("moving up");
                            isRotatingUp = true;
                        }
                        else if (newRealWorldPosition.y > initialRealWorldPosition.y)
                        {
                            Debug.Log("moving down");
                            isRotatingDown = true;
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
                        cannonPosition.MoveXRight();
                        isMovingRight = false;
                    }
                    else if (isMovingLeft)
                    {
                        cannonPosition.MoveXLeft();
                        isMovingLeft = false;
                    }
                    isReadyToMove = false;
                }
                if (isReadyToRotate)
                {
                    Debug.Log("should rotate now");

                    if (isRotatingUp)
                    {
                        cannonPosition.MoveYUp();
                        isRotatingUp = false;
                    }
                    else if (isRotatingDown)
                    {
                        cannonPosition.MoveYDown();
                        isRotatingDown = false;
                    }
                    isReadyToRotate = false;
                }
            }
        }
    }
}
