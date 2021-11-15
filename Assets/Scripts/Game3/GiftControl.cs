using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftControl : MonoBehaviour
{
    public float m_ThrowForce = 50f;
    public Vector3 targetScale;
    public float speed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == Constants.Tags.Balloon)
        {
            Debug.Log("gift hit by balloon");
            StartCoroutine(WaitUntilTargetCoverOpen());
            StartCoroutine(WaitToSetStatic());
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
        this.transform.GetComponent<Rigidbody>().AddForce(transform.forward * m_ThrowForce);
        while (transform.localScale.magnitude < targetScale.magnitude)
        {
            transform.localScale = Vector3.MoveTowards(this.transform.localScale, targetScale, step);
        }
    }
}
