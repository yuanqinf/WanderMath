using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private float cubeUpDistance = 0.8f;
    private UiController uiController;
    private SoundManager soundManager;
    private ARPlacement arPlacement;

    void Start()
    {
        uiController = FindObjectOfType<UiController>();
        soundManager = FindObjectOfType<SoundManager>();
        arPlacement = FindObjectOfType<ARPlacement>();
    }

    public float StartMovingSubtitleWithAudio()
    {
        var duration = soundManager.GetSelectSubtitleAudioDuration();
        StartCoroutine(SelectSubtitleWithAudio(duration));
        return duration;
    }

    IEnumerator SelectSubtitleWithAudio(float duration)
    {
        yield return new WaitForSeconds(1);
        soundManager.PlaySelectACubeAudio();
        uiController.SetSubtitleActive(true);
        uiController.SetNextSubtitleText();
        yield return new WaitForSeconds(duration);
        yield return new WaitForSeconds(1);
        uiController.SetSubtitleActive(false);
    }

    public void StartSubtitlesWithAudio()
    {
        uiController.SetSubtitleActive(true);
        StartCoroutine(InitialSubtitleWithAudio());
    }

    IEnumerator InitialSubtitleWithAudio()
    {
        var totalLen = soundManager.GetSubtitleAudioClipsLen();
        for (int i = 0; i < totalLen; i++)
        {
            yield return new WaitForSeconds(1);
            uiController.SetInitialSubtitleText(i);
            soundManager.PlayStartingSubtitleAudio(i);
            var audioDuration = soundManager.GetSubtitleAudioDuration(i);

            if (i == totalLen - 1)
            {
                arPlacement.PlaceCubeInSky(audioDuration, cubeUpDistance);
            }
            yield return new WaitForSeconds(audioDuration);
        }
        yield return new WaitForSeconds(1);
        uiController.SetSubtitleActive(false);
    }
}
