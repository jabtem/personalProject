using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoadSceneManger : MonoBehaviour
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

    public void LoadScene(int num)
    {
        SceneManager.LoadScene(num);
        SceneManager.sceneLoaded += SceneLoadedEnd;
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
        if(loadingBarCanvas.gameObject.activeSelf)
            loadingBarCanvas.gameObject.SetActive(false);

        //전환된씬이 캐릭터 선택창인경우 버튼세팅
        if(scene.buildIndex == 1)
        {
            Button butt = GameObject.FindObjectOfType<Button>();
            butt.onClick.RemoveAllListeners();
            butt.onClick.AddListener(delegate { LoadingScene(2); });
        }

        SceneManager.sceneLoaded -= SceneLoadedEnd;

    }
}
