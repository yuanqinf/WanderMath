using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericClass : MonoBehaviour
{
    internal HelperUtils utils;
    internal SoundManager soundManager;
    internal UiController uiController;
    internal GameController gameController;

    private void Start()
    {
        utils = FindObjectOfType<HelperUtils>();
        soundManager = FindObjectOfType<SoundManager>();
        uiController = FindObjectOfType<UiController>();
        gameController = FindObjectOfType<GameController>();
    }
}
