using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirthdayCardController : MonoBehaviour
{
    [SerializeField]
    private GameObject birthdayCard;
    [SerializeField]
    private GameObject completedBirthdayCard;
    [SerializeField]
    private float initialDegree = 90f;

    private GameObject spinArrow;
    private GameObject touchDrag;
    private HelperUtils utils;
    private ObjectMovementController objectMovementController;
    private SoundManager soundManager;
    private UiController uiController;

    private string birthdayCardPre = "Today is my friend Quinn's birthday!";
    private string birthdayCardInit = "I'm almost done making a birthday card. But I need to fold it!";
    private string birthdayCardComplete = "Thanks for helping me fold the card!";

    private void Start()
    {
        utils = FindObjectOfType<HelperUtils>();
        soundManager = FindObjectOfType<SoundManager>();
        uiController = FindObjectOfType<UiController>();
        objectMovementController = FindObjectOfType<ObjectMovementController>();
    }

    /// <summary>
    /// Play subtitle and audio for init birthday card
    /// </summary>
    /// <returns></returns>
    public float PlayBirthdayCardInitWithSubtitles()
    {
        var duration = soundManager.PlayBirthdayCardInitClip();
        uiController.PlaySubtitles(birthdayCardInit, duration);
        return duration;
    }

    public float PlayBirthdayCardCompleteWithSubtitles()
    {
        var duration = soundManager.PlayBirthdayCardCompleteClip();
        uiController.PlaySubtitles(birthdayCardComplete, duration);
        return duration;
    }

    public void InitializeBirthdayCard(Pose pose, float duration)
    {
        Debug.Log("placing birthday card object: " + pose);
        Vector3 rot = pose.rotation.eulerAngles; // may not need this as log shows that it is 0,0,0
        var newRot = new Vector3(rot.x, rot.y + initialDegree, rot.z);

        birthdayCard = utils.PlaceObjectInSky(birthdayCard, pose.position, Quaternion.Euler(newRot), duration, 0.5f);
        spinArrow = birthdayCard.transform.Find("spinArrow").gameObject;
        touchDrag = birthdayCard.transform.Find("touchDrag").gameObject;
        SwitchOffAnimation();

        StartCoroutine(ShowTutorialAnimation(duration));
    }

    IEnumerator ShowTutorialAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
        spinArrow.SetActive(true);
        touchDrag.SetActive(true);
        objectMovementController.SetObjectMovementEnabled(true);
    }

    public GameObject GetCompletedBirthdayCard()
    {
        return completedBirthdayCard;
    }

    public void SwitchOffAnimation()
    {
        spinArrow.SetActive(false);
        touchDrag.SetActive(false);
    }

    public float GetInitialDegree()
    {
        return initialDegree;
    }
}
