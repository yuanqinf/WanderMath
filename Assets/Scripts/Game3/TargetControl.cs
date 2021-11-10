using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetControl : MonoBehaviour
{
    public GameObject effect;
    private CharacterController characterController;
    private Game3Controller game3Controller;
    private Game3SoundManager game3SoundMG;

    private void Start()
    {
        characterController = FindObjectOfType<CharacterController>();
        game3Controller = FindObjectOfType<Game3Controller>();
        game3SoundMG = FindObjectOfType<Game3SoundManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == Constants.Tags.Balloon)
        {
            Debug.Log("hit by balloon");
            this.GetComponent<SphereCollider>().enabled = false;
            this.GetComponent<Animator>().SetTrigger(Constants.Animation.IsShotTrigger);
            var tempEffect = Instantiate(effect, this.transform);
            characterController.PlayShakeWater();
            Destroy(tempEffect, 1);
            this.GetComponent<Collider>().enabled = false;
            StartCoroutine(IncreaseTargetWithDelay());
            game3SoundMG.PlayBalloonSplash();
        }
    }
    IEnumerator IncreaseTargetWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        game3Controller.IncreaseTargetHit();
    }
}
