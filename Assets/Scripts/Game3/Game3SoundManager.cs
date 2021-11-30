using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game3SoundManager : MonoBehaviour
{
    public AudioClip balloonSplash;
    public AudioClip cannonShoot;
    public AudioClip cannonRaise;
    public AudioClip cannonLower;
    public AudioClip cannonLeftRight;
    public AudioClip cannonCannotMove;

    private AudioSource audioSource;

    private UiController uiController;

    public AudioClip[] phase0Start;
    public AudioClip[] phase0End;
    private string[] phase0StartSubtitles =
    {
        "Step right up to Finley's Cartesian Carnival! It's water balloon time!",
        "There's something cool behind every door. If you hit the target with your balloon, you'll get it!",
        "First up, the targets are on the X axis. Swipe side to side to aim your cannon at the right number!",
        "Then, press the button to throw it!",
    };
    private string[] phase0EndSubtitles =
    {
        "Awesome, you got them all!",
    };

    public AudioClip[] phase1Start;
    public AudioClip[] phase1End;
    private string[] phase1StartSubtitles =
    {
        "Okay, now, the numbers are on the Y axis. The prizes are even cooler!",
        "Swipe up and down to aim the cannon this time!",
    };
    private string[] phase1EndSubtitles =
    {
        "Sweet! You're doing amazing!",
    };

    public AudioClip[] phase2Start;
    public AudioClip[] phase2End;
    private string[] phase2StartSubtitles =
    {
        "All right, now, let's put it together. These are the best prizes in the whole carnival!",
        "Now, the targets have ordered pairs on them.",
        "The first number is the X axis, so you can aim side to side.",
        "Then, the second number is the Y axis, so aim up and down.",
        "When you've aimed for both numbers in the ordered pair, press the button to throw your balloon!",
    };
    private string[] phase2EndSubtitles =
    {
        "You got all the prizes!",
    };

    public AudioClip[] phase3Start;
    public AudioClip[] phase3End;
    private string[] phase3StartSubtitles =
    {
        "Now, the final challenge... there are a ton of targets, but there's only a prize behind five of them.",
        "We have the ordered pairs of the prize targets here.",
        "So figure out where to aim with the ordered pair, and fire your cannon!",
    };
    private string[] phase3EndSubtitles =
    {
        "Wow, you did amazing! You got all the prizes! Let me just pack these up for you...",
        "All right, thanks for coming to my carnival! See you later!",
    };
    public AudioClip phase3Wrong;
    private string phase3WrongSubtitle = "That's ok, the order of the pair got switched! The first number is the X axis, and the second number is the Y axis!";
    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        uiController = FindObjectOfType<UiController>();
    }

    public float PlayVoiceovers(string phase)
    {
        switch (phase)
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
            case Constants.VoiceOvers.PHASE1End:
                StartCoroutine(PlayVoiceover(phase1End, phase1EndSubtitles));
                break;
            case Constants.VoiceOvers.PHASE2Start:
                StartCoroutine(PlayVoiceover(phase2Start, phase2StartSubtitles));
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

    public float PlayPhase3Wrong()
    {
        if (audioSource.clip != phase3Wrong)
        {
            audioSource.PlayOneShot(phase3Wrong);
            uiController.PlaySubtitles(phase3WrongSubtitle, phase3Wrong.length);
        }
        return phase3Wrong.length;
    }

    public void PlayBalloonSplash()
    {
        audioSource.PlayOneShot(balloonSplash);
    }
    public void PlayCannonShoot()
    {
        audioSource.PlayOneShot(cannonShoot);
    }
    public void PlayCannonRaise()
    {
        audioSource.PlayOneShot(cannonRaise);
    }
    public void PlayCannonLower()
    {
        audioSource.PlayOneShot(cannonLower);
    }
    public void PlayCannonLeftRight()
    {
        audioSource.PlayOneShot(cannonLeftRight);
    }
    public void PlayCannonCannotMove()
    {
        audioSource.PlayOneShot(cannonCannotMove);
    }
}
