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
        "Okay, it's 1 foot by 1 foot. So, the area is 1 square foot. Sweet!",
        "To build the ledge, I have 1 cubic foot of concrete.",
        "Press on the middle of the square, then drag up to give it some height!"
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
        "I have 1 cubic foot of concrete this time. So let's see how tall it'll get!",
        "To make a ramp, it has to have a slant. So to build it, we'll drag up on one edge."
    };
    private string[] phase2EndSubtitles =
    {
        "Whoa, gnarly! Look how tall the ramp is! ",
        "Since it's a triangular prism, a ramp only uses half the concrete that it would if it was a ledge this tall.",
        "So it's length, times width, times height, times 1/2. Saves a lot of concrete for sure!",
        "Time for testing!"
    };

    public AudioClip phase2BestRampAudio;
    private string phase2BestRampSubtitle = "This is the best ramp ever! Wow!";

    public AudioClip[] phase3Start;
    public AudioClip[] phase3End;
    public AudioClip[] phase3Additional;
    private string[] phase3StartSubtitles =
    {
        "Okay, let's put it all together and build Finley Park!",
        "You can place the ledge and the ramp wherever you want!",
        "There's also 6 cubic feet of extra concrete to make more obstacles.",
        "Connect dots to make rectangles. Then, drag up the middle to make a ledge, or the edge to make a ramp!"
    };
    private string[] phase3EndSubtitles =
    {
        "Woohoo! Totally rad!"
    };
    private string[] phase3AdditionalSubtitles =
    {
        "Awesome! I feel like a pro skater.",
        "Okay, I think Finley park is complete! Thanks for helping me, and see you on the flip side!"
    };

    public AudioClip skatingSoundEffect;
    public AudioClip goodSoundEffect;
    public AudioClip finishDrawingEffect;
    public AudioClip WrongDrawingAudio;
    private string wrongDrawing = "That's too small for my ledge, I need a square.";
    public AudioClip WrongDiagonalAudio;
    private string wrongDiagonal = "That's a diagonal line! We don't need a diagonal line to draw a rectangle.";
    public AudioClip WrongLinesAudio;
    private string wrong4Lines = "Make sure to draw your rectangle with four lines! Let's try again.";
    public AudioClip phase1PerfectAudio;
    private string phase1PerfectLine = "It's perfect. I'll save it for later.";

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
            case Constants.VoiceOvers.PHASE3Start:
                StartCoroutine(PlayVoiceover(phase3Start, phase3StartSubtitles));
                break;
            case Constants.VoiceOvers.PHASE3End:
                StartCoroutine(PlayVoiceover(phase3End, phase3EndSubtitles));
                break;
            case Constants.VoiceOvers.PHASE3Additional:
                StartCoroutine(PlayVoiceover(phase3Additional, phase3AdditionalSubtitles));
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
        if (!audioSource.isPlaying) 
        {
            uiController.PlaySubtitles(wrongDrawing, WrongDrawingAudio.length);
            audioSource.PlayOneShot(WrongDrawingAudio);
        }
    }

    public void PlayWrongDiagonal()
    {
        if (!audioSource.isPlaying)
        {
            uiController.PlaySubtitles(wrongDiagonal, 5.5f);
            audioSource.PlayOneShot(WrongDiagonalAudio);
        }
    }
    public void PlayWrongLines()
    {
        if (audioSource.clip == WrongLinesAudio && audioSource.isPlaying)
        {
            return;
        } else
        {
            uiController.PlaySubtitles(wrong4Lines, 4.2f);
            audioSource.PlayOneShot(WrongLinesAudio);
        }
    }

    // used to play at the end
    public void PlayBestRampEver()
    {
        uiController.PlaySubtitles(phase2BestRampSubtitle, 3.9f);
        audioSource.PlayOneShot(phase2BestRampAudio);
    }

    public void PlayPhase1Perfect()
    {
        uiController.PlaySubtitles(phase1PerfectLine, 3f);
        audioSource.PlayOneShot(phase1PerfectAudio);
    }

    public void PlayWrongCubeLiftAudio()
    {
        audioSource.PlayOneShot(liftWrongVolAudio);
    }
}
