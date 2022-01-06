using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MonsterFOVTest : MonoBehaviour
{
    // Start is called before the first frame updat;


    [Range(0f,360f)]
    public float angle = 45f;
    [Range(0f,20f)]
    public float radius = 5f;
    Vector3 rightVector;
    Vector3 leftVector;
    public Transform target;
    public float playerColRadius;
    Vector3 targetDirection;
    Vector3 rVectorPoint;

    bool isCol = false;
    bool gameStart = false;



    private void Awake()
    {
        gameStart = true;
        //현재 플레이어가 캐릭터 컨트롤러를 사용하므로
        playerColRadius = target.GetComponent<CharacterController>().radius;
    }
    private void Update()
    {

        rightVector = new Vector3(Mathf.Sin(angle*0.5f * Mathf.Deg2Rad), 0, Mathf.Cos(angle*0.5f * Mathf.Deg2Rad));
        leftVector = new Vector3(-Mathf.Sin(angle*0.5f * Mathf.Deg2Rad), 0, Mathf.Cos(angle*0.5f * Mathf.Deg2Rad));


        //대상의 높이를 고려하지않음 자기자신의높이를기준
        targetDirection = new Vector3(target.position.x,transform.position.y,target.position.z) - transform.position;

        Vector3 leftVectToTraget = new Vector3(target.position.x, transform.position.y, target.position.z) - (transform.position + leftVector);


        // 백터의 내적 = 벡터A크기 * 벡터B크기 * Cos(theta)
        // Cos(theta) = 백터의내적 / 벡터A크기 / 벡터B크기
        // theta = ACos(벡터의 내적/ 벡터A크기/ 벡터 B크기)
        //플레이어 정면을 축으로 대상과의 각도가 angle의 절반보다 작거나 같다 => 시야범위내에 있다.
        //대상과의 거리가 부채꼴 반지름보다 작다 => 시야범위 내에 있다.
        //부채꼴의 선(벡터)과 점과의거리
        //부채꼴의 선을 벡터AB와 점을 P라할때 벡터AB와 벡터AP의 외적 = 선분AB의 길이 * 선분 AB와 점 P의 거리
        // 선분 AB와 점 P의거리 = 벡터 AB와 AP의 외적의크기/선분 AB의 길이
        // 내적 > 0 , theta < 90
        // 내적 < 0 , theta > 90
        // 내적 = 0 , theta = 90



        if (targetDirection.sqrMagnitude <= (radius+playerColRadius)*(radius+playerColRadius))
        {
            if(Mathf.Acos(Vector3.Dot(transform.forward, targetDirection.normalized)) * Mathf.Rad2Deg <= angle * 0.5f)
                isCol = true;
            if(Vector3.Cross(leftVector,targetDirection.normalized).magnitude/radius < radius)
            {
                isCol = true;
            }
            else
                isCol = false;
        }
        else
            isCol = false;


        

        Debug.DrawRay(transform.position, transform.forward * radius, Color.white);
        Debug.DrawRay(transform.position, transform.TransformDirection(rightVector.normalized) * radius, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(leftVector.normalized) * radius, Color.green);

    }

    private void OnDrawGizmos()
    {
        if(gameStart)
        {
            Handles.color = isCol ? new Color(1f, 0, 0, 0.2f) : new Color(0, 0, 1f, 0.2f);
            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angle*0.5f, radius);
            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angle*0.5f, radius);
            Handles.Label(transform.position+transform.forward *2f, angle.ToString());
        }

    }
}
