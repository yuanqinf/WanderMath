using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
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
    private AudioClip[] phase2InitClips;
    [SerializeField]
    private AudioClip[] phase2CompleteClips;
    [SerializeField]
    private AudioClip[] phase2WrongCubeClips;

    [SerializeField]
    private AudioClip[] phase3Clips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

// phase 1 cube setup audio
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

// phase 2 cube setup audio
    public float PlayInitCubesSubtitleAudio(int num)
    {
        audioSource.PlayOneShot(phase2InitClips[num]);
        return phase2InitClips[num].length;
    }

    public float PlayCompleteCubesSubtitleAudio(int num)
    {
        audioSource.PlayOneShot(phase2CompleteClips[num]);
        return phase2CompleteClips[num].length;
    }

    public float PlayWrongCubesSubtitleAudio(int num)
    {
        audioSource.PlayOneShot(phase2WrongCubeClips[num]);
        return phase2WrongCubeClips[num].length;
    }

    public float GetInitPlayCubesSubtitleAudio()
    {
        return phase2InitClips[0].length + phase2InitClips[1].length;
    }

    public float GetCompletePlayCubesSubtitleAudio()
    {
        return phase2CompleteClips[0].length + phase2CompleteClips[1].length;
    }

    // phase 3 audio
    public float PlayPhase3InitShapesSubtitleAudio(int num)
    {
        audioSource.PlayOneShot(phase3Clips[num]);
        return phase3Clips[num].length;
    }

    public float GetPhase3InitShapes(int num)
    {
        return phase3Clips[num].length;
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
}
