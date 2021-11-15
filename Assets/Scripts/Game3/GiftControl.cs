using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftControl : MonoBehaviour
{
    public float m_ThrowForce = 50f;
    public Vector3 targetScale;
    public float speed;
    private Game3Controller gameController;

    private void Start()
    {
        gameController = FindObjectOfType<Game3Controller>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == Constants.Tags.Balloon)
        {
            Debug.Log("gift hit by balloon");
            StartCoroutine(WaitUntilTargetCoverOpen());
            StartCoroutine(WaitToSetStatic());
            switch (this.name)
            {
                case "car":
                    gameController.UpdatePrizes(0, "<s>(1,5) - Car</s>");
                    break;
                case "guitar":
                    gameController.UpdatePrizes(1, "<s>(3,2) - Guitar</s>");
                    break;
                case "skates":
                    gameController.UpdatePrizes(2, "<s>(4,7) - Skates</s>");
                    break;
                case "camera":
                    gameController.UpdatePrizes(3, "<s>(6,4) - Camera</s>");
                    break;
                case "soccer":
                    gameController.UpdatePrizes(4, "<s>(8,3) - Soccer Ball</s>");
                    break;
            }
        }
    }

    IEnumerator WaitUntilTargetCoverOpen()
    {
        yield return new WaitForSeconds(0.5f);
        ActivateGift();
    }

    IEnumerator WaitToSetStatic()
    {
        yield return new WaitForSeconds(5f);
        this.transform.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void ActivateGift()
    {
        float step = speed * Time.deltaTime;
        this.transform.GetComponent<Rigidbody>().useGravity = true;
        while (transform.localScale.magnitude < targetScale.magnitude)
        {
            this.transform.GetComponent<Rigidbody>().AddForce(transform.forward * m_ThrowForce);
            transform.localScale = Vector3.MoveTowards(this.transform.localScale, targetScale, step);
        }
    }
}
