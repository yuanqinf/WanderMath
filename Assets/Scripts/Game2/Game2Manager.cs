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

    private void Start()
    {
        dotsManager = FindObjectOfType<DotsManager>();
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
        ActivatePhse1();
    }

    private void ActivatePhse1()
    {
        Debug.Log("phase 1 activated");
    }
}
