using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
public class ARExperienceManager : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnInitialized;
    [SerializeField]
    private UnityEvent OnRestarted;

    private ARPlaneManager arPlaneManager;

    private bool Initialized { get; set; }

    private void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arPlaneManager.planesChanged += PlanesChanged;

        #if UNITY_EDITOR
        OnInitialized?.Invoke();
            Initialized = true;
            arPlaneManager.enabled = false;
        #endif
    }

    void PlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (!Initialized)
        {
            Activate();
        }
    }

    private void Activate()
    {
        ARDebugManager.Instance.LogInfo("Activate experience");
        OnInitialized?.Invoke();
        Initialized = true;
        arPlaneManager.enabled = false;
    }

    public void Restart()
    {
        ARDebugManager.Instance.LogInfo("Restart experience");
        OnRestarted?.Invoke();
        Initialized = false;
        arPlaneManager.enabled = true;
    }
}
