using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetControl : MonoBehaviour
{
    public GameObject effect;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            this.GetComponent<Animator>().SetTrigger(Constants.Animation.IsShotTrigger);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == Constants.Tags.Balloon)
        {
            this.GetComponent<Animator>().SetTrigger(Constants.Animation.IsShotTrigger);
            var tempEffect = Instantiate(effect, this.transform);
            Destroy(tempEffect, 1);
        }
    }
}
