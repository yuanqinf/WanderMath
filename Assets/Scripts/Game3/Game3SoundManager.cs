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

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
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
