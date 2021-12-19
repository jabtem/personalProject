using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum Child {LoadingBar}
enum PlayerCharacter {Oriental = 1,Dagger, Axe};

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    GameManager[] objs;
    GameObject loading;
    LoadSceneManger loadSceneManager;

    //플레이어 타임스케일관리용 플레이어 행동을 정지시켜야될때 사용
    public float playerTimeScale = 1f;

    //플레이어 애니메이션 속도관리용
    Animator playerAnim;

    [SerializeField]
    PostProcessingManager _postProcessingManager;
    public PostProcessingManager postProcessingManager
    {
        set
        {
            if(_postProcessingManager == null)
                _postProcessingManager = value;
        }
    }



    //캐릭터 넘버
    public int CharacterNum = 0;

    private void Awake()
    {

        if (instance == null)
            instance = this;
        //게임매니저는 항상 한개만 존재해야한다
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        loading = transform.GetChild((int)Child.LoadingBar).gameObject;
        loadSceneManager = gameObject.GetComponent<LoadSceneManger>();
        Application.targetFrameRate = 60;

    }

    void Update()
    {

        //일시정지확인용 테스트코드
        if(Input.GetKeyDown(KeyCode.Space))
        {

            if (Time.timeScale >= 0.1)
            {
                SetTimeScale(0);
                SetPlayerAnimSpeed(0);
                playerTimeScale = 0;
            }
            else
            {
                SetTimeScale(1);
                SetPlayerAnimSpeed(1);
                playerTimeScale = 1f;
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

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Debug.Log("Set Time!" + timeScale);
    }
    public void SetPlayerAnimSpeed(float speed)
    {
        playerAnim.speed = speed;
    }

    public void SetPlayerAnim(Animator anim)
    {
        playerAnim = anim;
    }

    void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    public void InvokeResetTimeScale(float time)
    {
        Invoke("ResetTimeScale", time);
        Debug.Log("TimeReset!");
    }





}
