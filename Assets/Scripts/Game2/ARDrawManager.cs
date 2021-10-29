using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField]
    private ProBuilderMesh phase2Ramp;
    private LineRenderer prevLineRender;
    private LineRenderer currentLineRender = null;
    private GameObject currentLineGameObject = null;

    private List<LineRenderer> lines = new List<LineRenderer>();
    private HashSet<GameObject> gameObjSet = new HashSet<GameObject>();
    private Dictionary<int, Edge> rampTopEdges = new Dictionary<int, Edge>();
    private Dictionary<int, float> edgeHeights = new Dictionary<int, float>
    {
        {4, 0},
        {5, 0},
        {6, 0},
        {7, 0},
    };
    private Dictionary<int, GameObject> rampEdgeObjects = new Dictionary<int, GameObject>();
    private float phase2RampVolume = 0.0f;
    private float phase2RampHeight = 0.0f;
    public Vector2 ramp2DTouchPosition = Vector2.zero;
    public Vector2 rec2DTouchPosition = Vector2.zero;
    public GameObject rampEdgeCollider = null;
    private GameObject touchedPhase3Ramp = null;

    private int positionCount = 2;
    private Vector3 prevPointDistance = Vector3.zero;
    public string GamePhase { get; set; }

    public Vector3 startPos;
    public Vector3 endPos;
    public Vector3 movingTouchPosition;
    private GameObject startObject;
    private bool isSnapping = false;
    private int numLines = 0;

    public Vector3 boxInitialRealWorldPosition;
    public Vector3 boxNewRealWorldPosition;

    private Game2Manager game2Manager;
    private Game2SoundManager g2SoundManager;

    private Boolean canCubeLiftingSnap = false;


    private GameObject liftableCube;

    public GameObject concreteUIDisplay;
    public Image concreteUIFill;
    public TextMeshProUGUI concreteVolDisplay;

    private void Start()
    {
        game2Manager = FindObjectOfType<Game2Manager>();
        g2SoundManager = FindObjectOfType<Game2SoundManager>();
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
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // raycasting to hit the point
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hitObject))
            {
                // constantly track movement
                movingTouchPosition = hitObject.point;
                if (hitObject.transform.tag == "dot")
                {
                    // line not created, instantiate line renderer
                    if (currentLineRender == null)
                    {
                        // instead of using rough points, fix the points at each dot!
                        ARDebugManager.Instance.LogInfo("instantiate line");
                        startObject = hitObject.transform.gameObject;
                        AddNewLineRenderer(startObject.transform.position);
                    }
                    else if (currentLineRender != null && touch.phase == TouchPhase.Ended && !isSnapping)
                    {
                        var ratio = (hitObject.transform.position - startObject.transform.position).magnitude / Constants.ONE_FEET;

                        ARDebugManager.Instance.LogInfo("magnitude ratio is: " + ratio);
                        if ((ratio > 0.85 && ratio < 1.15) || (ratio > 1.85 && ratio < 2.15) || (ratio > 2.85 && ratio < 3.15) || (ratio > 3.85 && ratio < 4.15) || (ratio > 4.85 && ratio < 5.15))
                        {
                            numLines++;
                            isSnapping = true;
                            g2SoundManager.PlayGoodSoundEffect();
                        }
                        else
                        {
                            //game2Manager.PlayWrongDrawingWithAnimation();
                            game2Manager.PlayWrongDiagonalWithAnimation();
                        }
                    }
                }
                if (touch.phase == TouchPhase.Began)
                {
                    if (hitObject.transform.tag == "rampEdge")
                    {
                        // Initiate edge to be hit
                        rampEdgeCollider = hitObject.transform.gameObject;
                        // deactivate other edges
                        int edgeNum = int.Parse(rampEdgeCollider.name);
                        DeactivateOtherColliders(edgeNum);
                        // set ramp edge touch positions in 2D
                        ramp2DTouchPosition = touch.position;
                        Debug.Log("initialize rampEdgePos " + ramp2DTouchPosition);
                    }
                    if (hitObject.transform.tag == "p3Ramp" && GamePhase == Constants.GamePhase.PHASE3)
                    {
                        // Initiate face to be hit
                        touchedPhase3Ramp = hitObject.transform.gameObject;
                        // deactivate other edges, only left ramp
                        DeactivateOtherColliders(1);
                        // set ramp edge touch positions in 2D
                        ramp2DTouchPosition = touch.position;
                        Debug.Log("initialize rampface " + ramp2DTouchPosition);
                    }
                    if (hitObject.transform.tag == "liftable_shape")
                    {
                        rec2DTouchPosition = touch.position;
                        liftableCube = hitObject.transform.gameObject;
                        Debug.Log("hitObject.transform.gameObject.name: " + hitObject.transform.gameObject.name);
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                // moving rect after phase1
                if (rec2DTouchPosition != Vector2.zero && liftableCube != null)
                {
                    var uINumberControl = liftableCube.transform.root.gameObject.GetComponent<UINumberControl>();
                    var curVolNum = System.Math.Round((liftableCube.transform.parent.transform.localScale.y / 0.57f), 1);

                    Debug.Log("liftableCube.transform.parent.transform.localScale.y: " + liftableCube.transform.parent.transform.localScale.y);
                    Debug.Log("this is curVolNum: " + curVolNum);

                    if (curVolNum > 0.85 && curVolNum < 1.15)
                    {
                        //glow effect here
                        GameObject.FindGameObjectWithTag("ForceField").GetComponent<MeshRenderer>().enabled = true;
                        canCubeLiftingSnap = true;
                    }
                    else
                    {
                        GameObject.FindGameObjectWithTag("ForceField").GetComponent<MeshRenderer>().enabled = false;
                        canCubeLiftingSnap = false;
                    }

                    var newTouchpos = touch.position;


                    if (newTouchpos.y - rec2DTouchPosition.y > 0)
                    {
                        Debug.Log("lifting it now---------------------------------------");
                        if (curVolNum > 1)
                        {
                            uINumberControl.SetVolDisplay(1);
                        }
                        else
                        {
                            uINumberControl.SetVolDisplay(curVolNum);
                        }
                        // 3d shape lift
                        liftableCube.transform.parent.localScale += new Vector3(0, 0.008f, 0);
                        // 3d ui lift
                        liftableCube.transform.root.GetComponentInChildren<RectTransform>().localPosition += new Vector3(0, 0.008f, 0);
                    }
                    else if ((newTouchpos.y - rec2DTouchPosition.y < 0) && liftableCube.transform.parent.localScale.y >= 0)
                    {
                        Debug.Log("drag it down---------------------------------------");
                        uINumberControl.SetVolDisplay(curVolNum);
                        liftableCube.transform.parent.localScale -= new Vector3(0, 0.008f, 0);
                        liftableCube.transform.root.GetComponentInChildren<RectTransform>().localPosition += new Vector3(0, -0.008f, 0);

                    }
                }

                // moving ramp after phase2
                if (ramp2DTouchPosition != Vector2.zero && rampEdgeCollider != null)
                {
                    var newTouchpos = touch.position;
                    var movementRange = 0.008f;
                    var movingRange = new Vector3(movementRange, 0, 0);

                    int edgeNum = int.Parse(rampEdgeCollider.transform.name);
                    var movingEdge = Enumerable.Repeat(rampTopEdges[edgeNum], 1);
                    var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();

                    if (newTouchpos.y - ramp2DTouchPosition.y > 0)
                    {
                        Debug.Log("edge moving up");
                        edgeHeights[edgeNum] += movementRange;
                        phase2Ramp.TranslateVertices(movingEdge, movingRange);
                        rampEdgeCollider.transform.localPosition += movingRange;
                        uiNumberControl.IncreaseCanvasY(movementRange / 4);
                    } else if (newTouchpos.y - ramp2DTouchPosition.y < 0 && edgeHeights[edgeNum] > 0f)
                    {
                        Debug.Log("edge moving down");
                        edgeHeights[edgeNum] -= movementRange;
                        phase2Ramp.TranslateVertices(movingEdge, -movingRange);
                        rampEdgeCollider.transform.localPosition -= movingRange;
                        uiNumberControl.IncreaseCanvasY(-movementRange / 4);
                    }
                    // set UI height: with edgeHeights[i]
                    phase2RampHeight = edgeHeights[edgeNum];
                    uiNumberControl.Height = phase2RampHeight / Constants.ONE_FEET;
                    phase2RampVolume = (float)(phase2RampHeight * uiNumberControl.area / Constants.ONE_FEET);
                    uiNumberControl.SetVolDisplay(System.Math.Round(phase2RampVolume, 1));
                    // add concrete text
                    concreteVolDisplay.text = "Vol: " + System.Math.Round(phase2RampVolume, 1) + " ft<sup>3</sup>";
                    concreteUIFill.fillAmount = (float)phase2RampVolume / 2;

                    // activate glowing effect
                    var targetMat = phase2Ramp.GetComponent<Renderer>().material;
                    if (phase2RampVolume > 1.8f && phase2RampVolume < 2.2f && targetMat.GetFloat("_EmissIntensity") != 1.2f) {
                        targetMat.SetFloat("_EmissIntensity", 1.2f);
                    }
                    else if (targetMat.GetFloat("_EmissIntensity") != 0.66f)
                    {
                        targetMat.SetFloat("_EmissIntensity", 0.66f);
                    }
                }
                // moving ramp face
                if (ramp2DTouchPosition != Vector2.zero && touchedPhase3Ramp != null)
                {
                    var newTouchpos = touch.position;
                    var movementRange = 0.008f;
                    var movingRange = new Vector3(movementRange, 0, 0);
                    var topFace = touchedPhase3Ramp.transform.root.GetComponent<ProBuilderMesh>();
                    topFace.TranslateVertices(topFace.faces.ElementAt(1).edges, new Vector3(0.08f, 0, 0));
                }
                // drawing line logic
                if (currentLineRender != null && !isSnapping)
                {
                    DrawLineWithoutSnapping();
                }
            }

            // remove line logic
            if (touch.phase == TouchPhase.Ended)
            {
                // draw line logic after let go for all phases
                if (currentLineRender != null)
                {
                    if (!isSnapping)
                    {
                        Destroy(currentLineGameObject);
                    }
                    else
                    {
                        // create line and handle logic
                        endPos = hitObject.transform.gameObject.transform.position;
                        currentLineRender.SetPosition(1, endPos);
                        var lineMagnitude = (endPos - startPos).magnitude;
                        currentLineGameObject.GetComponent<LineController>().SetDistance(lineMagnitude);
                        ARDebugManager.Instance.LogInfo("endPos hit is: " + endPos);
                        if (GamePhase == Constants.GamePhase.PHASE2 || GamePhase == Constants.GamePhase.PHASE3)
                        {
                            gameObjSet.Add(startObject.gameObject);
                            gameObjSet.Add(hitObject.transform.gameObject);
                        }
                        HandleSnapObject();
                    }
                    ARDebugManager.Instance.LogInfo("let go. gamephase: " + GamePhase + "with numLines: " + numLines);
                    prevLineRender = currentLineRender;
                    currentLineRender = null;
                    startObject = null;
                    isSnapping = false;
                }
                if (GamePhase == Constants.GamePhase.PHASE1)
                {
                    if (rec2DTouchPosition != Vector2.zero && liftableCube != null)
                    {
                        rec2DTouchPosition = Vector2.zero;
                        liftableCube = null;
                    }
                }
                // phase2 moving ramp 
                if (GamePhase == Constants.GamePhase.PHASE2)
                {
                    // snaping for ramp
                    // TODO: apply to all cases in ramp
                    if (phase2RampVolume > 1.8f && phase2RampVolume < 2.2f)
                    {
                        int edgeNum = int.Parse(rampEdgeCollider.transform.name);

                        Debug.Log("completed ramp");
                        game2Manager.rampHeight = phase2RampHeight;
                        // phase 2 vol number snap
                        var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();
                        uiNumberControl.SetVolDisplay(2);
                        uiNumberControl.Height = System.Math.Round(phase2RampHeight / Constants.ONE_FEET);
                        concreteUIDisplay.SetActive(false);
                        concreteUIFill.fillAmount = 0;

                        // settle which position to use for animation
                        var topLeft = phase2Ramp.transform.FindChild("TopLeft").transform.position;
                        var topRight = phase2Ramp.transform.FindChild("TopRight").transform.position;
                        var botLeft = phase2Ramp.transform.FindChild("BotLeft").transform.position;
                        var botRight = phase2Ramp.transform.FindChild("BotRight").transform.position;

                        var animeEndPt = Vector3.zero;
                        var animeStartPt = Vector3.zero;
                        switch (edgeNum)
                        {
                            case 4:
                                animeEndPt= (topRight + botRight) / 2;
                                animeStartPt = (topLeft + botLeft) / 2;
                                break;
                            case 5:
                                animeEndPt = (botLeft + botRight) / 2;
                                animeStartPt = (topLeft + topRight) / 2;
                                break;
                            case 6:
                                animeEndPt = (topLeft + topRight) / 2;
                                animeStartPt = (botLeft + botRight) / 2;
                                break;
                            case 7:
                                animeEndPt = (topLeft + botLeft) / 2;
                                animeStartPt = (topRight + botRight) / 2;
                                break;
                        }
                        Debug.Log("animeStartPt: " + animeStartPt);
                        Debug.Log("animeEndPt: " + animeEndPt);
                        Debug.Log("phase2RampHeight: " + phase2RampHeight);
                        game2Manager.StartPhase2End(animeStartPt, animeEndPt, phase2RampHeight);
                        SetRampEdgeCollider(false);
                    }

                    // reset ramp positions and height
                    if (ramp2DTouchPosition != Vector2.zero && rampEdgeCollider != null)
                    {
                        if (phase2RampHeight <= 0)
                        {
                            SetRampEdgeCollider(true);
                        }
                        ramp2DTouchPosition = Vector2.zero;
                        rampEdgeCollider = null;
                    }
                }

                if (canCubeLiftingSnap == true)
                {
                    concreteUIFill.fillAmount = 0;
                    concreteUIDisplay.SetActive(false);
                    GameObject.FindObjectOfType<UINumberControl>().SetVolDisplay(1);
                    canCubeLiftingSnap = false;
                    game2Manager.EndPhase1();
                    g2SoundManager.PlayGoodSoundEffect();
                    GameObject.FindGameObjectWithTag("liftable_shape").GetComponent<BoxCollider>().enabled = false;
                }
            }
        }

        if (GamePhase == Constants.GamePhase.PHASE3)
        {
            //if (IsDoubleTap())
            //{
            //    Debug.Log("destroy is called without ramp");
            //}
            //if (IsDoubleTapDestroy())
            //{
            //    Debug.Log("destroy is called");
            //    Destroy(touchedPhase3Ramp.transform.root.gameObject);
            //}
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
        if (GamePhase == Constants.GamePhase.PHASE1 && numLines == 4)
        {
            concreteUIDisplay.SetActive(true);
            Debug.Log("phase1 mid now!!!!!!!!!!!!!!!~~~~~~~~~~~~~~~~`");
            DotsManager.Instance.ActivatePhase1Cube();
        }
        if (GamePhase == Constants.GamePhase.PHASE2 && numLines == 4)
        {
            foreach(GameObject game in gameObjSet)
            {
                Debug.Log("name in set is: " + game.transform.name);
            }
            if (gameObjSet.Count != 4)
            {
                Debug.Log("not a rectangle/square");
                game2Manager.PlayWrongLinesWithAnimation();
                numLines = 0;
                ClearLines();
                gameObjSet.Clear();
                return; // not 4 unique points, so not rectangle
            }
            var dotPoints = GetDotPoints();
            var maxLength = GetMaxPoints(dotPoints);

            var initializePos = Vector3.zero;
            foreach(GameObject gameObject in gameObjSet)
            {
                initializePos += gameObject.transform.position;
            }

            InitializeRamp(initializePos / 4);
            phase2Ramp.transform.localScale = new Vector3(0.5f, maxLength * Constants.ONE_FEET, Constants.ONE_FEET);

            // set initial volume and set up
            var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();
            uiNumberControl.SetAreaDisplay(maxLength);
            uiNumberControl.Height = 0;
            game2Manager.StartPhase2Mid();
            // set ramp start point & endpoint
            //game2Manager.rampStartPoint = minVector; // - new Vector3(0, Constants.HALF_FEET, 0);
            //game2Manager.rampEndPoint = maxVector; // + new Vector3(0, Constants.HALF_FEET, 0);
            numLines = 0;
        }

        if (GamePhase == Constants.GamePhase.PHASE3 && numLines == 4)
        {
            // same code as above
            if (!CheckIfRect()) return;
            var dotPoints = GetDotPoints();
            var maxLength = GetMaxPoints(dotPoints);

            var initializePos = Vector3.zero;
            foreach (GameObject gameObject in gameObjSet)
            {
                initializePos += gameObject.transform.position;
            }

            InitializeRamp(initializePos / 4);
            phase2Ramp.transform.localScale = new Vector3(0.5f, maxLength * Constants.ONE_FEET, Constants.ONE_FEET);
            // set initial volume and set up
            var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();
            uiNumberControl.SetAreaDisplay(maxLength);
            uiNumberControl.Height = 0;
            // difference in code
        }
        isSnapping = false;
        currentLineRender = null;
    }

    private bool CheckIfRect()
    {
        if (gameObjSet.Count != 4)
        {
            Debug.Log("not a rectangle/square");
            game2Manager.PlayWrongLinesWithAnimation();
            numLines = 0;
            ClearLines();
            gameObjSet.Clear();
            return false; // not 4 unique points, so not rectangle
        }
        return true;
    }

    private int GetMaxPoints(List<(int, int)> dotPoints)
    {
        int maxX = 0;
        int maxY = 0;
        for (int i = 1; i < dotPoints.Count; i++)
        {
            maxX = System.Math.Max(System.Math.Abs(dotPoints[0].Item1 - dotPoints[i].Item1), maxX);
            maxY = System.Math.Max(System.Math.Abs(dotPoints[0].Item2 - dotPoints[i].Item2), maxY);
        }
        var res = System.Math.Max(maxX, maxY);
        Debug.Log("maxLength is: " + res);
        return res;
    }

    private List<(int, int)> GetDotPoints()
    {
        List<(int, int)> dotPoints = new List<(int, int)>();
        foreach(GameObject game in gameObjSet)
        {
            var res = game.name.Split('_');
            dotPoints.Add((int.Parse(res[1]), int.Parse(res[2])));
        }
        return dotPoints;
    }

    /// <summary>
    /// Initialize ramp, edges and colliders.
    /// </summary>
    /// <param name="middlePos"></param>
    private void InitializeRamp(Vector3 middlePos)
    {
        phase2Ramp = Instantiate(phase2Ramp, middlePos, phase2Ramp.transform.rotation);
        DotsManager.Instance.ClearDots();
        gameObjSet.Clear();
        InitializeRampEdges();
        InitializeRampEdgeObjects();
        SetRampEdgeCollider(false);
    }

    private void DrawLineWithoutSnapping()
    {
        // drawing line while not snap to any object
        startPos = startObject.transform.position;
        currentLineRender.SetPosition(0, startPos);
        endPos = movingTouchPosition;
        endPos.y = startPos.y;
        currentLineRender.SetPosition(1, endPos);
        var lineMagnitude = (endPos - startPos).magnitude;
        //ARDebugManager.Instance.LogInfo("line length: " + lineMagnitude);
        var lineController = currentLineGameObject.GetComponent<LineController>();
        lineController.SetDistance(lineMagnitude);

        Vector3 midPointOfTwoPos = Vector3.zero;
        midPointOfTwoPos.x = startPos.x + (endPos.x - startPos.x) / 2;
        midPointOfTwoPos.y = startPos.y + (endPos.y - startPos.y) / 2 + 0.1f;
        midPointOfTwoPos.z = startPos.z + (endPos.z - startPos.z) / 2;
        lineController.SetPosition(midPointOfTwoPos);
    }

    public void SetRampEdgeCollider(bool isActive)
    {
        // activate all colliders when height is 0
        foreach (var gameObject in rampEdgeObjects)
        {
            gameObject.Value.SetActive(isActive);
        }
    }
    /// <summary>
    /// Activate ramp top or 1 edge.
    /// </summary>
    /// <param name="edgeNum"></param>
    private void DeactivateOtherColliders(int edgeNum)
    {
        foreach (var gameObject in rampEdgeObjects)
        {
            if (gameObject.Key != edgeNum)
            {
                gameObject.Value.SetActive(false);
            }
        }
        if (touchedPhase3Ramp != null)
        {
            if (edgeNum == 1) {
                touchedPhase3Ramp.SetActive(true);
            }
            else
            {
                touchedPhase3Ramp.SetActive(false);
            }
        }
    }

    private void InitializeRampEdgeObjects()
    {
        GameObject[] edgeObjects = GameObject.FindGameObjectsWithTag("rampEdge");
        foreach(GameObject edge in edgeObjects)
        {
            Debug.Log("edge object found: " + edge.gameObject.name);
            rampEdgeObjects.Add(int.Parse(edge.gameObject.name), edge);
        }
    }
    private void InitializeRampEdges()
    {
        int[] edgeNums = { 4, 5, 6, 7 };
        foreach (Face f in phase2Ramp.faces)
        {
            foreach (Edge e in f.edges)
            {
                foreach (int num in edgeNums)
                {
                    if (e.a == num)
                    {
                        rampTopEdges.Add(num, e);
                    }
                }
            }
        }
    }

    private bool IsDoubleTapDestroy()
    {
        bool result = false;
        float MaxTimeWait = 1;
        float VariancePosition = 1;

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            float DeltaTime = Input.GetTouch(0).deltaTime;
            float DeltaPositionLenght = Input.GetTouch(0).deltaPosition.magnitude;

            Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hitObject))
            {
                if (DeltaTime > 0 && DeltaTime < MaxTimeWait && DeltaPositionLenght < VariancePosition && hitObject.transform.tag == Constants.Tags.DestroyCollider)
                    result = true;
            }
        }
        return result;
    }

    private bool IsDoubleTap()
    {
        bool result = false;
        float MaxTimeWait = 1;
        float VariancePosition = 1;

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            
            float DeltaTime = Input.GetTouch(0).deltaTime;
            float DeltaPositionLenght = Input.GetTouch(0).deltaPosition.magnitude;

            if (DeltaTime > 0 && DeltaTime < MaxTimeWait && DeltaPositionLenght < VariancePosition)
                result = true;
        }
        return result;
    }

    private void AddNewLineRenderer(Vector3 touchPosition)
    {
        positionCount = 2;
        currentLineGameObject = Instantiate(linePrefab, touchPosition, Quaternion.identity);
        currentLineGameObject.name = $"LineRenderer_{lines.Count}";
        //currentLineGameObject.AddComponent<ARAnchor>();
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

    public void DestoryRamp()
    {
        Destroy(phase2Ramp.gameObject);
    }

    public void ClearLineRenders()
    {
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
        foreach (GameObject currentLine in lines)
        {
            LineRenderer line = currentLine.GetComponent<LineRenderer>();
            Destroy(currentLine);
        }
    }

    public void ClearLines()
    {
        numLines = 0;

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
    /// check if it is rectangle and determines maxLength of rect
    /// </summary>
    /// <param name="minVector"></param>
    /// <param name="maxVector"></param>
    /// <returns></returns>
    //private (float, bool) CheckRectAndGetValue(Vector3 minVector, Vector3 maxVector)
    //{
    //float maxValue = 0.0f;

    //var centerX = drawnPositions.Sum(dot => dot.x) / 4f;
    //var centerZ = drawnPositions.Sum(dot => dot.z) / 4f;

    //var dot1 = drawnPositions.ElementAt(0);
    //var dot2 = drawnPositions.ElementAt(1);
    //var dot3 = drawnPositions.ElementAt(2);
    //var dot4 = drawnPositions.ElementAt(3);
    //var line1 = System.Math.Round(System.Math.Pow(centerX - dot1.x, 2) + System.Math.Pow(centerZ - dot1.z, 2), 2);
    //var line2 = System.Math.Round(System.Math.Pow(centerX - dot2.x, 2) + System.Math.Pow(centerZ - dot2.z, 2), 2);
    //var line3 = System.Math.Round(System.Math.Pow(centerX - dot3.x, 2) + System.Math.Pow(centerZ - dot3.z, 2), 2);
    //var line4 = System.Math.Round(System.Math.Pow(centerX - dot4.x, 2) + System.Math.Pow(centerZ - dot4.z, 2), 2);

    //Debug.Log("line1: " + line1.ToString("N4"));
    //Debug.Log("line2: " + line2.ToString("N4"));
    //Debug.Log("line3: " + line3.ToString("N4"));
    //Debug.Log("line4: " + line4.ToString("N4"));

    //if (line1 == line2 && line1 == line3 && line1 == line4)
    //{
    //    Debug.Log("rect is formed");
    //    return (maxValue, true);
    //} else
    //{
    //    Debug.Log("rect is not formed");
    //    return (maxValue, false);
    //}

    //foreach (Vector3 pos in drawnPositions)
    //{
    //    Debug.Log("drawnPositions: " + pos);

    //var minMag = (minVector - pos).magnitude;
    //var maxMag = (maxVector - pos).magnitude;
    //maxValue = Mathf.Max(minMag, maxMag);
    //Debug.Log("minMag = : " + System.Math.Floor(minMag * 10f) + ", maxMag: " + System.Math.Floor(maxMag * 10f));
    //if (System.Math.Floor(minMag * 10f) % 3 != 0 || System.Math.Floor(maxMag * 10f) % 3 != 0)
    //{
    //    Debug.Log("it is not a square / rectangle");
    //    game2Manager.PlayWrongDrawingWithAnimation();
    //    numLines = 0;
    //    ClearLines();
    //    drawnPositions.Clear();
    //    return (maxValue, false);
    //}
    //}
    //return (maxValue, true);
    //}

    /// <summary>
    /// Get min and max vectors from drawnPositions
    /// </summary>
    /// <returns></returns>
    //private (Vector3, Vector3) GetMinMaxVector()
    //{
    //    Vector3 minVector = Vector3.positiveInfinity;
    //    Vector3 maxVector = Vector3.zero;
    //    foreach (Vector3 pos in drawnPositions)
    //    {
    //        //    minVector = Vector3.Min(pos, minVector);
    //        //    maxVector = Vector3.Max(pos, maxVector);
    //        Debug.Log("vector pos: " + pos.ToString("N4"));
    //        minVector = (pos.magnitude < minVector.magnitude) ? pos : minVector;
    //        maxVector = (pos.magnitude > maxVector.magnitude) ? pos : maxVector;
    //    }
    //    Debug.Log("minVector positions before: " + minVector.ToString("N4"));
    //    Debug.Log("maxVector positions before: " + maxVector.ToString("N4"));
    //    if (minVector.x > maxVector.x)
    //    {
    //        (minVector, maxVector) = (maxVector, minVector);
    //    }
    //    Debug.Log("minVector positions after: " + minVector.ToString("N4"));
    //    Debug.Log("maxVector positions after: " + maxVector.ToString("N4"));
    //    //drawnPositions.Remove(minVector);
    //    //drawnPositions.Remove(maxVector);
    //    return (minVector, maxVector);
    //}

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
