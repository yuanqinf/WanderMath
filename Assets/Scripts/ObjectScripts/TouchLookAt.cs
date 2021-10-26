using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchLookAt : MonoBehaviour
{
    private Camera arCamera;

    void Start()
    {
        arCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        var tempTrans = arCamera.transform;
        Debug.Log("touchlookat called");

        this.transform.LookAt(tempTrans);
        this.transform.rotation *= Quaternion.Euler(0, -90, 0);
    }
}
