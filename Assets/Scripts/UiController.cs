using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    [SerializeField]
    private Canvas canvasUi;
    [SerializeField]
    private GameObject subtitles;
    [SerializeField]
    private GameObject mouseCursor;

    private string selectCubeSubtitleLine = "Do you think you can make a cube out of that?";
    private string completeCubeSubtitleLine = "No way! That's a cube. It makes sense, see, a cube has six faces.";


    void Start()
    {
        subtitles.SetActive(false);
        mouseCursor.SetActive(false);
    }

    public void SetCursorActive(bool isActive)
    {
        mouseCursor.SetActive(isActive);
    }

    public void SetCursorPosition(Vector2 pos)
    {
        mouseCursor.transform.position = pos;
    }

    #region helper functions
    public void SetSubtitleActive(bool isActive)
    {
        subtitles.SetActive(isActive);
    }

    #endregion

    // part 3: start complete cube subtitles
    public void SetCompleteCubeSubtitles()
    {
        subtitles.GetComponent<TextMeshProUGUI>().text = completeCubeSubtitleLine;
    }

    // part 2: start select cube subtitles
    public void SetNextSubtitleText()
    {
        subtitles.GetComponent<TextMeshProUGUI>().text = selectCubeSubtitleLine;
    }

    /// <summary>
    /// Generic method to play subtitles based on a fixed duration and text
    /// </summary>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    public void PlaySubtitles(string text, float duration)
    {
        subtitles.SetActive(true);
        subtitles.GetComponent<TextMeshProUGUI>().text = text;
        StartCoroutine(StopSubtitles(duration));
    }

    IEnumerator StopSubtitles(float duration)
    {
        yield return new WaitForSeconds(duration);
        subtitles.SetActive(false);
    }
}
