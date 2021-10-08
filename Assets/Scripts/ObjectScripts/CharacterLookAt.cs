using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLookAt : MonoBehaviour
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

        this.transform.LookAt(tempTrans);
    }
}
