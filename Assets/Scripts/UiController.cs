using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    [SerializeField]
    private Canvas canvasUi;
    [SerializeField]
    private GameObject preStartText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void setPreStartText(bool active)
    {
        preStartText.SetActive(active);
        //Debug.Log(preStartText.GetComponent<TextMeshPro>().text);
    }
}
