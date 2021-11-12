using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderUtils
{
    public static void SwitchCubesCollider(bool active)
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag(Constants.Tags.CubeMain))
        {
            SetAllCollidersStatus(active, g);
        }
    }
    public static void SwitchCubeEasyCollider(bool active)
    {
        SetAllCollidersStatus(active, GameObject.FindGameObjectWithTag(Constants.Tags.CubeMain));
    }
    public static void SwitchBirthdayCollider(bool active)
    {
        SetAllCollidersStatus(active, GameObject.FindGameObjectWithTag(Constants.Tags.BirthdayCard));
    }
    public static void SetAllCollidersStatus(bool active, GameObject gameObject)
    {
        foreach (Collider c in gameObject.GetComponentsInChildren<Collider>())
        {
            c.enabled = active;
        }
    }
}
