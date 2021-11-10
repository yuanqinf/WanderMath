using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeNetLookAt : MonoBehaviour
{
    private Camera arCamera;

    // Start is called before the first frame update
    void Start()
    {
        arCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        var tempTrans = arCamera.transform;

        this.transform.LookAt(new Vector3(tempTrans.position.x, this.transform.position.y, tempTrans.position.z));
    }
}
