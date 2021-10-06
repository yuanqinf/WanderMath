using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesController : GenericClass
{
    [SerializeField]
    private GameObject cuboid;
    [SerializeField]
    private GameObject pyramid;
    [SerializeField]
    private GameObject hexagon;

    internal void StartPhase3(Pose placementPose)
    {
        //var startAudioLen = soundManager.PlayPhase2StartAudio();
        var startAudioLen = 2.0f;

        Vector3 cuboidPos = placementPose.position;
        Vector3 pyramidPos = placementPose.position + new Vector3(0f, 0f, 0.8f);
        Vector3 hexPos = placementPose.position + new Vector3(0.92f, 0f, 0.04f);

        cuboid = utils.PlaceObjectInSky(cuboid, cuboidPos, placementPose.rotation, startAudioLen, 0.5f);
        pyramid = utils.PlaceObjectInSky(pyramid, pyramidPos, placementPose.rotation, startAudioLen, 0.5f);
        hexagon = utils.PlaceObjectInSky(hexagon, hexPos, placementPose.rotation, startAudioLen, 0.5f);
    }
}
