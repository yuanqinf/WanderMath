using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkating : MonoBehaviour
{
    public bool isSkating = false;
    // Update is called once per frame
    void Update()
    {
        if (isSkating)
        {
            Debug.Log("camera transform: " + Camera.main.transform);
            //gameObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform);
            this.transform.Rotate(0, 90, 0, Space.Self);
            //this.transform(Camera.main.transform);
            isSkating = false;
        }
    }
}
