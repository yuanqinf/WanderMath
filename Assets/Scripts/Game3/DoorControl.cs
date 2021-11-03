using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public GameObject effect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "balloon")
        {
            var tempEffect = Instantiate(effect, this.transform);
            Destroy(tempEffect, 1);
        }

    }
}
