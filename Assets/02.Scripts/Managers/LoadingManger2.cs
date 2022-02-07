using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoadingManger2 : MonoBehaviour
{
    public Image loadingBar;
    static int next = 0;

    void Awake()
    {
        loadingBar.fillAmount = 0;
    }

    void Start()
    {
        StartCoroutine(Load());
    }

    public static void LoadScene(int sceneIndex)
    {
        next = sceneIndex;
        SceneManager.LoadScene(3);
    }


    private IEnumerator Load()
    {
        yield return new WaitForSeconds(1f);

        loadingBar.fillAmount = 0;
        float timer =0;
        AsyncOperation operation = SceneManager.LoadSceneAsync(next);
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


}
