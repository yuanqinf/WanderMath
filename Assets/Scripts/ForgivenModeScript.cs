using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgivenModeScript : MonoBehaviour
{
    public GameObject[] faces;

    public void AutoSnapAll()
    {
        foreach (var face in faces)
        {
            face.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 90);
        }
    }
}
