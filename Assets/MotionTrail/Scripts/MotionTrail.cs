using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrail : MonoBehaviour {
    
    [Header("타겟 스킨메쉬")]
    public GameObject TargetSkinMesh;

    [Header("이펙트 출력할 속도간격. 낮을 수록 부하가 심해집니다.")]
    [Range(0, 1)]
    public float ExportSpeedDelay = 0.1f;

    [Header("이펙트 출력시간. 체크할 경우 EffectLifeTime(초)동안 이펙트를 출력합니다. 체크 해제시 영구적으로 출력합니다.")]
    public bool UseLifeTime = false; 
    public float EffectLifeTime = 3;

    [Header("------------------------------------------------------------------------------------------------------------------------------------------------------")]
    [Header("쉐이더 변수 이름. 0~1까지 올라갑니다.")]
    public string ValueName;

    [Header("0 -> 1 속도 딜레이. 낮을 수록 빨라집니다. 0값이 되지 않도록 해주세요.")]
    [Range(0, 1)]
    public float ValueTimeDelay = 0.1f;

    [Header("변수 더할 값. 0.1이라면 1이 될때까지 10번 반복됨. 값이 높을 수록 가볍습니다.")]
    [Range(0, 1)]
    public float ValueDetail = 0.1f;

    private bool NeedObject;
    private void OnEnable()
    {
        if (TargetSkinMesh == null)
        {
            #if UNITY_EDITOR
            Debug.Log("<color=red>" + "타겟 스킨메쉬가 없습니다." + "</color>", this);
            #endif
        }
        if (ValueName == "")
        {
            #if UNITY_EDITOR
            Debug.Log("<color=red>" + "변경할 쉐이더 변수이름이 존재하지 않습니다." + "</color>", this);
            #endif
        }
        
        if(TargetSkinMesh != null && ValueName != "")
        {
            StopAllCoroutines();
            StartCoroutine("GhostStart");

            if(UseLifeTime == true)
            {
                StartCoroutine("TimerStart");
            }
        }
    }
    IEnumerator GhostStart()
    {
        while (true) //break할때 까지 계속 돔.
        {
            NeedObject = false; //
            for (int i = 1; i < transform.childCount + 1; i++)
            {
                #region
                //transform.GetChild(i).gameObject.transform.position = TargetSkinMesh.transform.position;
                //transform.GetChild(i).gameObject.transform.rotation = TargetSkinMesh.transform.rotation;
                //transform.GetChild(i).gameObject.GetComponent<MotionTrailRenderer>().SkinMesh = TargetSkinMesh.GetComponent<SkinnedMeshRenderer>();

                //transform.GetChild(i).gameObject.GetComponent<MotionTrailRenderer>().ValueName = ValueName;
                //transform.GetChild(i).gameObject.GetComponent<MotionTrailRenderer>().ValueTimeDelay = ValueTimeDelay;
                //transform.GetChild(i).gameObject.GetComponent<MotionTrailRenderer>().ValueDetail = ValueDetail;
                //transform.GetChild(i).gameObject.SetActive(true);
                #endregion
                NeedObject = CreateTrailMotion(i); //새로운 모션 생성
                if (NeedObject == true) //모션을 생성하고 성공 했는지의 여부를 받아옵니다. true일 경우 생성이 된 것입니다.
                {
                    //Debug.Log("모션 생성 성공");
                    break;
                }
            }
            //if(NeedObject == false)
            //{
            //    Instantiate(transform.GetChild(0), this.transform);
            //    #if UNITY_EDITOR
            //    Debug.Log("<color=red>" + "Ghost 오브젝트가 부족합니다." + "</color>" + "\n 해결방법1 : Export Speed Delay를 올려주세요. \n 해결방법2 : Value Time Delay를 내려주세요. \n 해결방법3 : Value Detail을 올려주세요. \n 해결방법4 : Ghost를 더 복제 해주세요.");
            //    #endif
            //}
            yield return new WaitForSeconds(ExportSpeedDelay);
        }
    }
    IEnumerator TimerStart() //타이머
    {
        yield return new WaitForSeconds(EffectLifeTime);
        StopAllCoroutines();
        yield return null;
    }

    public bool CreateTrailMotion(int ArrayNum)
    {
       if(ArrayNum < transform.childCount)
       {
            //Debug.Log("갯수가 충분합니다.");
            if (transform.GetChild(ArrayNum).gameObject.activeSelf == false) //오브젝트가 비활성화 되어 있음 (사용가능)
            {
                transform.GetChild(ArrayNum).gameObject.transform.position = TargetSkinMesh.transform.position;
                transform.GetChild(ArrayNum).gameObject.transform.rotation = TargetSkinMesh.transform.rotation;
                transform.GetChild(ArrayNum).gameObject.GetComponent<MotionTrailRenderer>().SkinMesh = TargetSkinMesh.GetComponent<SkinnedMeshRenderer>();

                transform.GetChild(ArrayNum).gameObject.GetComponent<MotionTrailRenderer>().ValueName = ValueName;
                transform.GetChild(ArrayNum).gameObject.GetComponent<MotionTrailRenderer>().ValueTimeDelay = ValueTimeDelay;
                transform.GetChild(ArrayNum).gameObject.GetComponent<MotionTrailRenderer>().ValueDetail = ValueDetail;
                transform.GetChild(ArrayNum).gameObject.SetActive(true);
                return true; //모션 생성에 성공했습니다.
            }
            else //활성화 되어 있는 경우 false를 반환합니다.
            {
                //Debug.Log("<color=red>" + "해당 오브젝트가 활성화 되어 있습니다." + "</color>");
                if(transform.childCount == ArrayNum + 1)
                {
                    //Debug.Log("<color=red>" + "모션생성(갯수부족1)" + "</color>");
                    Instantiate(transform.GetChild(0), this.transform); //새로운 모션을 생성합니다.
                }
                //Instantiate(transform.GetChild(0), this.transform); //새로운 모션을 생성합니다.
                return false;
            }
       }
       else //갯수 부족
       {
           //Debug.Log("<color=red>" + "모션생성(갯수부족2)" + "</color>");
           Instantiate(transform.GetChild(0), this.transform); //새로운 모션을 생성합니다.
           return false;
       }
    }
}
