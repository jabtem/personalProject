using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum Child {LoadingBar}
enum PlayerCharacter {Katana = 1,Spear, TwohandSword};

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

    PostProcessingManager _postProcessing;
    public PostProcessingManager PostProcessing
    {
        set
        {
            if(_postProcessing == null)
            {
                _postProcessing = value;
            }

        }
        get
        {
            return _postProcessing;
        }
    }

    EffectManager _effect;
    public EffectManager Effect
    {
        set
        {
            if(_effect == null)
            {
                _effect = value;
            }

        }
        get
        {
            return _effect;
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

    void ResetTimeSlow()
    {
        Time.timeScale = 1f;
        MotionTrailObjectPoolManager.instance.motionTrailOn = false;
        PostProcessing.TimeSlowEffect(0f);
    }

    public void InvokeResetTimeSlow(float time)
    {
        Invoke("ResetTimeSlow", time);
        Debug.Log("TimeReset!");
    }





}
