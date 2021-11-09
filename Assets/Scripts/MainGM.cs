using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGM : MonoBehaviour
{
    public GameObject SettingPanel;

    public GameObject[] GameObjectsShouldHide;

    public GameObject LoadCanvas;
    public Image ProgressBar;

    // Start is called before the first frame update
    private void Start()
    {
        closeSettingMenu();
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    private string activity1SceneName = "Activity1";
    private string activity2SceneName = "Activity2";
    private string activity3SceneName = "Activity3";

    public void backMain()
    {
        SceneManager.LoadScene("main");
        setGameObjectsShouldHide();
        Destroy(this.gameObject);
    }

    public void loadActivity1Scene()
    {
        setGameObjectsShouldHide();
        FindObjectOfType<ProgressSceneLoader>().LoadScene(activity1SceneName);
        DontDestroyOnLoad(this.gameObject);
    }

    public void loadActivity2Scene()
    {
        setGameObjectsShouldHide();
        FindObjectOfType<ProgressSceneLoader>().LoadScene(activity2SceneName);
        DontDestroyOnLoad(this.gameObject);
    }

    public void loadActivity3Scene()
    {
        setGameObjectsShouldHide();
        FindObjectOfType<ProgressSceneLoader>().LoadScene(activity3SceneName);
        DontDestroyOnLoad(this.gameObject);
    }

    public void openSettingMenu()
    {
        SettingPanel.SetActive(true);
    }

    public void closeSettingMenu()
    {
        SettingPanel.SetActive(false);
    }

    private void setGameObjectsShouldHide()
    {
        for(int i = 0; i < GameObjectsShouldHide.Length; i++)
        {
            GameObjectsShouldHide[i].SetActive(false);
        }
    }

    public void resetCurrentPhase()
    {
        if(SceneManager.GetActiveScene().name == activity1SceneName)
        {
            var gameController = FindObjectOfType<GameController>();
            gameController.resetPhase();

        }
        // TODO
        //if (SceneManager.GetActiveScene().name == activity2SceneName)
        //{
        //    var gameController = FindObjectOfType<Game2Manager>();
        //}
        //if (SceneManager.GetActiveScene().name == activity3SceneName)
        //{
        //    var gameController = FindObjectOfType<Game3Controller>();
        //}
        SettingPanel.SetActive(false);
    }
}
