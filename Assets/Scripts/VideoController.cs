using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    private void Awake()
    {
        this.GetComponent<VideoPlayer>().Prepare();
        this.GetComponent<VideoPlayer>().loopPointReached += CheckOver;
    }

    private void CheckOver(VideoPlayer vp)
    {
        SceneManager.LoadScene(Constants.Scenes.MainMenu);
    }
}
