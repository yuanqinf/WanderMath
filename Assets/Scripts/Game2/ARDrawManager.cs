using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Rendering.PostProcessing;
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
    private bool allowSimplification = true;
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

    [SerializeField]
    private float lineWidth = 0.05f;
    private Color randomStartColor = Color.white;
    private Color randomEndColor = Color.white;

    [SerializeField]
    private GameObject linePrefab;
    private LineRenderer prevLineRender;
    private LineRenderer currentLineRender = null;
    private GameObject currentLineGameObject = null;

    private List<GameObject> linesGameObject = new List<GameObject>();
    private List<ARAnchor> anchors = new List<ARAnchor>();
    private List<LineRenderer> lines = new List<LineRenderer>();
    private List<Vector3> phase2DrawnPos = new List<Vector3>();

    private int positionCount = 2;
    private Vector3 prevPointDistance = Vector3.zero;

    private bool CanDraw { get; set; }
    public string GamePhase { get; set; }

    public Vector3 startPos;
    public Vector3 endPos;
    public Vector3 movingTouchPosition;
    private GameObject startObject;
    private GameObject endObject;
    private bool isSnapping = false;
    private int numLines = 0;

    public Vector3 boxInitialRealWorldPosition;
    public Vector3 boxNewRealWorldPosition;

    private Game2Manager game2Manager;
    private UINumberControl uINumberControl;
    private Game2SoundManager g2SoundManager;

    public int[] startDotMatPos = new int[2];
    public int[] endDotMatPos = new int[2];

    private Boolean isPlayingWrongDraw = false;
    private Boolean startLiftCube = false;
    private Boolean canCubeLiftingSnap = false;


    private GameObject liftableCube;

    private HashSet<String> visitedDots;

    private void Start()
    {
        game2Manager = FindObjectOfType<Game2Manager>();
        uINumberControl = FindObjectOfType<UINumberControl>();
        g2SoundManager = FindObjectOfType<Game2SoundManager>();
        visitedDots = new HashSet<String>();
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
                liftableCube = hitObject.transform.gameObject;
                // constantly track movement
                movingTouchPosition = hitObject.point;
                if (hitObject.transform.tag == "dot")
                {
                    // line created, instantiate line renderer
                    if (currentLineRender == null)
                    {
                        // TODO: instead of using rough points, fix the points at each dot!
                        ARDebugManager.Instance.LogInfo("instantiate line");
                        startObject = hitObject.transform.gameObject;
                        AddNewLineRenderer(movingTouchPosition);
                    }
                    else if (currentLineRender != null && startObject.transform.position != hitObject.transform.position && !isSnapping)
                    {
                        if (GamePhase == Constants.GamePhase.PHASE0)
                        {
                            numLines++;
                            isSnapping = true;
                            ARDebugManager.Instance.LogInfo("touched object to snap");
                        }
                        else if (GamePhase == Constants.GamePhase.PHASE1)
                        {
                            // initialize array
                            for (int r = 0; r < 2; r++)
                            {
                                for (int c = 0; c < 2; c++)
                                {

                                    float startNum = (startObject.transform.position.x + startObject.transform.position.y + startObject.transform.position.z);
                                    float endNum = (hitObject.transform.position.x + hitObject.transform.position.y + hitObject.transform.position.z);
                                    float curMatrixPointNum = (DotsManager.Instance.phase1DotsMatrix[r, c].x + DotsManager.Instance.phase1DotsMatrix[r, c].y + DotsManager.Instance.phase1DotsMatrix[r, c].z);

                                    if (startNum >= curMatrixPointNum - 0.02f && startNum <= curMatrixPointNum + 0.02f)
                                    {
                                        Debug.Log("matched start");
                                        startDotMatPos[0] = r;
                                        startDotMatPos[1] = c;
                                    }
                                    if (endNum >= curMatrixPointNum - 0.02f && endNum <= curMatrixPointNum + 0.02f)
                                    {
                                        Debug.Log("matched end");
                                        endDotMatPos[0] = r;
                                        endDotMatPos[1] = c;
                                    }
                                }
                            }

                            // checking snapping
                            if ((startDotMatPos[0] == endDotMatPos[0] || startDotMatPos[1] == endDotMatPos[1]) && isSnapping == false)
                            {
                                numLines++;
                                isSnapping = true;
                                g2SoundManager.PlayGoodSoundEffect();
                                currentLineRender.SetPosition(1, hitObject.transform.position);
                                ARDebugManager.Instance.LogInfo("snapped. line created: " + numLines);
                                if (numLines == 4)
                                {
                                    g2SoundManager.playFinishDrawing();
                                    DotsManager.Instance.ActivatePhase1Cube();
                                }
                            }
                            else
                            {
                                if (isPlayingWrongDraw == false)
                                {
                                    isPlayingWrongDraw = true;
                                    g2SoundManager.playWrongDrawing();
                                }
                            }
                            startDotMatPos[0] = 0;
                            startDotMatPos[1] = 0;
                            endDotMatPos[0] = 0;
                            endDotMatPos[1] = 0;
                        }
                        else if (GamePhase == Constants.GamePhase.PHASE2)
                        {
                            var ratio = (hitObject.transform.position - startObject.transform.position).magnitude / Constants.ONE_FEET;

                            ARDebugManager.Instance.LogInfo("magnitude ratio is: " + ratio);
                            if ((ratio > 0.9 && ratio < 1.1) || (ratio > 1.9 && ratio < 2.1))
                            {
                                numLines++;
                                isSnapping = true;
                            }
                        }
                    }
                }
                if (hitObject.transform.tag == "liftable_shape")
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        boxInitialRealWorldPosition = hitObject.point;
                    }
                    // lifting detection begin
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Debug.Log("find the object!!!!!!!");
                        boxNewRealWorldPosition = hitObject.point;
                        uINumberControl.volDisplay = hitObject.transform.root.GetChild(1).gameObject;

                        double curVolNum = Math.Round(hitObject.transform.GetComponent<BoxCollider>().bounds.size.x * 3.28084 *
                                                      hitObject.transform.GetComponent<BoxCollider>().bounds.size.y * 3.28084 *
                                                      hitObject.transform.GetComponent<BoxCollider>().bounds.size.z * 3.28084, 2);

                        Debug.Log("this i hitObject.transform.GetComponent<BoxCollider>().bounds.size.x: " + hitObject.transform.GetComponent<BoxCollider>().bounds.size.x);
                        Debug.Log("this i hitObject.transform.GetComponent<BoxCollider>().bounds.size.y: " + hitObject.transform.GetComponent<BoxCollider>().bounds.size.y);
                        Debug.Log("this i hitObject.transform.GetComponent<BoxCollider>().bounds.size.z: " + hitObject.transform.GetComponent<BoxCollider>().bounds.size.z);

                        if (curVolNum > 0.8 && curVolNum < 1.12)
                        {
                            hitObject.transform.GetComponent<PostProcessVolume>().enabled = true;
                            canCubeLiftingSnap = true;
                        }
                        else
                        {
                            hitObject.transform.GetComponent<PostProcessVolume>().enabled = false;
                            canCubeLiftingSnap = false;
                        }

                        if (boxNewRealWorldPosition.z > boxInitialRealWorldPosition.z + 0.05 || boxNewRealWorldPosition.y > boxInitialRealWorldPosition.y + 0.05)
                        {
                            Debug.Log("lifting it now---------------------------------------");
                            uINumberControl.SetVolDisplay(curVolNum);
                            // 3d shape lift
                            hitObject.transform.parent.localScale += new Vector3(0, 0.008f, 0);
                            // 3d ui lift
                            hitObject.transform.root.GetComponentInChildren<RectTransform>().localPosition += new Vector3(0, 0.008f, 0);
                        }
                        else if ((boxNewRealWorldPosition.z < boxInitialRealWorldPosition.z - 0.05 || boxNewRealWorldPosition.y < boxInitialRealWorldPosition.y - 0.05) && hitObject.transform.parent.localScale.y >= 0)
                        {
                            Debug.Log("drag it down---------------------------------------");
                            uINumberControl.SetVolDisplay(curVolNum);
                            hitObject.transform.parent.localScale -= new Vector3(0, 0.008f, 0);
                            hitObject.transform.root.GetComponentInChildren<RectTransform>().localPosition += new Vector3(0, -0.008f, 0);
                        }

                    }
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (currentLineRender != null)
                {
                    // drawing line while not snap to any object
                    if (!isSnapping)
                    {
                        startPos = startObject.transform.position;
                        currentLineRender.SetPosition(0, startPos);
                        endPos = movingTouchPosition;
                        endPos.y = startPos.y;
                        currentLineRender.SetPosition(1, endPos);
                        var lineMagnitude = (endPos - startPos).magnitude;
                        //ARDebugManager.Instance.LogInfo("line length: " + lineMagnitude);
                        var lineController = currentLineGameObject.GetComponent<LineController>();
                        lineController.SetDistance(lineMagnitude);
                        lineController.SetPosition(endPos);
                    }
                    else
                    {
                        // create line and handle logic
                        //movingTouchPosition.y = startPos.y;
                        if (GamePhase == Constants.GamePhase.PHASE2)
                        {
                            endPos = hitObject.transform.gameObject.transform.position;
                            currentLineRender.SetPosition(1, endPos);
                            ARDebugManager.Instance.LogInfo("endPos hit is: " + endPos);
                            phase2DrawnPos.Add(startPos);
                            phase2DrawnPos.Add(endPos);
                            HandleSnapObject();
                        }
                        else
                        {
                            String startMatPos = startDotMatPos[0].ToString() + startDotMatPos[1].ToString();
                            String endMatPos = endDotMatPos[0].ToString() + endDotMatPos[1].ToString();
                            if (!visitedDots.Contains(startMatPos) && !visitedDots.Contains(endMatPos) && hitObject.transform.tag == "dot")
                            {
                                endPos = hitObject.transform.position;
                                endPos.y = startPos.y;
                                currentLineRender.SetPosition(1, endPos);
                                HandleSnapObject();
                                visitedDots.Add(startMatPos);
                                visitedDots.Add(endMatPos);
                            }
                        }
                    }
                }
            }

            // remove line logic
            if (touch.phase == TouchPhase.Ended)
            {
                isPlayingWrongDraw = false;

                if(currentLineRender != null)
                {
                    if (!isSnapping)
                    {
                        Destroy(currentLineGameObject);
                    }
                    ARDebugManager.Instance.LogInfo("let go. gamephase: " + GamePhase + "with numLines: " + numLines);
                    prevLineRender = currentLineRender;
                    currentLineRender = null;
                    startObject = null;
                    isSnapping = false;
                }

                if (canCubeLiftingSnap == true && startLiftCube)
                {
                    canCubeLiftingSnap = false;
                    game2Manager.EndPhase1();
                    g2SoundManager.PlayGoodSoundEffect();
                    liftableCube.GetComponent<BoxCollider>().enabled = false;
                }
                else if (canCubeLiftingSnap == false && startLiftCube)
                {
                    //g2SoundManager.playWrongCubeLiftAudio();
                }

                if(numLines == 4)
                {
                    startLiftCube = true;
                }
            }
        }
    }

    private void HandleSnapObject()
    {
        if (GamePhase == Constants.GamePhase.PHASE0 && numLines == 1)
        {
            game2Manager.EndPhase0();
            numLines = 0;
            Destroy(currentLineGameObject);
        }
        if (GamePhase == Constants.GamePhase.PHASE2 && numLines == 4)
        {
            // TODO: check if its square or rectangle based on the snapped dots
            phase2DrawnPos.ForEach(i => Debug.Log("positions: " + i));
            Vector3 minVector = Vector3.positiveInfinity;
            Vector3 maxVector = Vector3.zero;
            foreach (Vector3 pos in phase2DrawnPos)
            {
                minVector = (pos.magnitude < minVector.magnitude) ? pos : minVector;
                maxVector = (pos.magnitude > maxVector.magnitude) ? pos : maxVector;
            }
            Debug.Log("minVector positions: " + minVector);
            Debug.Log("maxVector positions: " + maxVector);
            numLines = 0;

            // TODO: create object
        }
        isSnapping = false;
        currentLineGameObject.GetComponent<LineController>().SetDistance(Constants.ONE_FEET);
        currentLineRender = null;
    }

    private void AddNewLineRenderer(Vector3 touchPosition)
    {
        positionCount = 2;
        currentLineGameObject = Instantiate(linePrefab, touchPosition, Quaternion.identity);
        currentLineGameObject.name = $"LineRenderer_{lines.Count}";
        currentLineGameObject.AddComponent<ARAnchor>();
        currentLineGameObject.transform.position = touchPosition;
        currentLineGameObject.tag = "Line";
        LineRenderer goLineRenderer = currentLineGameObject.AddComponent<LineRenderer>();
        goLineRenderer.startWidth = lineWidth;
        goLineRenderer.endWidth = lineWidth;
        goLineRenderer.material = defaultColorMaterial;
        goLineRenderer.useWorldSpace = true;
        goLineRenderer.positionCount = positionCount;
        //goLineRenderer.numCapVertices = 90;

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
        //GameObject[] lines = GetAllLinesInScene();
        //foreach (GameObject currentLine in lines)
        //{
        //    LineRenderer line = currentLine.GetComponent<LineRenderer>();
        //    Destroy(currentLine);
        //}

        GameObject[] lineObjects = GameObject.FindGameObjectsWithTag("Line");
        foreach (GameObject lineObj in lineObjects)
        {
            Destroy(lineObj);
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
