using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotControl : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

        //gameObject.transform.eulerAngles = new Vector3(-80,0,0);
        //gameObject.GetComponent<Rigidbody>().freezeRotation = true;
        //Debug.Log("rotate: " + (gameObject.GetComponent<Transform>().rotation.eulerAngles.x));
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
