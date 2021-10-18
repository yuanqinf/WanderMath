using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARAnchorManager))]
public class ARDrawManager : Singleton<ARDrawManager>
{
    [SerializeField]
    private float distanceFromCamera = 0.3f; // fix 1 axis
    [SerializeField]
    private Material defaultColorMaterial;
    [SerializeField]
    private int cornerVertices = 5;
    [SerializeField]
    private int endCapVeritices = 5;

    [Header("Tolerance Options")]
    [SerializeField]
    private bool allowSimplification = false;
    [SerializeField]
    private float tolerance = 0.001f;
    [SerializeField]
    private float applySimplifyAfterPoints = 20.0f;
    [SerializeField, Range(0, 1.0f)]
    private float minDistanceBeforeNewPoint = 0.01f;

    [SerializeField]
    private UnityEvent OnDraw;
    [SerializeField]
    private ARAnchorManager anchorManager;
    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private float lineWidth = 0.05f;
    private Color randomStartColor = Color.white;
    private Color randomEndColor = Color.white;

    private LineRenderer prevLineRender;
    private LineRenderer currentLineRender;

    private List<ARAnchor> anchors = new List<ARAnchor>();
    private List<LineRenderer> lines = new List<LineRenderer>();

    private int positionCount = 2;
    private Vector3 prevPointDistance = Vector3.zero;

    private bool CanDraw { get; set; }

    void Update()
    {
        #if !UNITY_EDITOR
        if (Input.touchCount > 0)
            DrawOnTouch();
        #else
        if (Input.GetMouseButton(0))
            DrawOnMouse();
        else
        {
            prevLineRender = null;
        }
        #endif
    }

    public void AllowDraw(bool isAllow)
    {
        CanDraw = isAllow;
    }

    private void SetLineSettings(LineRenderer currentLineRenderer)
    {
        currentLineRender.startWidth = lineWidth;
        currentLineRender.endWidth = lineWidth;
        currentLineRender.numCornerVertices = cornerVertices;
        currentLineRender.numCapVertices = endCapVeritices;
        if (allowSimplification) currentLineRender.Simplify(tolerance);
        currentLineRender.startColor = randomStartColor;
        currentLineRender.endColor = randomEndColor;
    }


    void DrawOnTouch()
    {
        if (!CanDraw) return;

        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = arCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, distanceFromCamera));

        ARDebugManager.Instance.LogInfo($"{touch.fingerId}");

        if (touch.phase == TouchPhase.Began)
        {
            OnDraw?.Invoke();

            // anchor ensure position is correct in real world
            ARAnchor anchor = anchorManager.AddAnchor(new Pose(touchPosition, Quaternion.identity));
            if (anchor == null)
                Debug.LogError("Error creating reference point");
            else
            {
                anchors.Add(anchor);
                ARDebugManager.Instance.LogInfo($"Anchor created & total of {anchors.Count} anchor(s)");
            }
            AddNewLineRenderer(anchor, touchPosition);
        }
        else
        {
            UpdateLine(touchPosition);
        }
    }

    private void UpdateLine(Vector3 touchPosition)
    {
        if (prevPointDistance == null) prevPointDistance = touchPosition;
        // create new point if above threshold of minDistance
        if (prevPointDistance != null && Mathf.Abs(Vector3.Distance(prevPointDistance, touchPosition)) >= minDistanceBeforeNewPoint)
        {
            prevPointDistance = touchPosition;
            AddPoint(prevPointDistance);
        }
    }

    private void AddPoint(Vector3 position)
    {
        positionCount++;
        currentLineRender.positionCount = positionCount;
        // iundex 0 positionCount must be -1
        currentLineRender.SetPosition(positionCount - 1, position);
        if (currentLineRender.positionCount % applySimplifyAfterPoints == 0 && allowSimplification)
        {
            currentLineRender.Simplify(tolerance);
        }
    }

    private void AddNewLineRenderer(ARAnchor anchor, Vector3 touchPosition)
    {
        positionCount = 2;
        GameObject go = new GameObject($"LineRenderer_{lines.Count}");
        go.transform.parent = anchor?.transform ?? transform; // use anchor transform instead of transform itself
        go.transform.position = touchPosition;
        go.tag = "Line";
        LineRenderer goLineRenderer = go.AddComponent<LineRenderer>();
        goLineRenderer.startWidth = lineWidth;
        goLineRenderer.endWidth = lineWidth;
        goLineRenderer.material = defaultColorMaterial;
        goLineRenderer.useWorldSpace = true;
        goLineRenderer.positionCount = positionCount;
        goLineRenderer.numCapVertices = 90;
        goLineRenderer.SetPosition(0, touchPosition);
        goLineRenderer.SetPosition(1, touchPosition);

        SetLineSettings(goLineRenderer);

        currentLineRender = goLineRenderer;
        prevLineRender = currentLineRender;
        lines.Add(goLineRenderer);

        ARDebugManager.Instance.LogInfo($"New line renderer created");
    }

    void DrawOnMouse()
    {
        if (!CanDraw) return;

        Vector3 mousePosition = arCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCamera));

        if (Input.GetMouseButton(0))
        {
            OnDraw?.Invoke();

            if (prevLineRender == null)
            {
                AddNewLineRenderer(null, mousePosition);
            }
            else
            {
                UpdateLine(mousePosition);
            }
        }
    }

    GameObject[] GetAllLinesInScene()
    {
        return GameObject.FindGameObjectsWithTag("Line");
    }

    public void ClearLines()
    {
        GameObject[] lines = GetAllLinesInScene();
        foreach (GameObject currentLine in lines)
        {
            LineRenderer line = currentLine.GetComponent<LineRenderer>();
            Destroy(currentLine);
        }
    }

}
