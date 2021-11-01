using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinleyAction : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera;

    // Start is called before the first frame update
    void Start()
    {
        arCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // raycasting to hit the point
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hitObject))
            {
                // touched dot logic
                if (hitObject.transform.tag == "finley")
                {
                    foreach (var tap in Input.touches)
                    {
                        if (tap.tapCount == 2)
                        {
                            SceneManager.LoadScene("main");
                        }
                    }
                }
            }
        }
    }

    private bool IsDoubleTap()
    {
        bool result = false;
        float MaxTimeWait = 2;
        float VariancePosition = 2;

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            float DeltaTime = Input.GetTouch(0).deltaTime;
            float DeltaPositionLength = Input.GetTouch(0).deltaPosition.magnitude;

            if (DeltaTime > 0 && DeltaTime < MaxTimeWait && DeltaPositionLength < VariancePosition)
                result = true;
        }
        return result;
    }
}
