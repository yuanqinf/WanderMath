using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game2SoundManager : MonoBehaviour
{
    public AudioClip[] phase0Start;
    public AudioClip[] phase0End;
    private string[] phase0StartSubtitles =
    {
        "Hey there! I'm gonna build Finley Park, my own personal skate park!",
        "First up~ I need a railing to skate on.",
        "Can you help me? Connect the two dots to build a railing."
    };
    private string[] phase0EndSubtitles =
    {
        "Right on! Let's test it!",
        "Okay, I'll save it for later."
    };

    public AudioClip[] phase1Start;
    public AudioClip[] phase1Mid;
    public AudioClip[] phase1End;
    public AudioSource audioSource;
    private string[] phase1StartSubtitles =
    {
        "Okay, for the next skate obstacle, I want a ledge to jump over. ",
        "Let's draw a square to see where we're going to build it."
    };
    private string[] phase1MidSubtitles =
    {
        "Right on! Let's test it!",
        "Okay, I'll save it for later."
    };
    private string[] phase1EndSubtitles =
    {
        "Right on! Let's test it!",
        "Okay, I'll save it for later."
    };

    public AudioClip goodSoundEffect;
    public AudioClip finishDrawingEffect;
    public AudioClip WrongDrawingAudio;
    public AudioClip finishDrawingAudio;

    public AudioClip liftWrongVolAudio;

    private UiController uiController;

    private void Start()
    {
        uiController = FindObjectOfType<UiController>();
    }

    public float PlayVoiceovers(string phase)
    {
        switch(phase)
        {
            case Constants.VoiceOvers.PHASE0Start:
                StartCoroutine(PlayVoiceover(phase0Start, phase0StartSubtitles));
                break;
            case Constants.VoiceOvers.PHASE0End:
                StartCoroutine(PlayVoiceover(phase0End, phase0EndSubtitles));
                break;
            case Constants.VoiceOvers.PHASE1Start:
                StartCoroutine(PlayVoiceover(phase1Start, phase1StartSubtitles));
                break;
            case Constants.VoiceOvers.PHASE1Mid:
                StartCoroutine(PlayVoiceover(phase1Mid, phase1MidSubtitles));
                break;
            case Constants.VoiceOvers.PHASE1End:
                StartCoroutine(PlayVoiceover(phase1End, phase1EndSubtitles));
                break;
            default:
                break;
        }
        return 1.0f;
    }

    private IEnumerator PlayVoiceover(AudioClip[] audioClips, string[] subtitles)
    {
        yield return null;

        //1.Loop through each AudioClip
        for (int i = 0; i < audioClips.Length; i++)
        {
            //2.Assign current AudioClip to audiosource
            audioSource.clip = audioClips[i];

            //3.Play Audio
            audioSource.Play();
            uiController.PlaySubtitles(subtitles[i], audioClips[i].length - 0.5f);

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
        audioSource.PlayOneShot(WrongDrawingAudio);
    }

    public void playFinishDrawingAudio()
    {
        audioSource.PlayOneShot(finishDrawingAudio);
    }

    public void playWrongCubeLiftAudio()
    {
        audioSource.PlayOneShot(liftWrongVolAudio);
    }
}
