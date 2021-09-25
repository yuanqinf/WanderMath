using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] startingSubtitleClips;
    [SerializeField]
    private AudioClip selectACubeClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    #region Starting subtitles
    public int GetSubtitleAudioClipsLen()
    {
        return startingSubtitleClips.Length;
    }

    public float GetSubtitleAudioDuration(int num)
    {
        return startingSubtitleClips[num].length;
    }

    public void PlayStartingSubtitleAudio(int num)
    {
        audioSource.PlayOneShot(startingSubtitleClips[num], 1);
    }
    #endregion
    #region Select subtitles
    public float GetSelectSubtitleAudioDuration()
    {
        return selectACubeClip.length;
    }

    public void PlaySelectACubeAudio()
    {
        audioSource.PlayOneShot(selectACubeClip, 1);
    }
    #endregion
}
