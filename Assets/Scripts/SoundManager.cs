using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] startingSubtitleClips;
    [SerializeField]
    private AudioClip selectACubeClip;
    [SerializeField]
    private AudioClip completeCubeClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    #region part 3: complete cube audio
    public float GetCompleteCubeSubtitleAudioDuration()
    {
        return completeCubeClip.length;
    }

    public void PlayCompleteCubeACubeAudio()
    {
        audioSource.PlayOneShot(completeCubeClip, 1);
    }
    #endregion

    #region part 2: start select cube audio
    public float GetSelectSubtitleAudioDuration()
    {
        return selectACubeClip.length;
    }

    public void PlaySelectACubeAudio()
    {
        audioSource.PlayOneShot(selectACubeClip, 1);
    }
    #endregion

    #region part 1: starting initial audio
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
}
