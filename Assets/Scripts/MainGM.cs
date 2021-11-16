using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;


public class MainGM : MonoBehaviour
{
    public GameObject SettingPanel;

    public GameObject[] GameObjectsShouldHide;

    public GameObject LoadCanvas;
    public Image ProgressBar;

    public GameObject vpObject;

    public Animator animator;

    public Button settingBtn;

    // Start is called before the first frame update
    private void Start()
    {
        //CloseSettingMenu();
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    private void Update()
    {
        if (GameObject.FindObjectOfType<AudioSource>() != null && GameObject.FindObjectOfType<AudioSource>().isPlaying)
        {
            settingBtn.interactable = false;
        }
        else
        {
            settingBtn.interactable = true;
        }
    }

    public void BackMain()
    {
        SceneManager.LoadScene(Constants.Scenes.MainMenu);
        SetGameObjectsShouldHide();
        Destroy(this.gameObject);
    }

    public void LoadActivity1Scene()
    {
        SetGameObjectsShouldHide();
        FindObjectOfType<ProgressSceneLoader>().LoadScene(Constants.Scenes.Activity1);
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadActivity2Scene()
    {
        SetGameObjectsShouldHide();
        FindObjectOfType<ProgressSceneLoader>().LoadScene(Constants.Scenes.Activity2);
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadActivity3Scene()
    {
        SetGameObjectsShouldHide();
        FindObjectOfType<ProgressSceneLoader>().LoadScene(Constants.Scenes.Activity3);
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadTestScene()
    {
        SetGameObjectsShouldHide();
        SceneManager.LoadScene(5);
        DontDestroyOnLoad(this.gameObject);
    }

    public void OpenSettingMenu()
    {
        SettingPanel.SetActive(true);
        animator.SetTrigger("pop");
    }

    public void CloseSettingMenu()
    {
        animator.SetTrigger("fold");
    }

    private void SetGameObjectsShouldHide()
    {
        for(int i = 0; i < GameObjectsShouldHide.Length; i++)
        {
            GameObjectsShouldHide[i].SetActive(false);
        }
    }

    public void ResetCurrentPhase()
    {
        if(SceneManager.GetActiveScene().name == Constants.Scenes.Activity1)
        {
            var gameController = FindObjectOfType<GameController>();
            gameController.ResetPhase();

        }
        if (SceneManager.GetActiveScene().name == Constants.Scenes.Activity2)
        {
            var gameController = FindObjectOfType<Game2Manager>();
            gameController.resetPhase();
        }
        if (SceneManager.GetActiveScene().name == Constants.Scenes.Activity3)
        {
            var gameController = FindObjectOfType<Game3Controller>();
            gameController.ResetPhase();
        }
        SettingPanel.SetActive(false);
    }

    public void ShowCutScene()
    {
        vpObject.SetActive(true);
        SetGameObjectsShouldHide();
        PlayCutScene();
    }

    private void PlayCutScene()
    {
        vpObject.GetComponent<VideoPlayer>().enabled = true;
        vpObject.GetComponent<VideoPlayer>().loopPointReached += CheckOver;
    }

    private void CheckOver(VideoPlayer vp)
    {
        vpObject.SetActive(false);
        SceneManager.LoadScene(Constants.Scenes.MainMenu);
    }
}
