using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;


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
    }

    public void LoadScene(int num)
    {
        SceneManager.LoadScene(num);
    }
}
