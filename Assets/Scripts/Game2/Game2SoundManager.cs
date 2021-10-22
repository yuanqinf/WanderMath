using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game2SoundManager : MonoBehaviour
{

    public AudioClip[] phase1Start;
    public AudioSource audioSource;

    public AudioClip goodSoundEffect;
    public AudioClip finishDrawingEffect;
    public AudioClip WrongDrawingEffect;

    public IEnumerator PlayPhase1StartAudio()
    {
        yield return null;

        //1.Loop through each AudioClip
        for (int i = 0; i < phase1Start.Length; i++)
        {
            //2.Assign current AudioClip to audiosource
            audioSource.clip = phase1Start[i];

            //3.Play Audio
            audioSource.Play();

            //4.Wait for it to finish playing
            while (audioSource.isPlaying)
            {
                yield return null;
            }

            //5. Go back to #2 and play the next audio in the adClips array
        }
    }

    public void PlayGoodSoundEffect()
    {
        audioSource.PlayOneShot(goodSoundEffect);
    }

    public void playFinishDrawing()
    {
        audioSource.PlayOneShot(finishDrawingEffect);
    }

    public void playWrongDrawing()
    {
        audioSource.PlayOneShot(WrongDrawingEffect);
    }
}
