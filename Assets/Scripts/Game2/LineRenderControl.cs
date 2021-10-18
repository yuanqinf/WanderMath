using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderControl : MonoBehaviour
{
    public GameObject startObj;
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

    // Start is called before the first frame update
    void Start()
    {
        if(lineObj != null)
        {

        }
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            mousePos = raycastHit.point;
            Debug.Log(raycastHit.transform.tag);
            if(raycastHit.transform.tag == "dot")
            {
                if (isDragging && raycastHit.transform.name != startObj.name)
                {
                    Debug.Log("should snap snap now!!!");
                    Debug.Log("raycastHit.transform.name: " + raycastHit.transform.name);
                    Debug.Log("startObj.name: " + startObj.name);
                    isSnapping = true;
                }
                startObj = raycastHit.transform.gameObject;
            }

        }
        mouseDir = mousePos - gameObject.transform.position;
        mouseDir.y = 0;
        mouseDir = mouseDir.normalized;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lineObj = Instantiate(lineObjPrefab);
            lr = lineObj.GetComponent<LineRenderer>();
        }

        if (Input.GetMouseButton(0) && !isSnapping)
        {
            startPos = startObj.transform.position;
            startPos.y = 0;
            lr.SetPosition(0, startPos);
            endPos = mousePos;
            endPos.y = 0;
            lr.SetPosition(1, endPos);
        }

        if (Input.GetMouseButtonUp(0))
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
