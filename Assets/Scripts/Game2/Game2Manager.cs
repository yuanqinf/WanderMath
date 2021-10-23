using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game2Manager : Singleton<Game2Manager>
{
    [SerializeField]
    private GameObject phase0Object;
    [SerializeField]
    private GameObject phase1Object;
    private DotsManager dotsManager;
    private CharacterController characterController;
    private string gamePhase = "waiting";

    private void Start()
    {
        dotsManager = FindObjectOfType<DotsManager>();
        characterController = FindObjectOfType<CharacterController>();
    }

    private void Update()
    {
        switch (gamePhase)
        {
            case Constants.GamePhase.PHASE0:
                StartPhase0();
                gamePhase = "waiting";
                break;
            case Constants.GamePhase.PHASE1:
                gamePhase = "waiting";
                StartPhase1();
                break;
            case Constants.GamePhase.PHASE2:
                gamePhase = "waiting";
                break;
            case Constants.GamePhase.PHASE3:
                gamePhase = "waiting";
                break;
            default:
                break;
        }
    }

    public void SetGamePhase(string gamePhase)
    {
        this.gamePhase = gamePhase;
    }

    private void StartPhase0()
    {
        characterController.InitCharacterSkatingAndAudio(dotsManager.placementPose);
        dotsManager.InstantiatePhase0Dots();
    }

    public void EndPhase0()
    {
        // replace points of dots with prefab
        Vector3 midPoint = new Vector3(0, 0, 0);
        foreach(GameObject dot in dotsManager.dots) {
            midPoint += dot.transform.position;
        }
        midPoint /= dotsManager.dots.Count;
        Instantiate(phase0Object, midPoint, dotsManager.dots[0].transform.rotation);
        
        dotsManager.ClearDots();
        // TODO: add animation & sound effect
        gamePhase = Constants.GamePhase.PHASE1;
    }

    private void StartPhase1()
    {
        dotsManager.InstantiatePhase1Dots();
    }
}
