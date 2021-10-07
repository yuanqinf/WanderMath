using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] startingSubtitleClips;
    // cube making
    [SerializeField]
    private AudioClip selectACubeClip;
    [SerializeField]
    private AudioClip completeCubeClip;
    // starting line
    [SerializeField]
    private AudioClip characterInitClip;

    [SerializeField]
    private AudioClip[] birthdayCardClips;

    [SerializeField]
    private AudioClip[] phase1CubeEasyClips;
    // sound effects
    [SerializeField]
    private AudioClip[] soundEffects;
    private AudioSource audioSource;

    // phase 2 sound effects
    [SerializeField]
    private AudioClip phase2WrongCube;
    [SerializeField]
    private AudioClip phase2PreStartSound;
    [SerializeField]
    private AudioClip phase2StartSound;
    [SerializeField]
    private AudioClip phase2EndSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

// cube setup audio
    public float PlaySetupCubeSubtitleAudio(int num)
    {
        audioSource.PlayOneShot(phase1CubeEasyClips[num]);
        return phase1CubeEasyClips[num].length;
    }

    public float GetPlayCubeEasySubtitleAudio()
    {
        return phase1CubeEasyClips[3].length + phase1CubeEasyClips[4].length;
    }

    public float GetCompleteCubeEasySubtitleAudio()
    {
        return phase1CubeEasyClips[5].length + phase1CubeEasyClips[6].length;
    }

    // Birthday card
    public float PlayBirthdayCardPreClip()
    {
        audioSource.PlayOneShot(birthdayCardClips[0]);
        return birthdayCardClips[0].length;
    }

    public float GetBirthdayCardInitClip()
    {
        return birthdayCardClips[1].length;
    }

    public void PlayBirthdayCardInitClip()
    {
        audioSource.PlayOneShot(birthdayCardClips[1]);
    }

    public float PlayBirthdayCardCompleteClip()
    {
        audioSource.PlayOneShot(birthdayCardClips[2]);
        return birthdayCardClips[2].length;
    }

// Sound effects
    public void PlaySnapSound()
    {
        audioSource.PlayOneShot(soundEffects[0]);
    }

    public void PlaySuccessSound() 
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

    public void PlayCompleteCubeACubeAudio()
    {
        audioSource.PlayOneShot(completeCubeClip, 1);
    }

    public void PlayCompleteSnapAudio()
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


    public float PlayPhase2StartAudio()
    {
        audioSource.PlayOneShot(phase2PreStartSound);

        //StartCoroutine(playSoundAfterTenSeconds(phase2PreStartSound.length + 1));

        return phase2StartSound.length;
    }

    //IEnumerator playSoundAfterTenSeconds(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    audioSource.PlayOneShot(phase2StartSound);
    //}

    public float PlayPhase2WrongCube()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(phase2WrongCube);
        return phase2WrongCube.length;
    }

    public float PlayPhase2EndAudio()
    {
        audioSource.PlayOneShot(phase2EndSound);
        return phase2EndSound.length;
    }
}
