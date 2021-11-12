using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirthdayCardController : GenericClass
{
    #region Fields
    [SerializeField]
    private GameObject birthdayCard;
    //[SerializeField]
    //private GameObject completedBirthdayCard;
    [SerializeField]
    private float initialDegree = 90f;
    #endregion

    private GameObject spinArrow;
    private GameObject touchDrag;

    private string[] birthdayCardSubtitles =
    {
        "Today is my friend Quinn's birthday!",
        "I'm almost done making a birthday card. But I need to fold it!",
        "Thanks for helping me fold the card!"
    };

    #region Properties
    public GameObject BirthdayCard { get { return birthdayCard; } }
    #endregion

    /// <summary>
    /// Play subtitle and audio for pre and init birthday card
    /// </summary>
    /// <returns></returns>
    public float PlayBirthdayCardInitWithSubtitles()
    {
        var duration = soundManager.PlayBirthdayCardPreClip();
        uiController.PlaySubtitles(birthdayCardSubtitles[0], duration);
        StartCoroutine(PlayBirthdayCardInitSecondWithSubtitles(duration));
        return duration + soundManager.GetBirthdayCardInitClip();
    }

    IEnumerator PlayBirthdayCardInitSecondWithSubtitles(float duration)
    {
        yield return new WaitForSeconds(duration);
        soundManager.PlayBirthdayCardInitClip();
        uiController.PlaySubtitles(birthdayCardSubtitles[1], duration);
    }
    /// <summary>
    /// Play subtitle and audio for completed birthday card
    /// </summary>
    /// <returns></returns>
    private float PlayBirthdayCardCompleteWithSubtitles()
    {
        var duration = soundManager.PlayBirthdayCardCompleteClip();
        characterController.PlayTalkingAnimationWithDuration(duration);
        uiController.PlaySubtitles(birthdayCardSubtitles[2], duration);
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
        var newRot = new Vector3(rot.x - 90, rot.y + 180, rot.z);

        utils.PlaceObjectInSky(birthdayCard, pose.position, Quaternion.Euler(newRot), duration, 0.5f);
        spinArrow = birthdayCard.transform.Find("spinArrow").gameObject;
        touchDrag = birthdayCard.transform.Find("touchDrag").gameObject;
        // TODO: turn off collider and turn on after
        SwitchOffAnimation();
        ColliderUtils.SwitchBirthdayCollider(false);

        StartCoroutine(ShowTutorialAnimation(duration));
    }

    /// <summary>
    /// Allows the card to move accordingly when touched
    /// </summary>
    /// <param name="touchedObject"></param>
    /// <param name="newRealWorldPosition"></param>
    /// <param name="initialRealWorldPosition"></param>
    internal void UpdateCardMovement(GameObject touchedObject, Vector3 newRealWorldPosition, Vector3 initialRealWorldPosition)
    {
        if (newRealWorldPosition.x > initialRealWorldPosition.x)
        {
            Debug.Log("spinArrow.name: " + spinArrow.transform.name);
            Debug.Log("touchDrag.name: " + touchDrag.transform.name);
            touchedObject.transform.Rotate(new Vector3(0, 0, Constants.ROTATION_DEGREE));
        }
        var eulerAngle = touchedObject.transform.localEulerAngles;
        Debug.Log("touched object angle: " + eulerAngle);
        //snap here
        if (touchedObject.transform.localEulerAngles.z > 330f)
        {
            SwitchOffAnimation();
            soundManager.PlaySuccessSound();
            var duration = PlayBirthdayCardCompleteWithSubtitles();
            //var completedBirthdayCard = birthdayCardController.GetCompletedBirthdayCard();
            objectMovementController.ResetGameObject();
            SnapObject(eulerAngle.x, eulerAngle.y, eulerAngle.z, touchedObject, duration);
            gameController.SetGamePhaseWithDelay("phase1", duration);
        }
    }

    private void SnapObject(float x, float y, float z, GameObject gameObject, float duration)
    {
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 150);
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        // move cube to character
        StartCoroutine(utils.LerpMovement(gameObject.transform.position, gameController.GetArCharacterPosition(), duration, gameObject.transform.parent.gameObject));
    }

    IEnumerator ShowTutorialAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
        SwitchOnAnimation();
        ColliderUtils.SwitchBirthdayCollider(true);
        objectMovementController.SetObjectMovementEnabled(true);
    }

    private void SwitchOnAnimation()
    {
        spinArrow.SetActive(true);
        touchDrag.SetActive(true);
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
