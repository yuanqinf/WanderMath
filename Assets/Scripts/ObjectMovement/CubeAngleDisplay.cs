using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAngleDisplay : MonoBehaviour
{
    public Vector3 localAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        localAngle = transform.localEulerAngles;
    }
}
