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
        "To build the ledge, I have 1 cubic foot of concrete.",
        "Tap on the middle of the square, then drag up to give it some height!"
    };
    private string[] phase1EndSubtitles =
    {
        "Rad! It's 1 foot long times 1 foot wide times 1 foot high~ 1 cubic foot!",
        "Okay, time to ollie.",
    };

    public AudioClip[] phase2Start;
    public AudioClip[] phase2Mid;
    public AudioClip[] phase2End;
    private string[] phase2StartSubtitles =
    {
        "Now, I definitely need a ramp for Finley Park.",
        "First, we need to draw another rectangle. You can make it as big as you want!",
    };
    private string[] phase2MidSubtitles =
    {
        "Cool! We have a rectangle.",
        "Nice! That's the area, the length times the width.",
        "I have 2 cubic feet of concrete this time. So let's see how tall it'll get!",
        "To make a ramp, it has to have a slant. So to build it, we'll drag up on one edge."
    };
    private string[] phase2EndSubtitles =
    {
        "Whoa, gnarly! Look how tall the ramp is! ",
        "Since it's a triangular prism, a ramp only uses half the concrete that it would if it was a ledge this tall.",
        "So it's length, times width, times height, times 1/2. Saves a lot of concrete for sure!",
        "Time for testing! This is the best ramp ever! Wow!"
    };

    public AudioClip[] phase3Start;
    public AudioClip[] phase3Mid;
    public AudioClip[] phase3End;
    public AudioClip[] phase3Additional;
    private string[] phase3StartSubtitles =
    {
        "Okay, let's put it all together and build Finley Park!",
        "You can place the ledge and the ramp wherever you want!",
        "There's also 6 cubic feet of extra concrete to make more obstacles.",
    };
    private string[] phase3MidSubtitles =
    {
        "If you want to take something out of the park, double tap it.",
        "Tap on me any time you want me to try something out! Or double tap me to finish the park!",
        "Hm, I think that's too tall for me to jump. Maybe if it had a smaller base?"
    };
    private string[] phase3EndSubtitles =
    {
        "Okay, I think Finley park is complete! Thanks for helping me, and see you on the flip side!",
    };
    private string[] phase3AdditionalSubtitles =
    {
        "Woohoo! Totally rad!",
        "Awesome! I feel like a pro skater.",
        "Aw yeah, did you see that?",
        "Best! Skate park! Ever!"
    };

    public AudioClip skatingSoundEffect;
    public AudioClip goodSoundEffect;
    public AudioClip finishDrawingEffect;
    public AudioClip WrongDrawingAudio;
    private string wrongDrawing = "That's too small for my ledge, I need a square.";
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
            case Constants.VoiceOvers.PHASE2Start:
                StartCoroutine(PlayVoiceover(phase2Start, phase2StartSubtitles));
                break;
            case Constants.VoiceOvers.PHASE2Mid:
                StartCoroutine(PlayVoiceover(phase2Mid, phase2MidSubtitles));
                break;
            case Constants.VoiceOvers.PHASE2End:
                StartCoroutine(PlayVoiceover(phase2End, phase2EndSubtitles));
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

    public void PlaySkatingSoundForTime(float time)
    {
        audioSource.clip = skatingSoundEffect;
        audioSource.Play();
        Invoke("StopAudio", time);
    }
    private void StopAudio()
    {
        GetComponent<AudioSource>().Stop();
    }

    public void PlayGoodSoundEffect()
    {
        audioSource.PlayOneShot(goodSoundEffect);
    }

    public void playFinishDrawing()
    {
        audioSource.PlayOneShot(finishDrawingEffect);
    }

    public void PlayWrongDrawing()
    {
        uiController.PlaySubtitles(wrongDrawing, WrongDrawingAudio.length);
        audioSource.PlayOneShot(WrongDrawingAudio);
    }

    public float playFinishDrawingAudio()
    {
        audioSource.PlayOneShot(finishDrawingAudio);
        return finishDrawingAudio.length;
    }

    public void playWrongCubeLiftAudio()
    {
        audioSource.PlayOneShot(liftWrongVolAudio);
    }
}
