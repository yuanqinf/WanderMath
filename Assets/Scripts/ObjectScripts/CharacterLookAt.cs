using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLookAt : MonoBehaviour
{
    private Camera arCamera;
    public bool isSkating = false;
    public GameObject skateBoard;
    public string skateDirection = "faceCamera";
    public Vector3 targetPos = Vector3.zero;

    void Start()
    {
        arCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        var tempTrans = arCamera.transform;
        if (isSkating)
        {
            switch (skateDirection)
            {
                case "faceCamera":
                    Debug.Log("facing  camera");
                    rotateTowards(arCamera.transform.position);
                    break;
                case "facePoint":
                    if (targetPos != Vector3.zero)
                    {
                        rotateTowards(targetPos);
                    }
                    break;
            }
        }
        else
        {
            Debug.Log("looking at camera");
            this.transform.LookAt(new Vector3(tempTrans.position.x, this.transform.position.y, tempTrans.position.z));
        }
    }

    private void rotateTowards(Vector3 to, float turn_speed = 0.1f)
    {

        Quaternion _lookRotation =
            Quaternion.LookRotation((to - this.transform.position).normalized);
        _lookRotation *= Quaternion.Euler(0, 90, 0);
        //over time
        transform.rotation =
            Quaternion.Slerp(this.transform.rotation, _lookRotation, Time.deltaTime * turn_speed);

        //instant
        //transform.rotation = _lookRotation;
    }

    public Vector3 GetSkateBoardPos()
    {
        return this.skateBoard.transform.position;
    }
}
