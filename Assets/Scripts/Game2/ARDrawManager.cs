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

public struct AnimationPoint
{
    public Vector3 startPos;
    public Vector3 endPos;
    public float height;
    public bool isJump;

    public AnimationPoint(Vector3 start, Vector3 end, float height, bool isJump)
    {
        this.startPos = start;
        this.endPos = end;
        this.height = height;
        this.isJump = isJump;
    }
};

[RequireComponent(typeof(ARAnchorManager))]
public class ARDrawManager : Singleton<ARDrawManager>
{
    [SerializeField]
    private Material defaultColorMaterial;
    [SerializeField]
    private Material failedColorMaterial;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private float lineWidth = 0.05f;
    private Color randomStartColor = Color.white;
    private Color randomEndColor = Color.white;

    // ramp and line related
    [SerializeField]
    private GameObject linePrefab;
    [SerializeField]
    private ProBuilderMesh genericRamp;
    private ProBuilderMesh phase2Ramp;
    private LineRenderer currentLineRender = null;
    private GameObject currentLineGameObject = null;

    private List<LineRenderer> lines = new List<LineRenderer>();
    private Dictionary<GameObject, int> dotsGameObjDict = new Dictionary<GameObject, int>();
    private Dictionary<int, Edge> rampTopEdges = new Dictionary<int, Edge>();
    private Dictionary<int, float> edgeHeights = new Dictionary<int, float>
    {
        {4, 0},
        {5, 0},
        {6, 0},
        {7, 0},
    };
    private Dictionary<int, GameObject> rampEdgeObjects = new Dictionary<int, GameObject>();
    private GameObject rampTopObject = null;
    private float phase2RampVolume = 0.0f;
    private float phase2RampHeight = 0.0f;
    private float prevVolume = 0.0f;
    public Vector2 ramp2DTouchPosition = Vector2.zero;
    public Vector2 rec2DTouchPosition = Vector2.zero;
    public GameObject rampEdgeCollider = null;
    private GameObject rampTopCollider = null;
    private float rampTopHeight = 0f;
    private List<AnimationPoint> phase3AnimationPoints = new List<AnimationPoint>();

    public string GamePhase { get; set; }

    public Vector3 startPos;
    public Vector3 endPos;
    public Vector3 movingTouchPosition;
    private GameObject startObject;
    private bool isSnapping = false;
    private int numLines = 0;

    private Game2Manager game2Manager;
    private Game2SoundManager g2SoundManager;

    private bool canCubeLiftingSnap = false;

    private GameObject liftableCube;
    // ui texts
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
        currentLineRenderer.numCornerVertices = 8;
        currentLineRenderer.numCapVertices = 8;
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
                // touched dot logic
                if (hitObject.transform.tag == Constants.Tags.DOT)
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
                        Debug.Log("points ratio is: " + ratio);

                        ARDebugManager.Instance.LogInfo("magnitude ratio is: " + ratio);
                        if ((ratio > 0.85 && ratio < 1.15) || (ratio > 1.85 && ratio < 2.15) || (ratio > 2.85 && ratio < 3.15) || (ratio > 3.85 && ratio < 4.15) || (ratio > 4.85 && ratio < 5.15))
                        {
                            numLines++;
                            isSnapping = true;
                            g2SoundManager.PlayGoodSoundEffect();
                        }
                        else if (ratio > 0.2)
                        {
                            game2Manager.PlayWrongDiagonalWithAnimation();
                        }
                    }
                }
                // touched various objects logic
                if (touch.phase == TouchPhase.Began)
                {
                    if (hitObject.transform.tag == Constants.Tags.RampEdge)
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
                    // only allowed to touch in phase3
                    if (hitObject.transform.tag == Constants.Tags.P3Ramp && GamePhase == Constants.GamePhase.PHASE3)
                    {
                        // Initiate face to be hit
                        rampTopCollider = hitObject.transform.gameObject;
                        // deactivate other edges, only left ramp
                        DeactivateOtherColliders(1);
                        // set ramp edge touch positions in 2D
                        ramp2DTouchPosition = touch.position;
                        Debug.Log("initialize rampface " + ramp2DTouchPosition);
                    }
                    if (hitObject.transform.tag == Constants.Tags.LiftableShape)
                    {
                        rec2DTouchPosition = touch.position;
                        liftableCube = hitObject.transform.gameObject;
                        Debug.Log("hitObject.transform.gameObject.name: " + hitObject.transform.gameObject.name);
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                var newTouchpos = touch.position;
                // moving rect after phase1
                if (rec2DTouchPosition != Vector2.zero && liftableCube != null)
                {
                    var movingRange = new Vector3(0, Constants.MOVEMENT_RANGE, 0);
                    var uINumberControl = liftableCube.transform.root.gameObject.GetComponent<UINumberControl>();
                    var curVolNum = (float)System.Math.Round((liftableCube.transform.parent.transform.localScale.y / 0.57f), 1);

                    if (curVolNum > 0.85 && curVolNum < 1.15)
                    {
                        //glow effect here
                        GameObject.FindGameObjectWithTag(Constants.Tags.ForceFieldMat).GetComponent<MeshRenderer>().enabled = true;
                        canCubeLiftingSnap = true;
                    }
                    else
                    {
                        GameObject.FindGameObjectWithTag(Constants.Tags.ForceFieldMat).GetComponent<MeshRenderer>().enabled = false;
                        canCubeLiftingSnap = false;
                    }
                    uINumberControl.SetVolDisplay(curVolNum);
                    concreteVolDisplay.text = "Vol: " + curVolNum + " ft<sup>3</sup>";
                    concreteUIFill.fillAmount = curVolNum;
                    ShowOverusedText(curVolNum, 1.15f);

                    if (newTouchpos.y - rec2DTouchPosition.y > 0 && curVolNum <= 5)
                    {
                        Debug.Log("lifting it now---------------------------------------");
                        // 3d shape lift
                        liftableCube.transform.parent.localScale += movingRange;
                        // 3d ui lift
                        liftableCube.transform.root.GetComponentInChildren<RectTransform>().localPosition += movingRange;
                    }
                    else if ((newTouchpos.y - rec2DTouchPosition.y < 0) && liftableCube.transform.parent.localScale.y >= 0)
                    {
                        Debug.Log("drag it down---------------------------------------");
                        liftableCube.transform.parent.localScale -= movingRange;
                        liftableCube.transform.root.GetComponentInChildren<RectTransform>().localPosition -= movingRange;
                    }
                }

                // moving ramp in phase2 on edges
                if (ramp2DTouchPosition != Vector2.zero && rampEdgeCollider != null)
                {
                    var movingRangeX = new Vector3(Constants.MOVEMENT_RANGE, 0, 0) ;
                    var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();
                    int edgeNum = int.Parse(rampEdgeCollider.transform.name);
                    var movingEdge = Enumerable.Repeat(rampTopEdges[edgeNum], 1);

                    if (newTouchpos.y - ramp2DTouchPosition.y > 0 && edgeHeights[edgeNum] <= Constants.FIVE_FEET)
                    {
                        edgeHeights[edgeNum] += Constants.MOVEMENT_RANGE; // tracking height movement
                        phase2Ramp.TranslateVertices(movingEdge, movingRangeX); // actual ramp moving
                        rampEdgeCollider.transform.localScale += movingRangeX * 15; // collider scaling
                        uiNumberControl.IncreaseCanvasY(Constants.MOVEMENT_RANGE / 4); // moving UI element upwards
                    } else if (newTouchpos.y - ramp2DTouchPosition.y < 0 && edgeHeights[edgeNum] > 0f)
                    {
                        edgeHeights[edgeNum] -= Constants.MOVEMENT_RANGE;
                        phase2Ramp.TranslateVertices(movingEdge, -movingRangeX);
                        rampEdgeCollider.transform.localScale -= movingRangeX * 15;
                        uiNumberControl.IncreaseCanvasY(-Constants.MOVEMENT_RANGE / 4);
                    }
                    // set UI height: with edgeHeights[i]
                    phase2RampHeight = edgeHeights[edgeNum];
                    uiNumberControl.Height = phase2RampHeight / Constants.ONE_FEET;
                    // formula for ramp: 0.5 * area * height
                    phase2RampVolume = (float)System.Math.Round(0.5 * phase2RampHeight * uiNumberControl.area / Constants.ONE_FEET, 1);
                    uiNumberControl.SetVolDisplay(phase2RampVolume);
                    // add concrete text & details
                    if (GamePhase == Constants.GamePhase.PHASE2)
                    {
                        concreteVolDisplay.text = "Vol: " + phase2RampVolume + " ft<sup>3</sup>";
                        concreteUIFill.fillAmount = phase2RampVolume;
                        ShowOverusedText(phase2RampVolume, 1.15f);
                    }
                    if (GamePhase == Constants.GamePhase.PHASE3)
                    {
                        var totalVol = (float)System.Math.Round(prevVolume + phase2RampVolume, 1);
                        Debug.Log("total vol: " + totalVol);
                        concreteVolDisplay.text = "Vol: " + totalVol + " ft<sup>3</sup>";
                        concreteUIFill.fillAmount = totalVol / Constants.FillAmountVolFor6;
                        ShowOverusedText(totalVol, 6.15f);
                    }

                    // activate glowing effect in phase2 only
                    var targetMat = phase2Ramp.GetComponent<Renderer>().material;
                    if (phase2RampVolume > 0.8f && phase2RampVolume < 1.2f && GamePhase == Constants.GamePhase.PHASE2 && targetMat.GetFloat("_EmissIntensity") != 1.2f) {
                        targetMat.SetFloat("_EmissIntensity", 1.2f);
                    }
                    else if (targetMat.GetFloat("_EmissIntensity") != 0.66f)
                    {
                        targetMat.SetFloat("_EmissIntensity", 0.66f);
                    }
                }
                // moving ramp face in phase3
                if (ramp2DTouchPosition != Vector2.zero && rampTopCollider != null)
                {
                    var movingRange = new Vector3(Constants.MOVEMENT_RANGE, 0, 0);
                    var topFace = rampTopCollider.transform.root.GetComponent<ProBuilderMesh>();
                    var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();

                    if (newTouchpos.y > ramp2DTouchPosition.y && rampTopHeight <= 5)
                    {
                        rampTopHeight += Constants.MOVEMENT_RANGE;
                        topFace.TranslateVertices(topFace.faces.ElementAt(1).edges, movingRange);
                        rampTopCollider.transform.localPosition += movingRange;
                        uiNumberControl.IncreaseCanvasY(Constants.MOVEMENT_RANGE / 4);
                    }
                    else if (newTouchpos.y < ramp2DTouchPosition.y && rampTopHeight > 0f)
                    {
                        rampTopHeight -= Constants.MOVEMENT_RANGE;
                        topFace.TranslateVertices(topFace.faces.ElementAt(1).edges, -movingRange);
                        rampTopCollider.transform.localPosition -= movingRange;
                        uiNumberControl.IncreaseCanvasY(-Constants.MOVEMENT_RANGE / 4);
                    }
                    // set UI height: with face height
                    uiNumberControl.Height = rampTopHeight / Constants.ONE_FEET;
                    phase2RampVolume = (float)(rampTopHeight * uiNumberControl.area / Constants.ONE_FEET);
                    uiNumberControl.SetVolDisplay((float)System.Math.Round(phase2RampVolume, 1));
                    // add concrete text
                    var totalVol = (float)System.Math.Round(prevVolume + phase2RampVolume, 1);
                    concreteVolDisplay.text = "Vol: " + totalVol + " ft<sup>3</sup>";
                    concreteUIFill.fillAmount = totalVol / Constants.FillAmountVolFor6;
                    ShowOverusedText(totalVol, 6.15f);
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
                        if (GamePhase == Constants.GamePhase.PHASE2 || GamePhase == Constants.GamePhase.PHASE3)
                        {
                            int currentVal;
                            dotsGameObjDict.TryGetValue(startObject.gameObject, out currentVal);
                            dotsGameObjDict[startObject.gameObject] = currentVal + 1;
                            dotsGameObjDict.TryGetValue(hitObject.transform.gameObject, out currentVal);
                            dotsGameObjDict[hitObject.transform.gameObject] = currentVal + 1;
                        }
                        HandleSnapObject();
                    }
                    ARDebugManager.Instance.LogInfo("let go. gamephase: " + GamePhase + "with numLines: " + numLines);
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
                    if (phase2RampVolume > 0.8f && phase2RampVolume < 1.2f && rampEdgeCollider != null)
                    {
                        int edgeNum = int.Parse(rampEdgeCollider.transform.name);

                        Debug.Log("completed ramp");
                        game2Manager.rampHeight = phase2RampHeight;
                        // phase 2 vol number snap
                        var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();
                        uiNumberControl.SetVolDisplay(1);
                        uiNumberControl.Height = System.Math.Round(phase2RampHeight / Constants.ONE_FEET);
                        concreteUIDisplay.SetActive(false);
                        concreteVolDisplay.text = "Vol: 0 ft<sup>3</sup>";
                        concreteUIFill.fillAmount = 0;
                        phase2RampVolume = 0;

                        (var animeStartPt, var animeEndPt) = GetRampAnimationPoints(edgeNum);
                        Debug.Log("animeStartPt: " + animeStartPt);
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
                // phase2 moving ramp & cube 
                if (GamePhase == Constants.GamePhase.PHASE3)
                {
                    var totalVol = (float)System.Math.Round(prevVolume + phase2RampVolume, 1);
                    Debug.Log("totalVol is: " + totalVol);
                    if (totalVol > 5.5f && totalVol < 6.5f && (rampEdgeCollider != null || rampTopCollider != null))
                    {
                        Debug.Log("phase3 snap is called");
                        // phase 2 vol number snap
                        var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();
                        var leftoverVol = (float)System.Math.Round(6 - prevVolume, 1);
                        uiNumberControl.SetVolDisplay(leftoverVol);
                        uiNumberControl.Height = System.Math.Round(leftoverVol / Constants.ONE_FEET);
                        concreteUIDisplay.SetActive(false);
                        concreteVolDisplay.text = "Vol: 6 ft<sup>3</sup>";
                        concreteUIFill.fillAmount = 0;
                        AddPhase3AnimationPoint();
                        // play phase 3 animations
                        game2Manager.StartPhase3End(phase3AnimationPoints);
                        // disable colliders
                        SetRampEdgeCollider(false);
                        SetRampTopCollider(false);
                    }
                    // reset ramp edge positions and height
                    if (ramp2DTouchPosition != Vector2.zero && rampEdgeCollider != null)
                    {
                        if (phase2RampHeight <= 0)
                        {
                            SetRampEdgeCollider(true);
                            SetRampTopCollider(true);
                        }
                        ramp2DTouchPosition = Vector2.zero;
                        rampEdgeCollider = null;
                    }
                    // reset ramp top 
                    if (ramp2DTouchPosition != Vector2.zero && rampTopCollider != null)
                    {
                        if (rampTopHeight <= 0)
                        {
                            SetRampEdgeCollider(true);
                            SetRampTopCollider(true);
                        }
                        ramp2DTouchPosition = Vector2.zero;
                        rampTopCollider = null;
                    }
                }

                if (canCubeLiftingSnap == true)
                {
                    concreteUIFill.fillAmount = 0;
                    concreteUIDisplay.SetActive(false);
                    FindObjectOfType<UINumberControl>().SetVolDisplay(1);
                    canCubeLiftingSnap = false;
                    game2Manager.EndPhase1();
                    g2SoundManager.PlayGoodSoundEffect();
                    GameObject.FindGameObjectWithTag("liftable_shape").GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
    }


    private void ShowOverusedText(float curVolNum, float limit)
    {
        if (curVolNum > limit)
        {
            concreteVolDisplay.text = "Overused";
            concreteUIFill.color = Color.red;
        }
        else
        {
            concreteUIFill.color = Color.white;
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
            GetConnectedDotsInDict();
            if (dotsGameObjDict.Count != 4)
            {
                Debug.Log("not a rectangle/square");
                game2Manager.PlayWrongLinesWithAnimation();
                numLines = 0;
                ClearLines();
                dotsGameObjDict.Clear();
                return; // not 4 unique points, so not rectangle
            }
            var dotPoints = GetSortedDotPoints();
            var maxLength = GetMaxPoints(dotPoints);

            var initializePos = Vector3.zero;
            foreach(var item in dotsGameObjDict)
            {
                initializePos += item.Key.transform.position;
            }

            InitializeRamp(initializePos / 4);
            phase2Ramp.transform.localScale = new Vector3(0.5f, maxLength * Constants.ONE_FEET, Constants.ONE_FEET);

            // set initial volume and set up
            var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();
            uiNumberControl.SetAreaDisplay(maxLength);
            uiNumberControl.Height = 0;
            game2Manager.StartPhase2Mid();
            numLines = 0;
        }

        if (GamePhase == Constants.GamePhase.PHASE3 && numLines == 4)
        {
            // same code as above
            if (!CheckIfRect()) return;
            var dotPoints = GetSortedDotPoints();
            (var maxWidth, var maxLength) = GetLenAndWidthPoints(dotPoints);

            var initializePos = Vector3.zero;
            foreach (var item in dotsGameObjDict)
            {
                initializePos += item.Key.transform.position;
            }

            InitializePhase3Ramp(initializePos / 4);
            phase2Ramp.transform.localScale = new Vector3(0.5f, maxLength * Constants.ONE_FEET, maxWidth * Constants.ONE_FEET);
            // set initial volume and set up
            var uiNumberControl = phase2Ramp.GetComponent<UINumberControl>();
            uiNumberControl.SetAreaDisplay(maxWidth * maxLength);
            uiNumberControl.Height = 0;
            // difference in code
        }
        isSnapping = false;
        currentLineRender = null;
    }

    private void GetConnectedDotsInDict()
    {
        var res = dotsGameObjDict.Where(i => i.Value == 2);
        foreach (var item in dotsGameObjDict)
        {
            if (item.Value == 2)
            {
                //item.Key;
            }
        }
    }

    /// <summary>
    /// animation points for ramp
    /// </summary>
    /// <param name="edgeNum"></param>
    /// <returns></returns>
    private (Vector3, Vector3) GetRampAnimationPoints(int edgeNum)
    {
        // settle which position to use for animation
        var topLeft = phase2Ramp.transform.Find("TopLeft").transform.position;
        var topRight = phase2Ramp.transform.Find("TopRight").transform.position;
        var botLeft = phase2Ramp.transform.Find("BotLeft").transform.position;
        var botRight = phase2Ramp.transform.Find("BotRight").transform.position;

        var animeEndPt = Vector3.zero;
        var animeStartPt = Vector3.zero;
        switch (edgeNum)
        {
            case 4:
                animeEndPt = (topRight + botRight) / 2; // change x to prevent hitting the ramp when jump down
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
                animeEndPt = (topLeft + botLeft) / 2; // change x to prevent hitting the ramp when jump down
                animeStartPt = (topRight + botRight) / 2;
                break;
            case 1:
                animeEndPt = (topLeft + botLeft) / 2;
                animeStartPt = (topRight + botRight) / 2;
                break;
        }
        return (animeStartPt, animeEndPt);
    }

    private bool CheckIfRect()
    {
        if (dotsGameObjDict.Count != 4)
        {
            Debug.Log("not a rectangle/square");
            game2Manager.PlayWrongLinesWithAnimation();
            numLines = 0;
            ClearLines();
            dotsGameObjDict.Clear();
            return false; // not 4 unique points, so not rectangle
        }
        return true;
    }

    /// <summary>
    /// Get length and width of the rect drawn.
    /// </summary>
    /// <param name="dotPoints"></param>
    /// <returns></returns>
    private (int, int) GetLenAndWidthPoints(List<(int, int)> dotPoints)
    {
        var width = dotPoints[2].Item1 - dotPoints[1].Item1;
        var length = dotPoints[1].Item2 - dotPoints[0].Item2;

        return (width, length);
    }

    /// <summary>
    /// get the max length of the drawn rect.
    /// </summary>
    /// <param name="dotPoints"></param>
    /// <returns></returns>
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
        return res;
    }

    /// <summary>
    /// Get the list of drawn sorted dot points in tuple.
    /// </summary>
    /// <returns></returns>
    private List<(int, int)> GetSortedDotPoints()
    {
        List<(int, int)> dotPoints = new List<(int, int)>();
        foreach(var item in dotsGameObjDict)
        {
            var res = item.Key.name.Split('_');
            dotPoints.Add((int.Parse(res[1]), int.Parse(res[2])));
        }
        dotPoints.Sort();
        return dotPoints;
    }

    private void InitializePhase3Ramp(Vector3 middlePos)
    {
        // get points based on rectangle or ramp
        if (phase2Ramp != null)
        {
            AddPhase3AnimationPoint();
        }

        // re-initalize references
        if (prevVolume == 0.0f) prevVolume = phase2RampVolume;
        // disable colliders
        SetRampEdgeCollider(false);
        DeactivateSelectedDots();
        dotsGameObjDict.Clear();

        if (rampTopObject != null) SetRampTopCollider(false);
        ClearRampRefereces();

        phase2Ramp = Instantiate(genericRamp, middlePos, genericRamp.transform.rotation);
        concreteUIDisplay.SetActive(true);
        ClearLines();
        InitializeRampEdges();
        InitializeRampEdgeObjects(phase2Ramp.gameObject);
        InitializeRampTopObject(phase2Ramp.gameObject);
        SetRampEdgeCollider(true); // TODO: change when audio changes
    }
    private void AddPhase3AnimationPoint()
    {
        // find edge/top
        (var rampPoint, var rampHeight) = FindSelectedEdgeOrTop();
        // get ramp animation endpoints
        (var startPoint, var endPoint) = GetRampAnimationPoints(rampPoint);
        var isJump = (rampPoint == 1) ? true : false;
        phase3AnimationPoints.Add(new AnimationPoint(startPoint, endPoint, rampHeight, isJump));
    }

    /// <summary>
    /// Deactivate dots interactions.
    /// </summary>
    private void DeactivateSelectedDots()
    {
        var dotNameSet = new HashSet<string>();
        var sortedDots = GetSortedDotPoints();
        for (int d = 1; d < sortedDots.Count; d++)
        {
            for (int i = sortedDots[0].Item1; i <= sortedDots[d].Item1; i++)
            {
                for (int j = sortedDots[0].Item2; j <= sortedDots[d].Item2; j++)
                {
                    dotNameSet.Add($"dot_{i}_{j}");
                }
            }
        }

        var allDots = GameObject.FindGameObjectsWithTag(Constants.Tags.DOT);
        foreach (var dotObj in allDots)
        {
            if (dotNameSet.Contains(dotObj.name)) {
                dotObj.GetComponent<MeshRenderer>().material = failedColorMaterial;
                dotObj.GetComponent<Collider>().enabled = false;
            }
        }
    }

    /// <summary>
    /// Initialize ramp, edges and colliders.
    /// </summary>
    /// <param name="middlePos"></param>
    private void InitializeRamp(Vector3 middlePos)
    {
        phase2Ramp = Instantiate(genericRamp, middlePos, genericRamp.transform.rotation);
        DotsManager.Instance.ClearDots();
        dotsGameObjDict.Clear();
        phase2Ramp.transform.Find("top").gameObject.SetActive(false);
        InitializeRampEdges();
        InitializeRampEdgeObjects(phase2Ramp.gameObject);
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

    private (int, float) FindSelectedEdgeOrTop()
    {
        var heightNum = 0;
        var height = 0f;

        if (rampTopHeight > 0)
        {
            heightNum = 1;
            height = rampTopHeight;
        }
        foreach(var item in edgeHeights)
        {
            if (item.Value > 0)
            {
                heightNum = item.Key;
                height = item.Value;
            }
        }
        return (heightNum, height);
    }

    private void SetRampTopCollider(bool isActive)
    {
        Debug.Log("rampTopObject is: " + rampTopObject.name);
        rampTopObject.SetActive(isActive);
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
        if (rampTopObject != null)
        {
            if (edgeNum == 1) {
                SetRampTopCollider(true);
            }
            else
            {
                SetRampTopCollider(false);
            }
        }
    }

    private void InitializeRampTopObject(GameObject ramp)
    {
        rampTopObject = ramp.transform.Find("top").gameObject;
    }
    // only initialize when its parent
    private void InitializeRampEdgeObjects(GameObject parent)
    {
        GameObject[] edgeObjects = GameObject.FindGameObjectsWithTag("rampEdge");
        foreach(GameObject edge in edgeObjects)
        {
            if (edge.transform.IsChildOf(parent.transform))
            {
                Debug.Log("edge object found: " + edge.gameObject.name);
                rampEdgeObjects.Add(int.Parse(edge.gameObject.name), edge);
            }
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
        currentLineGameObject = Instantiate(linePrefab, touchPosition, Quaternion.identity);
        currentLineGameObject.name = $"LineRenderer_{lines.Count}";
        //currentLineGameObject.AddComponent<ARAnchor>();
        currentLineGameObject.tag = "Line";
        LineRenderer goLineRenderer = currentLineGameObject.AddComponent<LineRenderer>();
        goLineRenderer.startWidth = lineWidth;
        goLineRenderer.endWidth = lineWidth;
        goLineRenderer.material = defaultColorMaterial;
        goLineRenderer.useWorldSpace = true;
        goLineRenderer.positionCount = 2;
        goLineRenderer.SetPosition(0, currentLineGameObject.transform.position);
        goLineRenderer.SetPosition(1, currentLineGameObject.transform.position);
        //goLineRenderer.numCapVertices = 90;

        SetLineSettings(goLineRenderer);

        currentLineRender = goLineRenderer;
        lines.Add(goLineRenderer);

        ARDebugManager.Instance.LogInfo($"New line renderer created");
    }

    private void ClearRampRefereces()
    {
        rampTopEdges.Clear();
        rampEdgeObjects.Clear();
        dotsGameObjDict.Clear();
        edgeHeights.Keys.ToList().ForEach(k => edgeHeights[k] = 0);
        rampTopObject = null;
        rampTopHeight = 0.0f;
    }
    // destroy ramp to calculate next ramp
    public void DestoryRampAndReferences()
    {
        Destroy(phase2Ramp.gameObject);
        phase2Ramp = null;
        ClearRampRefereces();
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
    #endregion
}
