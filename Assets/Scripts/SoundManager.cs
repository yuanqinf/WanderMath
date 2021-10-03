using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] startingSubtitleClips;
// cube making
    [SerializeField]
    private AudioClip[] setupCubeClipIntro;
    [SerializeField]
    private AudioClip selectACubeClip;
    [SerializeField]
    private AudioClip completeCubeClip;
// starting line
    [SerializeField]
    private AudioClip characterInitClip;

// birthday audio
    [SerializeField]
    private AudioClip birthdayCardPreClip;
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

// cube setup audio
    public float PlaySetubCubeSubtitleAudio(int num)
    {
        audioSource.PlayOneShot(setupCubeClipIntro[num]);
        return setupCubeClipIntro[num].length;
    }

    // Birthday card
    public float PlayBirthdayCardPreClip()
    {
        audioSource.PlayOneShot(birthdayCardPreClip);
        return birthdayCardPreClip.length;
    }

    public float GetBirthdayCardInitClip()
    {
        return birthdayCardInitClip.length;
    }

    public void PlayBirthdayCardInitClip()
    {
        audioSource.PlayOneShot(birthdayCardInitClip);
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
