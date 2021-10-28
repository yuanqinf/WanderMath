using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeAnimationHelper : MonoBehaviour
{
    public Vector3[] shapeDots;
    public HashSet<Vector3> dotPositions = new HashSet<Vector3>();

    public void AddShapeDots(Vector3 dot)
    {
        dotPositions.Add(dot);
    }
}
