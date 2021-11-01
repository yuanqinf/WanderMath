using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGM : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    private string activity1SceneName = "Activity1";
    private string activity2SceneName = "Activity2";
    private string activity3SceneName = "Activity3";

    public void loadActivity1Scene()
    {
        SceneManager.LoadScene(activity1SceneName);
    }

    public void loadActivity2Scene()
    {
        SceneManager.LoadScene(activity2SceneName);
    }

    public void loadActivity3Scene()
    {
        SceneManager.LoadScene(activity3SceneName);
    }
}
