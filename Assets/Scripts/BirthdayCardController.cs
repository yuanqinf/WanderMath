using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirthdayCardController : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private GameObject birthdayCard;
    [SerializeField]
    private GameObject completedBirthdayCard;
    [SerializeField]
    private float initialDegree = 90f;
    #endregion

    private GameObject spinArrow;
    private GameObject touchDrag;
    private HelperUtils utils;
    private ObjectMovementController objectMovementController;
    private GameController gameController;
    private SoundManager soundManager;
    private UiController uiController;

    private string birthdayCardPre = "Today is my friend Quinn's birthday!";
    private string birthdayCardInit = "I'm almost done making a birthday card. But I need to fold it!";
    private string birthdayCardComplete = "Thanks for helping me fold the card!";

    #region Properties
    public GameObject BirthdayCard { get { return birthdayCard; } }
    #endregion

    private void Start()
    {
        utils = FindObjectOfType<HelperUtils>();
        soundManager = FindObjectOfType<SoundManager>();
        uiController = FindObjectOfType<UiController>();
        objectMovementController = FindObjectOfType<ObjectMovementController>();
        gameController = FindObjectOfType<GameController>();
    }

    /// <summary>
    /// Play subtitle and audio for pre and init birthday card
    /// </summary>
    /// <returns></returns>
    public float PlayBirthdayCardInitWithSubtitles()
    {
        var duration = soundManager.PlayBirthdayCardPreClip();
        uiController.PlaySubtitles(birthdayCardPre, duration);
        StartCoroutine(PlayBirthdayCardInitSecondWithSubtitles(duration));
        return duration + soundManager.GetBirthdayCardInitClip();
    }

    IEnumerator PlayBirthdayCardInitSecondWithSubtitles(float duration)
    {
        yield return new WaitForSeconds(duration);
        soundManager.PlayBirthdayCardInitClip();
        uiController.PlaySubtitles(birthdayCardInit, duration);
    }
    /// <summary>
    /// Play subtitle and audio for completed birthday card
    /// </summary>
    /// <returns></returns>
    private float PlayBirthdayCardCompleteWithSubtitles()
    {
        var duration = soundManager.PlayBirthdayCardCompleteClip();
        uiController.PlaySubtitles(birthdayCardComplete, duration);
        return duration;
    }

    /// <summary>
    /// initialize birthdaycard based on location and duration
    /// </summary>
    /// <param name="pose"></param>
    /// <param name="duration"></param>
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

    internal void UpdateCardMovement(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        // in charge of moving
        if (newRealWorldPosition.x < initialRealWorldPosition.x)
        {
            touchedObject.transform.Rotate(new Vector3(5f, 0, 0));
            SwitchOffAnimation();
        }
        // in charge of snapping logic
        var eulerAngle = touchedObject.transform.eulerAngles;
        //Debug.Log("touched object angle: " + eulerAngle);
        if (eulerAngle.y > 50 + 90f)
        {
            soundManager.PlaySuccessSound();
            var duration = PlayBirthdayCardCompleteWithSubtitles();
            //var completedBirthdayCard = birthdayCardController.GetCompletedBirthdayCard();
            SnapObject(eulerAngle.x, 60 + 90f, eulerAngle.z, touchedObject, duration);
            gameController.SetGamePhaseWithDelay("phase1", duration);
        }
    }

    private void SnapObject(float x, float y, float z, GameObject gameObject, float duration)
    {
        gameObject.transform.eulerAngles = new Vector3(x, y, z);
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        // move cube to character
        StartCoroutine(utils.LerpMovement(gameObject.transform.position, gameController.GetArCharacterPosition(), duration, gameObject.transform.parent.gameObject));
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

    private void SwitchOffAnimation()
    {
        spinArrow.SetActive(false);
        touchDrag.SetActive(false);
    }

    public float GetInitialDegree()
    {
        return initialDegree;
    }
}
