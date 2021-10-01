using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirthdayCardController : MonoBehaviour
{
    [SerializeField]
    private GameObject birthdayCard;

    private GameObject spinArrow;
    private GameObject touchDrag;
    private HelperUtils utils;

    private void Start()
    {
        utils = FindObjectOfType<HelperUtils>();
    }

    public void InitializeBirthdayCard(Pose pose)
    {
        Debug.Log("placing birthday card object: " + pose);
        Vector3 rot = pose.rotation.eulerAngles; // may not need this as log shows that it is 0,0,0
        var newRot = new Vector3(rot.x, rot.y + 90, rot.z);

        var duration = 3.0f;

        birthdayCard = utils.PlaceObjectInSky(birthdayCard, pose.position, Quaternion.Euler(newRot), duration, 0.5f);
        spinArrow = birthdayCard.transform.Find("spinArrow").gameObject;
        touchDrag = birthdayCard.transform.Find("touchDrag").gameObject;
        spinArrow.SetActive(false);
        touchDrag.SetActive(false);

        StartCoroutine(ShowTutorialAnimation(duration));
    }

    IEnumerator ShowTutorialAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
        spinArrow.SetActive(true);
        touchDrag.SetActive(true);
    }
}
