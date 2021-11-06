using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftControl : MonoBehaviour
{
    public float m_ThrowForce = 50f;
    public Vector3 targetScale;
    public float speed;
    public bool isChangeScale = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == Constants.Tags.Balloon)
        {
            Debug.Log("gift hit by balloon");
            isChangeScale = true;
            StartCoroutine(WaitUntilTargetCoverOpen());

        }
    }

    IEnumerator WaitUntilTargetCoverOpen()
    {
        yield return new WaitForSeconds(4f);
        this.transform.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {
        if (isChangeScale)
        {
            this.transform.GetComponent<Rigidbody>().useGravity = true;
            this.transform.GetComponent<Rigidbody>().AddForce(transform.forward * m_ThrowForce);
            float step = speed * Time.deltaTime;
            transform.localScale = Vector3.MoveTowards(this.transform.localScale, targetScale, step);
        }
    }
}
