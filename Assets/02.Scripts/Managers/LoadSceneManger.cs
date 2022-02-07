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
        //현재 활성화된씬이 캐릭선택창인경우
        if(SceneManager.GetActiveScene().buildIndex == 1 && GameManager.instance.CharacterNum == 0)
        {
            Debug.Log("캐릭터를 선택하세요");
            return;
        }

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

            loadingBar.fillAmount = Mathf.Lerp(0, 1f, operation.progress);


            if(operation.progress>=0.9f)
            {
                operation.allowSceneActivation = true;
            }
            //if (operation.progress < 0.9f)
            //{
            //    loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, operation.progress, timer);
            //    if (loadingBar.fillAmount >= operation.progress)
            //    {
            //        timer = 0f;
            //    }

            //}
            //else
            //{
            //    loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 1f, timer);

            //    if (loadingBar.fillAmount == 1.0f)
            //    {
            //        operation.allowSceneActivation = true;
            //        yield break;
            //    }
            //}

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
        else if(scene.buildIndex == 2)
        {
            CreatePlayer obj = GameObject.FindObjectOfType<CreatePlayer>();
            obj.Create(GameManager.instance.CharacterNum);
            Button backButt;
            GameObject.FindGameObjectWithTag("BackButton").TryGetComponent<Button>(out backButt);

            backButt.onClick.RemoveAllListeners();
            backButt.onClick.AddListener(() => { LoadingScene(1); });
        }

        SceneManager.sceneLoaded -= SceneLoadedEnd;

    }
}
