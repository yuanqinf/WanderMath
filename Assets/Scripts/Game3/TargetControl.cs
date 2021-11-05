using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetControl : MonoBehaviour
{
    public GameObject effect;
    private CharacterController characterController;
    private Game3Controller game3Controller;

    private void Start()
    {
        characterController = FindObjectOfType<CharacterController>();
        game3Controller = FindObjectOfType<Game3Controller>();
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
            this.GetComponent<Collider>().enabled = false;
            StartCoroutine(IncreaseTargetWithDelay());
        }
    }
    IEnumerator IncreaseTargetWithDelay()
    {
        yield return new WaitForSeconds(1.5f);
        game3Controller.IncreaseTargetHit();
    }
}
