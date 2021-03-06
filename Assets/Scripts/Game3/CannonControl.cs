using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Cannon
{
    public int x;
    public int y;
    private GameObject cannonBase;
    private GameObject muzzle;
    private float cannonMovement;
    public float muzzleAngle;
    private Game3SoundManager soundManager;

    public Cannon(int x, int y, GameObject cannonBase, GameObject muzzle, Game3SoundManager soundManager)
    {
        this.x = x;
        this.y = y;
        this.cannonBase = cannonBase;
        this.muzzle = muzzle;
        this.soundManager = soundManager;
        this.muzzleAngle = 3.55f;
        this.cannonMovement = 11f;
    }

    public void MoveXLeft()
    {
        if (x > 0)
        {
            cannonBase.transform.localPosition += (new Vector3(cannonMovement, 0f, 0f));
            x--;
            soundManager.PlayCannonLeftRight();
        }
        else
        {
            Debug.Log("play unable to move sound effect");
            soundManager.PlayCannonCannotMove();
        }
    }
    public void MoveXRight()
    {
        if (x < 9)
        {
            cannonBase.transform.localPosition -= (new Vector3(cannonMovement, 0f, 0f));
            x++;
            soundManager.PlayCannonLeftRight();
        }
        else
        {
            Debug.Log("play unable to move sound effect");
            soundManager.PlayCannonCannotMove();
        }
    }
    public void MoveYDown()
    {
        if (y > 0)
        {
            this.muzzle.transform.Rotate(new Vector3(-muzzleAngle, 0f, 0f));
            y--;
            soundManager.PlayCannonLower();
        }
        else
        {
            Debug.Log("play unable to move sound effect");
            soundManager.PlayCannonCannotMove();
        }
    }
    public void MoveYUp()
    {
        if (y < 9)
        {
            this.muzzle.transform.Rotate(new Vector3(muzzleAngle, 0f, 0f));
            y++;
            soundManager.PlayCannonRaise();
        }
        else
        {
            Debug.Log("play unable to move sound effect");
            soundManager.PlayCannonCannotMove();
        }
    }
}

public class CannonControl : MonoBehaviour
{
    private Camera arCamera;
    private Vector3 initialRealWorldPosition;
    private Vector2 initialPosition;
    private bool isReadyToMove;
    private bool isMovingRight;
    private bool isMovingLeft;

    private bool isReadyToRotate;
    private bool isRotatingUp;
    private bool isRotatingDown;
    //private bool isFirstXMove = true;
    //private bool isFirstYMove = true;
    public GameObject muzzle;
    private Cannon cannonPosition;
    private Animator cannonAnimator;
    private Game3Controller game3Controller;
    private Game3SoundManager soundManager;
    [SerializeField]
    private bool is2DMap = true;

    public Animator buttonAnimator;

    // Start is called before the first frame update
    void Start()
    {
        arCamera = Camera.main;
        game3Controller = FindObjectOfType<Game3Controller>();
        soundManager = FindObjectOfType<Game3SoundManager>();
        cannonPosition = new Cannon(0, 0, this.transform.gameObject, muzzle, soundManager); // start x in the middle
        cannonAnimator = muzzle.transform.parent.parent.GetComponent<Animator>();
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
                    Debug.Log("hit object: " + hitObject.transform.name);

                    if (hitObject.transform.tag == Constants.Tags.Cannon)
                    {
                        if (is2DMap)
                        {
                            initialPosition = touch.position;
                        }
                        else
                        {
                            initialRealWorldPosition = hitObject.point;
                        }
                        isReadyToMove = true;
                        Debug.Log("move begin");
                    }

                    if (hitObject.transform.tag == Constants.Tags.Muzzle)
                    {
                        if (is2DMap)
                        {
                            initialPosition = touch.position;
                        }
                        else
                        {
                            initialRealWorldPosition = hitObject.point;
                        }
                        isReadyToRotate = true;
                        Debug.Log("rotate begin");
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (isReadyToMove)
                {
                    Vector2 newTouchPosition = Input.GetTouch(0).position;
                    if (is2DMap)
                    {
                        if (newTouchPosition.x < initialPosition.x)
                        {
                            isMovingLeft = true;
                        } else if (newTouchPosition.x > initialPosition.x)
                        {
                            isMovingRight = true;
                        }
                    }
                    else
                    {
                        Ray ray = arCamera.ScreenPointToRay(newTouchPosition);
                        if (Physics.Raycast(ray, out RaycastHit rayLocation))
                        {
                            Vector3 newRealWorldPosition = rayLocation.point;
                            if (rayLocation.transform.tag == Constants.Tags.Cannon)
                            {
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
                    }
                }
                if (isReadyToRotate)
                {
                    Vector2 newTouchPosition = Input.GetTouch(0).position;
                    if (is2DMap)
                    {
                        if (newTouchPosition.y > initialPosition.y)
                        {
                            isRotatingDown = true;
                            //isRotatingUp = true;
                        }
                        else if (newTouchPosition.y < initialPosition.y)
                        {
                            isRotatingUp = true;
                            //isRotatingDown = true;
                        }
                    }
                    else
                    {
                        Ray ray = arCamera.ScreenPointToRay(newTouchPosition);
                        if (Physics.Raycast(ray, out RaycastHit rayLocation))
                        {
                            Vector3 newRealWorldPosition = rayLocation.point;
                            if (rayLocation.transform.tag == Constants.Tags.Muzzle)
                            {
                                if (newRealWorldPosition.y > initialRealWorldPosition.y)
                                {
                                    Debug.Log("moving down");
                                    isRotatingDown = true;

                                    //Debug.Log("moving up");
                                    //isRotatingUp = true;
                                }
                                else if (newRealWorldPosition.y < initialRealWorldPosition.y)
                                {

                                    Debug.Log("moving up");
                                    isRotatingUp = true;

                                    //Debug.Log("moving down");
                                    //isRotatingDown = true;
                                }
                            }
                        }
                    }
                }

            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (isReadyToMove)
                {
                    if (isMovingRight)
                    {
                        cannonPosition.MoveXRight();
                        isMovingRight = false;
                        game3Controller.SetXMatPosition(cannonPosition.x, cannonPosition.x - 1);
                        //if (isFirstXMove)
                        //{
                            game3Controller.SetXHand(false);
                            //isFirstXMove = false;
                        //}
                    }
                    else if (isMovingLeft)
                    {
                        cannonPosition.MoveXLeft();
                        isMovingLeft = false;
                        game3Controller.SetXMatPosition(cannonPosition.x, cannonPosition.x + 1);
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
                        game3Controller.SetYMatPosition(cannonPosition.y, cannonPosition.y - 1);
                        //if (isFirstYMove)
                        //{
                            game3Controller.SetYHand(false);
                            //isFirstYMove = false;
                        //}
                    }
                    else if (isRotatingDown)
                    {
                        cannonPosition.MoveYDown();
                        isRotatingDown = false;
                        game3Controller.SetYMatPosition(cannonPosition.y, cannonPosition.y + 1);
                    }
                    isReadyToRotate = false;
                }
            }
        }
    }

    public void FireCannon()
    {
        Debug.Log("play cannon animation");
        cannonAnimator.SetTrigger(Constants.Animation.IsShootingTrigger);
        buttonAnimator.SetTrigger("Press");
        StartCoroutine(ResetButtonPos());
        soundManager.PlayCannonShoot();
    }

    public void ResetCannonPosition()
    {
        for (int i = cannonPosition.x; i >=0; i-- )
        {
            cannonPosition.MoveXLeft();
        }
        for (int i = cannonPosition.y; i >= 0; i--)
        {
            cannonPosition.MoveYDown();
        }
        Debug.Log($"cannont positions {cannonPosition.x} and {cannonPosition.x}");
    }

    IEnumerator ResetButtonPos()
    {
        yield return new WaitForSeconds(1f);

        buttonAnimator.SetTrigger("Reset");
    }
}
