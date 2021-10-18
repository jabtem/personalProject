using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoadingManger : MonoBehaviour
{
    public Image loadingBar;
    public CanvasGroup loadingBarCanvas;

    void Awake()
    {
        loadingBar.fillAmount = 0;
    }

    public void LoadingScene(int num)
    {
        loadingBarCanvas.gameObject.SetActive(true);
        SceneManager.sceneLoaded += SceneLoadedEnd;
        StartCoroutine(Load(num));
    }

    private IEnumerator Load(int sceneIndex)
    {
        loadingBar.fillAmount = 0;
        float timer =0;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;
        while(!operation.isDone)
        {
            
            yield return null;
            timer += Time.unscaledDeltaTime;

            if (operation.progress<0.9f)
            {
                Debug.LogError(operation.progress);
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, operation.progress, timer);
                if(loadingBar.fillAmount >= operation.progress)
                {
                    timer = 0f;
                }

            }
            else
            { 
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 1f, timer);

                if(loadingBar.fillAmount == 1.0f)
                {
                    operation.allowSceneActivation = true;
                    yield break;
                }
            }

        }

    }

    private void SceneLoadedEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        loadingBarCanvas.gameObject.SetActive(false);
        SceneManager.sceneLoaded -= SceneLoadedEnd;

    }
}
