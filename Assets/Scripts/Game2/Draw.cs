using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    public GameObject spacePenPoint;
    public GameObject surfacePenPoint;

    public GameObject stroke;
    public bool mouseLookTesting;
    public static bool drawing = false;

    private float pitch = 0;
    private float yaw = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseLookTesting)
        {
            yaw += 2 * Input.GetAxis("Mouse X");
            pitch -= 2 * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        if (PenManager.drawingOnSurface)
        {
            spacePenPoint.GetComponent<MeshRenderer>().enabled = false;
            surfacePenPoint.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            surfacePenPoint.GetComponent<MeshRenderer>().enabled = false;
            spacePenPoint.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void StartStroke()
    {
        GameObject currentStroke;
        drawing = true;
        currentStroke = Instantiate(stroke, spacePenPoint.transform.position, spacePenPoint.transform.rotation) as GameObject;
    }

    public void EndStroke()
    {
        drawing = false;
    }
}
