using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressSceneLoader : MonoBehaviour
{
	public GameObject canvas;
	[SerializeField]
	private Text progressText;
	[SerializeField]
	private Image slider;

	private AsyncOperation operation;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void LoadScene(string sceneName)
	{
		UpdateProgressUI(0);
		canvas.gameObject.SetActive(true);
		StartCoroutine(BeginLoad(sceneName));
	}

	private IEnumerator BeginLoad(string sceneName)
	{
		operation = SceneManager.LoadSceneAsync(sceneName);

		while (!operation.isDone)
		{
			UpdateProgressUI(operation.progress);
			yield return null;
		}

		UpdateProgressUI(operation.progress);
		operation = null;
		canvas.gameObject.SetActive(false);
	}

	private void UpdateProgressUI(float progress)
	{
		slider.fillAmount = progress;
		progressText.text = (int)(progress * 100f) + "%";
	}
}