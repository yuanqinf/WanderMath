using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongCubeSnap : MonoBehaviour
{
    public int snapNum = 0;
    private SoundManager soundManager;


    // Start is called before the first frame update
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(snapNum >= 3)
        {
            float time = soundManager.PlayPhase2WrongCube();
            Destroy(this.gameObject, time);
        }
    }

    public void increaseSnapNum()
    {
        Debug.Log("increase snap number from WrongCubeSnap");
        snapNum += 1;
    }
}
