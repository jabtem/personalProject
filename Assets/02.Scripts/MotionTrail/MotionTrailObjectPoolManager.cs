using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrailObjectPoolManager : MonoBehaviour
{
    public static MotionTrailObjectPoolManager instance;

    Stack<GameObject> motionTrailContainerStack = new Stack<GameObject>();

    [SerializeField]
    bool _motionTrailOn;


    public bool motionTrailOn
    {
        get
        {
            return _motionTrailOn;
        }
        set
        {
            _motionTrailOn = value;
        }
    }


    int index = 0;

    public Material motionTrailMat;
    public Gradient motionTrailGradient = new Gradient()
    {
        colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(new Color(1.0f, 0.0f, 0.0f), 0.00f),
            new GradientColorKey(new Color(0.0f, 1.0f, 0.0f), 0.50f),
            new GradientColorKey(new Color(0.0f, 0.0f, 1.0f), 1.0f),
        }
    };

    [Header("잔상색상 변화속도")]
    [Range(0.01f, 1f)]
    public float colorChangeSpeed = 1f;

    [Header("잔상생성 주기")]
    [Range(0.1f, 2f)]
    public float motionTrailCycle = 0.1f;


    [Header("쉐이더 색상조절 속성이름")]
    public string colorValueName;
    [Header("쉐이더 투명도조절 속성이름")]
    public string alphaValueNmae;


    float colorChangeTime = 0f;
    float motionTarilTime = 0f;



    private void Awake()
    {
        //SingleTion
        if (instance == null)
            instance = this;
        else if(instance != null)
        {
            Destroy(this.gameObject);
        }
    }


    void Update()
    {
        if(_motionTrailOn)
        {
            if(colorChangeTime >1.0f)
            {
                colorChangeTime = 0f;
            }

            motionTarilTime += Time.deltaTime;
            colorChangeTime += colorChangeSpeed;

            if (motionTarilTime > motionTrailCycle)
            {
                GetMotionTrailContainer();
                motionTarilTime = 0f;
            }
        }
    }

    void CreateMotionTrailContainer()
    {
        Color color = motionTrailGradient.Evaluate(colorChangeTime);
        GameObject mtContainerObj = new GameObject($"MotionTrailContianer {index++}");
        MotionTrailContainer mtContainer = mtContainerObj.AddComponent<MotionTrailContainer>();
        mtContainer.MotionTrailContainerSetting(index,motionTrailMat, colorValueName, alphaValueNmae);
        mtContainer.ColorSet(color);
        mtContainerObj.transform.SetParent(this.transform);
        mtContainerObj.SetActive(false);
        motionTrailContainerStack.Push(mtContainerObj);
    }
    


    void GetMotionTrailContainer()
    {
        GameObject reqObject = null;

        //개수가 모자라면 추가로 생성
        if (motionTrailContainerStack.Count <= 0)
        {
            CreateMotionTrailContainer();
        }
        Color color = motionTrailGradient.Evaluate(colorChangeTime);
        reqObject = motionTrailContainerStack.Pop();
        MotionTrailContainer mtContainer = reqObject.GetComponent<MotionTrailContainer>();
        mtContainer.ColorSet(color);


        reqObject.gameObject.SetActive(true);
    }

    public void SetMotionTrailContainer(GameObject obj)
    {
        obj.SetActive(false);
        motionTrailContainerStack.Push(obj);
    }

}

