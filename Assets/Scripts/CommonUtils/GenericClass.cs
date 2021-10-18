using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericClass : MonoBehaviour
{
    internal HelperUtils utils;
    internal SoundManager soundManager;
    internal UiController uiController;
    internal CharacterController characterController;
    internal GameController gameController;
    internal ObjectMovementController objectMovementController;
    internal CubeRotateControl cubeRotateControl;
    internal ShapesController shapesController;

    private void Start()
    {
        utils = FindObjectOfType<HelperUtils>();
        soundManager = FindObjectOfType<SoundManager>();
        uiController = FindObjectOfType<UiController>();
        characterController = FindObjectOfType<CharacterController>();
        gameController = FindObjectOfType<GameController>();
        objectMovementController = FindObjectOfType<ObjectMovementController>();
        cubeRotateControl = FindObjectOfType<CubeRotateControl>();
        shapesController = FindObjectOfType<ShapesController>();
    }
}
