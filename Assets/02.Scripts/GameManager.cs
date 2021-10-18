using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum Child {LoadinBar}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    GameManager[] objs;
    GameObject loading;
    LoadSceneManger loadSceneManager;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("GameManager is NULL");
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        objs = FindObjectsOfType<GameManager>();
        loading = transform.GetChild((int)Child.LoadinBar).gameObject;
        loadSceneManager = gameObject.GetComponent<LoadSceneManger>();
        //게임매니저는 항상 한개만 존재해야한다
        //여러개 존재하면 하나빼고 다삭제
        if (objs.Length != 1)
        {
            for (int i = 1; i < objs.Length; i++)
            {
                Destroy(objs[i].gameObject);
            }
        }
    }

    public void LoadScene(int num)
    {
        loadSceneManager.LoadScene(num);
    }
    public void LoadLoadingScene(int num)
    {
        //로딩화면을불러옴(UI)
        //UI를 활용해서 하는게 프레임드랍이 적다 
        loadSceneManager.LoadingScene(num);

        //로딩씬불러옴(씬)
        //LoadingManger2.LoadScene(num);
    }
}
