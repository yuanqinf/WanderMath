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

    // Start is called before the first frame update
    void Start()
    {
        startPos = startObj.transform.position;
        lr = GetComponent<LineRenderer>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            mousePos = raycastHit.point;

        }
        mouseDir = mousePos - gameObject.transform.position;
        mouseDir.y = 0;
        mouseDir = mouseDir.normalized;

        if (Input.GetMouseButtonDown(0))
        {
            lr.enabled = true;
        }

        if (Input.GetMouseButton(0))
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
            lr.enabled = false;
        }
    }
}
