using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private UiController uiController;
    private SoundManager soundManager;

    void Start()
    {
        uiController = FindObjectOfType<UiController>();
        soundManager = FindObjectOfType<SoundManager>();
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
            uiController.SetSubtitleText(i);
            soundManager.PlaySubtitleAudio(i);
            yield return new WaitForSeconds(soundManager.GetSubtitleAudioDuration(i));
        }
        yield return new WaitForSeconds(1);
        uiController.SetSubtitleActive(false);
    }
}
