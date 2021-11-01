using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public GameObject effect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "balloon")
        {
            var tempEffect = Instantiate(effect, this.transform);
            Destroy(tempEffect, 1);
        }
    }
}
