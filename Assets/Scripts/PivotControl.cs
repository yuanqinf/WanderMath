using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotControl : MonoBehaviour
{

    public Vector3 parentLocal;
    public Quaternion parentGlobal;
    public Vector3 newParentLocal;

    public Vector3 local;
    public Quaternion global;

    // Start is called before the first frame update
    void Start()
    {
        parentLocal = transform.parent.transform.eulerAngles;
        parentGlobal = transform.parent.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        parentLocal = transform.parent.transform.eulerAngles;
        parentGlobal = transform.parent.transform.rotation;
        newParentLocal = transform.parent.GetComponent<Transform>().eulerAngles;

        local = this.GetComponent<Transform>().eulerAngles;
        global = transform.rotation;
    }
}
