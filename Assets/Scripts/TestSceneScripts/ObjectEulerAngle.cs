using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEulerAngle : MonoBehaviour
{
    public Vector3 eulerAngle;

    // Update is called once per frame
    void Update()
    {
        eulerAngle = this.gameObject.transform.eulerAngles;
    }
}
