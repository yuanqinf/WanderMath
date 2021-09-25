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
        "Do you think you can make a cube out of that?",
        "No way! That's a cube. It makes sense, see, a cube has six faces.",
        "Oh, I guess that one doesn't make a cube. There's a gap, so it won't cover the whole present.",
        "I totally thought that would make a cube! It has six faces, but I guess they're not arranged the right way.",
        "That's awesome, thank you! Since you're a folding master, do you want to help me wrap a few more presents?",
        "No worries, I bet I'll find something cool to make out of it. Wanna see if we can find one that makes a cube?"
    };

    public void setPreStartText(bool isActive)
    {
        preStartText.SetActive(isActive);
        //Debug.Log(preStartText.GetComponent<TextMeshProUGUI>().text);
    }

    public void startSubtitles(bool isActive)
    {
        //subtitles.SetActive(isActive);
    }
}
