using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotControl : MonoBehaviour
{

    [System.NonSerialized]
    private int offset = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(canSnap && transform.localRotation.eulerAngles.x > 80)
        //{
        //    transform.Rotate((90 - transform.localRotation.eulerAngles.x), 0, 0, Space.Self);
        //    //canSnap = false;
        //}

        //if (canSnap && transform.localRotation.eulerAngles.x < -90)
        //{
        //    transform.Rotate((-90 - transform.localRotation.eulerAngles.x), 0, 0, Space.Self);
        //    //canSnap = false;
        //}
    }
}
