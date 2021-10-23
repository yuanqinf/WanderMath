using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLookAt : MonoBehaviour
{
    private Camera arCamera;
    public bool isSkating = false;

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
            this.transform.LookAt(new Vector3(tempTrans.transform.position.x, this.transform.position.y, tempTrans.position.z));
            this.transform.rotation = Quaternion.LookRotation(Camera.main.transform.right * -180f);
        }
        else
        {
            this.transform.LookAt(new Vector3(tempTrans.position.x, this.transform.position.y, tempTrans.position.z));
        }
    }
}
