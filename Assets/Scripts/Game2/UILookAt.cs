using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UILookAt : MonoBehaviour
{
    private Camera cam;
    public TextMeshProUGUI lenDisplay;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        lenDisplay.transform.LookAt(transform.position + cam.transform.forward);
    }
}
