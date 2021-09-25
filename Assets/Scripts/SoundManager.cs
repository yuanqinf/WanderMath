using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] startingSubtitleClips;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public int GetSubtitleAudioClipsLen()
    {
        return startingSubtitleClips.Length;
    }

    public float GetSubtitleAudioDuration(int num)
    {
        return startingSubtitleClips[num].length;
    }

    public void PlaySubtitleAudio(int num)
    {
        audioSource.PlayOneShot(startingSubtitleClips[num], 1);
    }
}
