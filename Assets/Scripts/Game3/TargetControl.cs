using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetControl : MonoBehaviour
{
    public GameObject effect;
    private CharacterController characterController;

    private void Start()
    {
        characterController = FindObjectOfType<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == Constants.Tags.Balloon)
        {
            Debug.Log("hit by balloon");
            this.GetComponent<Animator>().SetTrigger(Constants.Animation.IsShotTrigger);
            var tempEffect = Instantiate(effect, this.transform);
            characterController.ShakeWater();
            Destroy(tempEffect, 1);
        }
    }
}
