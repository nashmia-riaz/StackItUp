using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public GameObject MenuPanel, ArcadePanel, LoadingPanel, BackButton;
    public Image loadingBar;
    
    private GameObject currentPanel;

    private void Start()
    {
        currentPanel = MenuPanel;
    }

    public void OnClickEndless()
    {
        NextPanel(LoadingPanel);
        BackButton.SetActive(false);
        StartCoroutine(LoadLevel("Endless Scene"));
    }

    public void OnClickArcade()
    {
        NextPanel(ArcadePanel);
        BackButton.SetActive(true);
        BackButton.GetComponent<Button>().onClick.AddListener(()=> {
            NextPanel(MenuPanel);
            BackButton.SetActive(false);
        });
    }
    
    void NextPanel(GameObject panel) {
        currentPanel.SetActive(false);
        panel.SetActive(true);
        currentPanel = panel;
    }

    public void OnClickArcadeBurger(int mode)
    {
        PlayerPrefs.SetInt("RecipeSelected", mode);
        StartCoroutine(LoadLevel("Arcade Scene"));
        NextPanel(LoadingPanel);
        BackButton.SetActive(false);
    }
    
    IEnumerator LoadLevel(string sceneName)
    {
        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            loadingBar.fillAmount = async.progress;
            if (async.progress >= 0.9f)
            {
               async.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
