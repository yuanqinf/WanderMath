using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLookAt : MonoBehaviour
{
    private Camera arCamera;
    private bool isSkating = false;
    public GameObject skateBoard;
    public string skateDirection = "faceCamera";
    public Vector3 skatePos = Vector3.zero;

    void Start()
    {
        arCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        var tempTrans = arCamera.transform;
        if (isSkating && skatePos != Vector3.zero)
        {
            rotateTowards(skatePos);
        }
        else
        {
            this.transform.LookAt(new Vector3(tempTrans.position.x, this.transform.position.y, tempTrans.position.z));
        }
    }

    public void SkateDirection(Vector3 pos)
    {
        this.skatePos = pos;
        this.isSkating = true;
    }
    public void StopSkating()
    {
        this.isSkating = false;
        this.skatePos = Vector3.zero;
    }

    private void rotateTowards(Vector3 to, float turn_speed = 0.1f)
    {

        Quaternion _lookRotation =
            Quaternion.LookRotation((to - this.transform.position).normalized);
        _lookRotation *= Quaternion.Euler(0, 90, 0);
        //over time
        //transform.rotation =
        //    Quaternion.Slerp(this.transform.rotation, _lookRotation, Time.deltaTime * turn_speed);

        //instant
        transform.rotation = _lookRotation;
    }

    public Vector3 GetSkateBoardPos()
    {
        return this.skateBoard.transform.position;
    }
}
