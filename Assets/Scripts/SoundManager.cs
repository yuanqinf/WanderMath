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
// starting line
    [SerializeField]
    private AudioClip characterInitClip;

// birthday audio
    [SerializeField]
    private AudioClip birthdayCardInitClip;
    [SerializeField]
    private AudioClip birthdayCardCompleteClip;

// sound effects
    [SerializeField]
    private AudioClip[] soundEffects;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

// Birthday card
    public float PlayBirthdayCardInitClip()
    {
        audioSource.PlayOneShot(birthdayCardInitClip);
        return birthdayCardInitClip.length;
    }

    public float PlayBirthdayCardCompleteClip()
    {
        audioSource.PlayOneShot(birthdayCardCompleteClip);
        return birthdayCardCompleteClip.length;
    }

// Sound effects
    public void PlaySuccessSound()
    {
        audioSource.PlayOneShot(soundEffects[0]);
    }

    public void PlaySnapSound()
    {
        audioSource.PlayOneShot(soundEffects[1]);
    }

// Play character sound
    public float PlayCharacterInitClip()
    {
        audioSource.PlayOneShot(characterInitClip);
        return characterInitClip.length;
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

    public float PlaySelectACubeAudio()
    {
        audioSource.PlayOneShot(selectACubeClip, 1);
        return selectACubeClip.length;
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
