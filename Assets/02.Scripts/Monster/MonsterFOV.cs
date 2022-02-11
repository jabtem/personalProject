using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class MonsterFOV : MonoBehaviour
{
    // Start is called before the first frame updat;


    [Range(0f,360f)]
    public float angle = 45f;
    [Range(0f,20f)]
    public float fovRadius = 5f;
    Vector3 rightVector;
    Vector3 leftVector;


    Vector3 targetDirection;

    public Vector3 TargetDirection
    {
        get
        {
            return targetDirection;
        }
        set
        {
            targetDirection = value;
        }
    }

    //충돌여부판단
    public bool IsColision
    {
        get
        {
            return isCol;
        }
        set
        {
            isCol = value;
        }
    }
    bool isCol;
    //플레이어 컨트롤러의 반지름
    float targetRaius;
    public float TargetRadius
    {
        get
        {
            return targetRaius;
        }
        set
        {
            targetRaius = value;
        }
    }
    Transform target;
    public Transform Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }

    [SerializeField]
    Image fovImage;
    [SerializeField]
    bool viewGizmo = false;

    MonsterAction monsterAction;



    private void Awake()
    {

        monsterAction = GetComponent<MonsterAction>();
        fovImage.transform.localScale = new Vector3(fovRadius, fovRadius, 1);
    }
    private void Start()
    {
        Target = GameObject.FindGameObjectWithTag("Player").transform;
        //현재 플레이어가 캐릭터 컨트롤러를 사용하므로
        TargetRadius = Target.GetComponent<CharacterController>().radius;
        monsterAction.PlayerInfoSet(Target.GetComponent<PlayerActionCtrl>());
    }
    private void Update()
    {
        //시야각이 켜져있을때만 계산
        if(fovImage.gameObject.activeSelf)
        {
            //시야각 오른쪽 경계선 벡터
            rightVector = new Vector3(Mathf.Sin((transform.eulerAngles.y + angle * 0.5f) * Mathf.Deg2Rad),
                0, Mathf.Cos((transform.eulerAngles.y + angle * 0.5f) * Mathf.Deg2Rad)) * fovRadius;
            //시야각 왼쪽 경계선 벡터
            leftVector = new Vector3(Mathf.Sin((transform.eulerAngles.y - angle * 0.5f) * Mathf.Deg2Rad),
                0, Mathf.Cos((transform.eulerAngles.y - angle * 0.5f) * Mathf.Deg2Rad)) * fovRadius;
            //오브젝트에서 타겟방향 벡터
            targetDirection = new Vector3(Target.position.x, transform.position.y, Target.position.z) - transform.position;

            /***************************** 부채꼴 원충돌 ********************************/
            // 타겟과의 거리가 (부채꼴반지름 + 플레이어의반지름) 보다 작거나같으면
            // 3가지 경우를 체크
            // 1.오브젝트 정면벡터와 타겟 방향 벡터 사이의 각이 지정한 Angle/2 보다 작거나 같은경우(정면기준 좌우로나뉘기때문에 Angle의 절반씩 확인)
            // -> 시야각 호부분과 시야각 내부와 플레이어가 접촉
            // 2.부채꼴 왼쪽선과 타겟사이의 거리가 플레이어 콜라이더 반지름보다 작거나 같은경우
            // -> 시야각 왼쪽 경계선에 플레이어가 접촉
            // 3.부채꼴 오른쪽선과 타겟사이의 거리가 플레이어 콜라이더 반지름보다 작거나 같은경우
            // -> 시야각 오른쪽 경계선에 플레이어가 접촉
            /****************************************************************************/
            if (targetDirection.sqrMagnitude <= (fovRadius + TargetRadius) * (fovRadius + TargetRadius))
            {
                //부체꼴 호부분과 원 충돌 체크
                if (Mathf.Acos(Vector3.Dot(transform.forward, targetDirection.normalized)) * Mathf.Rad2Deg <= angle * 0.5f)
                {
                    IsColision = true;
                }
                //부채꼴 왼쪽선과 원 충돌체크
                //대상이 오브젝트 정면기준으로 왼쪽에 있을때만체크
                else if (Vector3.Cross(leftVector, targetDirection).magnitude / fovRadius <= TargetRadius &&
                    Vector3.Cross(targetDirection, transform.forward).y >= 0 &&
                    Vector3.Dot(targetDirection.normalized, leftVector.normalized) >= 0)
                {

                    IsColision = true;
                }
                //부채꼴 오른쪽선과 원 충돌체크
                //대상이 오브젝트 정면기준으로 오른쪽에 있을때만체크
                else if (Vector3.Cross(rightVector, targetDirection).magnitude / fovRadius <= TargetRadius &&
                    Vector3.Cross(targetDirection, transform.forward).y < 0 &&
                    Vector3.Dot(targetDirection.normalized, rightVector.normalized) >= 0)
                {
                    IsColision = true;
                }
                else
                {
                    IsColision = false;
                }

            }
            else
                IsColision = false;


            //if(IsColision)
            //{
            //    if (fovImage.gameObject.activeSelf)
            //        fovImage.gameObject.SetActive(false);
            //    else
            //        return;
            //}
            //else if(!IsColision)
            //{
            //    if (!fovImage.gameObject.activeSelf)
            //    {
            //        fovImage.gameObject.SetActive(true);
            //    }
            //    else
            //        return;
            //}




            Debug.DrawRay(transform.position, targetDirection, Color.yellow);
            Debug.DrawRay(transform.position, targetDirection*-1, Color.red);
            Debug.DrawRay(transform.position, transform.forward * fovRadius, Color.white);

            Debug.DrawRay(transform.position, rightVector, Color.red);
            Debug.DrawRay(transform.position, leftVector, Color.green);

        }

    }

    private void OnDrawGizmosSelected()
    {
        if (viewGizmo)
        {
            Handles.color = isCol ? new Color(1f, 0, 0, 0.2f) : new Color(0, 0, 1f, 0.2f);
            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angle * 0.5f, fovRadius);
            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angle * 0.5f, fovRadius);
            Handles.Label(transform.position + transform.forward * 2f, angle.ToString());

        }

    }

    public void SetFovActive(bool value)
    {
        fovImage.gameObject.SetActive(value);
    }


    /************************사용한 수식정리*****************************/
    // 백터의 내적 = 벡터A크기 * 벡터B크기 * Cos(theta) = A.x*B.x + A.y*B.y + A.z*B.z
    // 벡터의 외적 = 벡터A * 벡터b * Sin(theta) = (A.y*B.z - A.z*B.y, A.z*B.x - A.x*B.z, A.x*B.y - A.y*B.x)
    // Cos(theta) = 백터의내적 / 벡터A크기 / 벡터B크기
    // theta = ACos(벡터의 내적/ 벡터A크기/ 벡터 B크기)
    // 플레이어 정면을 축으로 대상과의 각도가 angle의 절반보다 작거나 같다 => 시야범위내에 있다.
    // 대상과의 거리가 부채꼴 반지름보다 작다 => 시야범위 내에 있다.
    // 부채꼴의 선(벡터)과 점과의거리
    // 직선을 벡터AB와 점을 P라할때 벡터AB와 벡터AP의 외적 = 선분AB의 길이 * 선분 AB와 점 P의 거리
    // 선분 AB와 점 P의거리 = 벡터 AB와 AP의 외적의크기/선분 AB의 길이
    // 내적 > 0 , theta < 90
    // 내적 < 0 , theta > 90
    // 내적 = 0 , theta = 90
    // 타겟벡터와 중심벡터의 외적의 y값이 0보다 큼 = 타겟이 중심기준 왼쪽에 위치
    // 타겟벡터와 중심벡터의 외적의 y값이 0보다 작음 = 타겟이 중심기준 오른쪽에 위치
}
