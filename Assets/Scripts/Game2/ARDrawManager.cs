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
    private HashSet<Vector3> phase2DrawnPos = new HashSet<Vector3>();
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
    public GameObject rampEdge = null;
    public GameObject rampTopFace = null;

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

    private Boolean startLiftCube = false;
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
                liftableCube = hitObject.transform.gameObject;
                Debug.Log("hitObject.transform.gameObject.name: " + hitObject.transform.gameObject.name); ;

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
                        if ((ratio > 0.9 && ratio < 1.1) || (ratio > 1.9 && ratio < 2.1))
                        {
                            numLines++;
                            isSnapping = true;
                            g2SoundManager.PlayGoodSoundEffect();
                        }
                        else
                        {
                            g2SoundManager.playWrongDrawing();
                        }
                    }
                }
                if (hitObject.transform.tag == "rampEdge" && touch.phase == TouchPhase.Began)
                {
                    // Initiate edge to be hit
                    rampEdge = hitObject.transform.gameObject;
                    // deactivate other edges
                    int edgeNum = int.Parse(rampEdge.name);
                    DeactivateOtherColliders(edgeNum);
                    // set ramp edge touch positions in 2D
                    ramp2DTouchPosition = touch.position;
                    Debug.Log("initialize rampEdgePos " + ramp2DTouchPosition);
                }
                if (hitObject.transform.tag == "p3Ramp" && touch.phase == TouchPhase.Began && GamePhase == Constants.GamePhase.PHASE3)
                {
                    // Initiate face to be hit
                    rampTopFace = hitObject.transform.gameObject;
                    // deactivate other edges, only left ramp
                    DeactivateOtherColliders(1);
                    // set ramp edge touch positions in 2D
                    ramp2DTouchPosition = touch.position;
                    Debug.Log("initialize rampface " + ramp2DTouchPosition);
                }
                if (hitObject.transform.tag == "liftable_shape")
                {
                    Debug.Log("this is liftable cube tocuhed here!!!!!!!!!!~~~~~~~");

                    if (touch.phase == TouchPhase.Began)
                    {
                        boxInitialRealWorldPosition = hitObject.point;
                    }
                    // lifting detection begin
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Debug.Log("find the object!!!!!!!");
                        boxNewRealWorldPosition = hitObject.point;
                        var uINumberControl = hitObject.transform.root.gameObject.GetComponent<UINumberControl>();

                        double curVolNum = System.Math.Round(hitObject.transform.GetComponent<BoxCollider>().bounds.size.x * 3.28084 *
                                                      hitObject.transform.GetComponent<BoxCollider>().bounds.size.y * 3.28084 *
                                                      hitObject.transform.GetComponent<BoxCollider>().bounds.size.z * 3.28084, 2);

                        // update concrete UI fill display
                        if((float)curVolNum / 1 > 1)
                        {
                            concreteVolDisplay.text = "Vol: " + (float)curVolNum + " ft<sup>3</sup>";
                            concreteUIFill.fillAmount = 1;
                        }
                        else
                        {
                            concreteVolDisplay.text = "Vol: " + (float)curVolNum + " ft<sup>3</sup>";
                            concreteUIFill.fillAmount = (float)curVolNum / 1;
                        }

                        Debug.Log("this i hitObject.transform.GetComponent<BoxCollider>().bounds.size.x: " + hitObject.transform.GetComponent<BoxCollider>().bounds.size.x);
                        Debug.Log("this i hitObject.transform.GetComponent<BoxCollider>().bounds.size.y: " + hitObject.transform.GetComponent<BoxCollider>().bounds.size.y);
                        Debug.Log("this i hitObject.transform.GetComponent<BoxCollider>().bounds.size.z: " + hitObject.transform.GetComponent<BoxCollider>().bounds.size.z);

                        if (curVolNum > 0.9 && curVolNum < 1.1)
                        {
                            //glow effect here
                            GameObject.FindGameObjectWithTag("ForceField").GetComponent<MeshRenderer>().enabled = true;
                            //hitObject.transform.GetComponent<PostProcessVolume>().enabled = true;
                            canCubeLiftingSnap = true;
                            liftableCube = hitObject.transform.gameObject;
                        }
                        else
                        {
                            GameObject.FindGameObjectWithTag("ForceField").GetComponent<MeshRenderer>().enabled = false;
                            //hitObject.transform.GetComponent<PostProcessVolume>().enabled = false;
                            canCubeLiftingSnap = false;
                        }

                        if (boxNewRealWorldPosition.z > boxInitialRealWorldPosition.z + 0.05 || boxNewRealWorldPosition.y > boxInitialRealWorldPosition.y + 0.05)
                        {
                            Debug.Log("lifting it now---------------------------------------");
                            uINumberControl.SetVolDisplay(curVolNum);
                            // 3d shape lift
                            hitObject.transform.parent.localScale += new Vector3(0, 0.008f, 0);
                            // 3d ui lift
                            hitObject.transform.root.GetComponentInChildren<RectTransform>().localPosition += new Vector3(0, 0.005f, 0);
                        }
                        else if ((boxNewRealWorldPosition.z < boxInitialRealWorldPosition.z - 0.05 || boxNewRealWorldPosition.y < boxInitialRealWorldPosition.y - 0.05) && hitObject.transform.parent.localScale.y >= 0)
                        {
                            Debug.Log("drag it down---------------------------------------");
                            uINumberControl.SetVolDisplay(curVolNum);
                            hitObject.transform.parent.localScale -= new Vector3(0, 0.008f, 0);
                            hitObject.transform.root.GetComponentInChildren<RectTransform>().localPosition += new Vector3(0, -0.005f, 0);
                        }

                    }
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                // moving ramp after phase2
                if (ramp2DTouchPosition != Vector2.zero && rampEdge != null)
                {
                    var newTouchpos = touch.position;
                    var movementRange = 0.008f;
                    var movingRange = new Vector3(movementRange, 0, 0);

                    int edgeNum = int.Parse(rampEdge.transform.name);
                    var movingEdge = Enumerable.Repeat(rampTopEdges[edgeNum], 1);
                    var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();

                    if (newTouchpos.y - ramp2DTouchPosition.y > 0)
                    {
                        Debug.Log("edge moving up");
                        edgeHeights[edgeNum] += movementRange;
                        phase2Ramp.TranslateVertices(movingEdge, movingRange);
                        rampEdge.transform.localPosition += movingRange;
                        uiNumberControl.IncreaseCanvasY(movementRange / 4);
                    } else if (newTouchpos.y - ramp2DTouchPosition.y < 0 && edgeHeights[edgeNum] > 0f)
                    {
                        Debug.Log("edge moving down");
                        edgeHeights[edgeNum] -= movementRange;
                        phase2Ramp.TranslateVertices(movingEdge, -movingRange);
                        rampEdge.transform.localPosition -= movingRange;
                        uiNumberControl.IncreaseCanvasY(-movementRange / 4);
                    }
                    // set UI height: with edgeHeights[i]
                    phase2RampHeight = edgeHeights[edgeNum];
                    uiNumberControl.Height = phase2RampHeight / Constants.ONE_FEET;
                    phase2RampVolume = (float)(phase2RampHeight * uiNumberControl.area / Constants.ONE_FEET);
                    uiNumberControl.SetVolDisplay(System.Math.Round(phase2RampVolume, 2));
                    // add concrete text
                    concreteVolDisplay.text = "Vol: " + System.Math.Round(phase2RampVolume, 2) + " ft<sup>3</sup>";
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
                // drawing line logic
                if (currentLineRender != null && !isSnapping)
                {
                    DrawLineWithoutSnapping();
                }
            }

            // remove line logic
            if (touch.phase == TouchPhase.Ended)
            {
                if (GamePhase == Constants.GamePhase.PHASE2)
                {
                    // reset ramp positions and height
                    if (ramp2DTouchPosition != Vector2.zero && rampEdge != null)
                    {
                        if (phase2RampHeight <= 0)
                        {
                            SetRampEdgeCollider(true);
                        }
                        ramp2DTouchPosition = Vector2.zero;
                        rampEdge = null;
                    }
                    // snaping for ramp
                    if (phase2RampVolume > 1.8f && phase2RampVolume < 2.2f)
                    {
                        Debug.Log("completed ramp");
                        game2Manager.rampHeight = phase2RampHeight;
                        // phase 2 vol number snap
                        var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();
                        uiNumberControl.SetVolDisplay(2);
                        uiNumberControl.Height = System.Math.Round(phase2RampHeight / Constants.ONE_FEET);
                        concreteUIDisplay.SetActive(false);
                        concreteUIFill.fillAmount = 0;

                        game2Manager.StartPhase2End();
                        SetRampEdgeCollider(false);
                    }
                }
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
                        if (GamePhase == Constants.GamePhase.PHASE2)
                        {
                            phase2DrawnPos.Add(startPos);
                            phase2DrawnPos.Add(endPos);
                        }
                        HandleSnapObject();
                    }
                    ARDebugManager.Instance.LogInfo("let go. gamephase: " + GamePhase + "with numLines: " + numLines);
                    prevLineRender = currentLineRender;
                    currentLineRender = null;
                    startObject = null;
                    isSnapping = false;
                }

                if (canCubeLiftingSnap == true && startLiftCube)
                {
                    concreteUIFill.fillAmount = 0;
                    concreteUIDisplay.SetActive(false);
                    GameObject.FindObjectOfType<UINumberControl>().SetVolDisplay(1);
                    canCubeLiftingSnap = false;
                    game2Manager.EndPhase1();
                    g2SoundManager.PlayGoodSoundEffect();
                    liftableCube.GetComponent<BoxCollider>().enabled = false;
                }

                if(numLines == 4)
                {
                    startLiftCube = true;
                }
            }
        }

        if (GamePhase == Constants.GamePhase.PHASE3)
        {
            if (touchedPhase3Ramp != null && IsDoubleTap())
            {
                Destroy(touchedPhase3Ramp);
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
            // check if its square or rectangle based on the snapped dots
            Debug.Log("totalPos: " + phase2DrawnPos.Count);
            Vector3 minVector = Vector3.positiveInfinity;
            Vector3 maxVector = Vector3.zero;
            foreach (Vector3 pos in phase2DrawnPos)
            {
                //    minVector = Vector3.Min(pos, minVector);
                //    maxVector = Vector3.Max(pos, maxVector);
                minVector = (pos.magnitude < minVector.magnitude) ? pos : minVector;
                maxVector = (pos.magnitude > maxVector.magnitude) ? pos : maxVector;
            }
            if (minVector.x > maxVector.x) {
                (minVector, maxVector) = (maxVector, minVector);
            }
            Debug.Log("minVector positions: " + minVector);
            Debug.Log("maxVector positions: " + maxVector);
            phase2DrawnPos.Remove(minVector);
            phase2DrawnPos.Remove(maxVector);
            // check if a square/rec is formed
            float maxValue = 0.0f;
            foreach (Vector3 pos in phase2DrawnPos)
            {
                Debug.Log("leftover pos: " + pos);
                var minMag = (minVector - pos).magnitude;
                var maxMag = (maxVector - pos).magnitude;
                maxValue = Mathf.Max(minMag, maxMag);
                Debug.Log("minMag = : " + System.Math.Floor(minMag * 10f) + ", maxMag: " + System.Math.Floor(maxMag * 10f));
                if (System.Math.Floor(minMag * 10f) % 3 != 0 || System.Math.Floor(maxMag * 10f) % 3 != 0)
                {
                    Debug.Log("it is not a square / rectangle");
                    // reset -> add animation
                    numLines = 0;
                    g2SoundManager.playWrongDrawing();
                    ClearLines();
                    phase2DrawnPos.Clear();
                    return;
                }
            }

            // initialize ramp
            phase2Ramp = Instantiate(phase2Ramp, (minVector + maxVector)/2, phase2Ramp.transform.rotation);
            DotsManager.Instance.ClearDots();
            InitializeRampEdges();
            InitializeRampEdgeObjects();
            SetRampEdgeCollider(false);
            var volume = 0.0f;
            if (System.Math.Floor(maxValue * 10f) / 3 == 1)
            {
                Debug.Log("it is a square");
                phase2Ramp.transform.localScale = new Vector3(0.5f, Constants.ONE_FEET, Constants.ONE_FEET);
                volume = 1.0f;
            }
            else
            {
                Debug.Log("it is rect");
                phase2Ramp.transform.localScale = new Vector3(0.5f, 2 * Constants.ONE_FEET, Constants.ONE_FEET);
                volume = 2.0f;
            }
            // set initial volume and set up
            var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();
            uiNumberControl.SetAreaDisplay(volume);
            uiNumberControl.Height = 0;
            game2Manager.StartPhase2Mid();
            // set ramp start point & endpoint
            game2Manager.rampStartPoint = minVector; // - new Vector3(0, Constants.HALF_FEET, 0);
            game2Manager.rampEndPoint = maxVector; // + new Vector3(0, Constants.HALF_FEET, 0);
            numLines = 0;
        }

        if (GamePhase == Constants.GamePhase.PHASE1 && numLines == 4)
        {
            concreteUIDisplay.SetActive(true);
            Debug.Log("phase1 mid now!!!!!!!!!!!!!!!~~~~~~~~~~~~~~~~`");
            DotsManager.Instance.ActivatePhase1Cube();
        }
        isSnapping = false;
        currentLineRender = null;
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

    private void DeactivateOtherColliders(int edgeNum)
    {
        foreach (var gameObject in rampEdgeObjects)
        {
            if (gameObject.Key != edgeNum)
            {
                gameObject.Value.SetActive(false);
            }
        }
        if (rampTopFace != null)
        {
            if (edgeNum == 1) {
                rampTopFace.SetActive(true);
            }
            else
            {
                rampTopFace.SetActive(false);
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
