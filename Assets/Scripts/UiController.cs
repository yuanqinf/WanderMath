using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    [SerializeField]
    private Canvas canvasUi;
    [SerializeField]
    private GameObject preStartText;
    [SerializeField]
    private GameObject subtitles;
    private string[] subtitleLines = {
        "Oh, hi! I'm Quinnchilla, but you can call me Quinn. Nice to meet you!",
        "I was just wrapping some presents for my friends.",
        "Well, I was trying to, anyway.",
        "I need some boxes, but I only have these flat pieces.",
        "Can you help me make a cube, so I can finish wrapping the presents?",
    };

    private string selectCubeSubtitleLine = "Do you think you can make a cube out of that?";


    void Start()
    {
        preStartText.SetActive(false);
        subtitles.SetActive(false);
    }

    public void SetPreStartTextActive(bool isActive)
    {
        preStartText.SetActive(isActive);
        //Debug.Log(preStartText.GetComponent<TextMeshProUGUI>().text);
    }

    public void SetSubtitleActive(bool isActive)
    {
        subtitles.SetActive(isActive);
    }

    public void SetInitialSubtitleText(int num)
    {
        subtitles.GetComponent<TextMeshProUGUI>().text = subtitleLines[num];
    }

    public void SetNextSubtitleText()
    {
        subtitles.GetComponent<TextMeshProUGUI>().text = selectCubeSubtitleLine;
    }
}
