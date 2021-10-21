using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

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
    private Camera arCamera;

    private Game2Manager game2Manager;

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
    public string GamePhase { get; set; }

    public Vector3 startPos;
    public Vector3 endPos;
    public Vector3 touchPosition;
    private GameObject startObject;
    private bool isSnapping = false;
    private int numLines = 0;

    public Vector3 initialRealWorldPosition;
    public Vector3 newRealWorldPosition;

    private UINumberControl uINumberControl;

    private void Start()
    {
        game2Manager = FindObjectOfType<Game2Manager>();
        uINumberControl = FindObjectOfType<UINumberControl>();
    }

    void Update()
    {
        #if !UNITY_EDITOR
        //if (Input.touchCount > 0)
        //    DrawOnTouch();
        #else
        //if (Input.GetMouseButton(0))
        //    DrawOnMouse();
        //else
        //{
        //    prevLineRender = null;
        //}
        #endif
    }

    public void AllowDraw(bool isAllow)
    {
        CanDraw = isAllow;
    }

    private void SetLineSettings(LineRenderer currentLineRenderer)
    {
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.numCornerVertices = cornerVertices;
        currentLineRenderer.numCapVertices = endCapVeritices;
        if (allowSimplification) currentLineRenderer.Simplify(tolerance);
        currentLineRenderer.startColor = randomStartColor;
        currentLineRenderer.endColor = randomEndColor;
    }

    public void DrawOnTouch()
    {
        if (!DotsManager.Instance.isDotsPlaced || !CanDraw) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // raycasting to hit the point
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hitObject))
            {
                touchPosition = hitObject.point;

                if (touch.phase == TouchPhase.Began)
                {
                    initialRealWorldPosition = hitObject.point;
                }

                if (hitObject.transform.tag == "dot")
                {
                    // line created, instantiate line renderer
                    if (currentLineRender == null)
                    {
                        ARDebugManager.Instance.LogInfo("instantiate line");
                        startObject = hitObject.transform.gameObject;
                        AddNewLineRenderer(touchPosition);
                    }
                    else if (currentLineRender != null && startObject.transform.position != hitObject.transform.position)
                    {
                        ARDebugManager.Instance.LogInfo("snap now!!!");
                        isSnapping = true;
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                // drawing line
                if (currentLineRender != null && !isSnapping)
                {
                    startPos = startObject.transform.position;
                    currentLineRender.SetPosition(0, startPos);
                    endPos = touchPosition;
                    endPos.y = startPos.y;
                    currentLineRender.SetPosition(1, endPos);
                }
                // lifting detection begin
                if (touch.phase == TouchPhase.Moved)
                {
                    // lifting to 3d shape
                    if (hitObject.transform.tag == "liftable_shape")
                    {
                        RaycastHit rayLocation;
                        if (Physics.Raycast(ray, out rayLocation))
                        {
                            Debug.Log("find the object!!!!!!!");
                            newRealWorldPosition = rayLocation.point;
                            uINumberControl.volDisplay = hitObject.transform.root.GetChild(1).gameObject;

                            int curVolNum = (int) (hitObject.transform.GetComponent<BoxCollider>().bounds.size.x * 3.28084 *
                                                   hitObject.transform.GetComponent<BoxCollider>().bounds.size.y * 3.28084 *
                                                   hitObject.transform.GetComponent<BoxCollider>().bounds.size.z * 3.28084);

                            if (newRealWorldPosition.z > initialRealWorldPosition.z + 0.05 || newRealWorldPosition.y > initialRealWorldPosition.y + 0.05)
                            {
                                Debug.Log("lifting it now---------------------------------------");
                                uINumberControl.SetVolDisplay(curVolNum);
                                hitObject.transform.parent.localScale += new Vector3(0, 0.008f, 0);
                                hitObject.transform.root.GetComponentInChildren<RectTransform>().localPosition += new Vector3(0, 0.008f, 0);
                            }

                            if ((newRealWorldPosition.z < initialRealWorldPosition.z - 0.05 || newRealWorldPosition.y < initialRealWorldPosition.y - 0.05) && hitObject.transform.parent.localScale.y >= 0)
                            {
                                Debug.Log("drag it down---------------------------------------");
                                uINumberControl.SetVolDisplay(curVolNum);
                                hitObject.transform.parent.localScale -= new Vector3(0, 0.008f, 0);
                                hitObject.transform.root.GetComponentInChildren<RectTransform>().localPosition += new Vector3(0, -0.008f, 0);
                            }
                        }
                    }
                }
            }

            // remove line logic
            if (touch.phase == TouchPhase.Ended && currentLineRender != null)
            {
                if (!isSnapping)
                {
                    Destroy(currentLineRender);
                }
                Debug.Log("gamephase: " + GamePhase + "with numLines: " + numLines);
                prevLineRender = currentLineRender;
                currentLineRender = null;
                startObject = null;
                isSnapping = false;
            }

            // create line and handle logic
            if (isSnapping)
            {
                numLines++;
                ARDebugManager.Instance.LogInfo("line created: " + numLines);
                if (GamePhase == Constants.GamePhase.PHASE0 && numLines == 1)
                {
                    game2Manager.EndPhase0();
                    numLines = 0;
                }
                if (GamePhase == Constants.GamePhase.PHASE1 && numLines == 4)
                {
                    // do sth in phase1
                }
                Destroy(currentLineRender);

                isSnapping = false;
            }
        }
    }

    private void AddNewLineRenderer(Vector3 touchPosition)
    {
        positionCount = 2;
        GameObject go = new GameObject($"LineRenderer_{lines.Count}");
        go.AddComponent<ARAnchor>();
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
                AddNewLineRenderer(mousePosition);
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

    #region unused methods in ar
    // TODO in the future: convert to touch position to raycast position
    // Tried to minus the diff between raycast and the conversion but it is not accurate.
    //arCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, distanceFromCamera));

    /// <summary>
    /// Helpful method used to draw freehand.
    /// </summary>
    /// <param name="touchPosition"></param>
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
    #endregion
}
