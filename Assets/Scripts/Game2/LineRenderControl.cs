using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderControl : MonoBehaviour
{
    private GameObject startObj;
    public Vector3 startPos;
    public Vector3 endPos;
    public Vector3 mousePos;
    public Vector3 mouseDir;
    private Camera cam;
    private LineRenderer lr;

    public GameObject lineObjPrefab;
    public GameObject lineObj;

    private bool isDragging = false;
    private bool isSnapping = false;

    public int curedgeCount = 0;
    private int squareEdgeCount = 4;

    public HashSet<GameObject> connectedDots;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("this is connectedDots: " + connectedDots);

        if (Input.touchCount > 0 && curedgeCount < squareEdgeCount)
        {
            Touch touch = Input.GetTouch(0);

            Ray ray = cam.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                mousePos = raycastHit.point;
                if(raycastHit.transform.tag == "dot")
                {
                    if (isDragging && raycastHit.transform.position != startObj.transform.position)
                    {
                        connectedDots.Add(startObj);
                        connectedDots.Add(raycastHit.transform.gameObject);
                        isSnapping = true;
                        curedgeCount += 1;
                    }
                    startObj = raycastHit.transform.gameObject;
                }

            }
            mouseDir = mousePos - gameObject.transform.position;
            mouseDir.y = 0;
            mouseDir = mouseDir.normalized;

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lineObj = Instantiate(lineObjPrefab);
                lr = lineObj.GetComponent<LineRenderer>();
            }

            if (touch.phase == TouchPhase.Moved && !isSnapping)
            {
                startPos = startObj.transform.position;
                lr.SetPosition(0, startPos);
                endPos = mousePos;
                endPos.y = startPos.y;
                lr.SetPosition(1, endPos);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
                startObj = null;
                if (!isSnapping)
                {
                    Destroy(lineObj);
                }
                isSnapping = false;
            }
        }
    }
}
