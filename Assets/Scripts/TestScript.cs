using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public float x;
    public float y;
    public float z;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        x = transform.transform.eulerAngles.x;
        y = transform.transform.eulerAngles.y;
        z = transform.transform.eulerAngles.z;
    }
}
