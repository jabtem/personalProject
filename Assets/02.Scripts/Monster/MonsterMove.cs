using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System;

public class MonsterMove : MonoBehaviour
{

    public enum RoamingMode
    {
        지점순회, 영역내무작위이동
    };
    public enum MonsterState
    {
        Roaming, Trace, Battle, Die
    };

    /****************몬스터이동로직**********************/
    //기본적인 움직임은 네비게이션메쉬 사용(장애물인식을 원할하게하기위함)
    //인스펙터에서 로밍(정찰) 영역을 지정하게하려면 어떻게해야하는가
    //최종적으로는 지정한 영역외를 벗어나진못하되 
    //지정 영역내에서는 무작위로 행동하도록 구현목적
    [HideInInspector]
    public Vector3[] roamingArea;

    public Vector2[] roamingPoints = new Vector2[0];

    [HideInInspector]
    public float roamingAreaWidth;
    [HideInInspector]
    public float roamingAreaHeight;
    [HideInInspector]
    public float moveRomaingAreaPosionX;
    [HideInInspector]
    public float moveRomaingAreaPosionZ;

    bool gameStart = false;
    Vector3 roamingAreaPosition;
    int roamingPointsIndex;
    //오브젝트 시작전위치
    Vector3 firstPosition;

    public RoamingMode roamingMode;
    MonsterState _state;

    //상태가 변화되면 이전상태에서 호출한 코루틴 정지후
    //해당 상태에서 필요한 동작 실행
    public MonsterState State
    {
        get
        {
            return _state;
        }
        set
        {
            StopAllCoroutines();
            _state = value;

            switch (_state)
            {
                case MonsterState.Roaming:
                    if (myNavMesh.isStopped)
                        myNavMesh.isStopped = false;
                    myNavMesh.speed = 5f;
                    myNavMesh.stoppingDistance = 0.0f;
                    switch (roamingMode)
                    {
                        case RoamingMode.영역내무작위이동:
                            StartCoroutine(RandomMove());
                            break;
                        case RoamingMode.지점순회:
                            StartCoroutine(PointMove());
                            break;
                    }

                    break;
            }


        }
    }
    NavMeshAgent myNavMesh;
    [SerializeField]
    bool viewGizmo;
    [SerializeField]
    float speed;

    Transform target;

    
    void Awake()
    {
        roamingAreaPosition = new Vector3(transform.position.x + moveRomaingAreaPosionX, 0f, transform.position.z + moveRomaingAreaPosionZ);
        roamingPointsIndex = 0;
        firstPosition = transform.position;
        myNavMesh = GetComponent<NavMeshAgent>();
        State = MonsterState.Roaming;
        roamingArea = new Vector3[]
        {
            //왼쪽아래
            new Vector3(roamingAreaPosition.x - roamingAreaWidth, 0, roamingAreaPosition.z - roamingAreaHeight),
            //왼쪽 위
            new Vector3(roamingAreaPosition.x - roamingAreaWidth, 0, roamingAreaPosition.z + roamingAreaHeight),
            //오른쪽 위
            new Vector3(roamingAreaPosition.x + roamingAreaWidth, 0, roamingAreaPosition.z + roamingAreaHeight),
            //오른쪽 아래
            new Vector3(roamingAreaPosition.x + roamingAreaWidth, 0, roamingAreaPosition.z - roamingAreaHeight)
        };
        gameStart = true;


    }


    #region 지점순회
    IEnumerator PointMove()
    {
        while(true)
        {
            if (transform.position == myNavMesh.destination)
            {
                roamingPointsIndex %= roamingPoints.Length;
                Vector3 roamingTarget = new Vector3(firstPosition.x + roamingPoints[roamingPointsIndex].x, 0f, firstPosition.z + roamingPoints[roamingPointsIndex].y);
                myNavMesh.SetDestination(roamingTarget);
                ++roamingPointsIndex;
            }
            yield return null;
        }
    }
    #endregion

    #region 영역 무작위 이동
    IEnumerator RandomMove()
    {
        //무작위 숫자 범위 밖의값
        int ranAngle = -1;
        while (true)
        {
            ranAngle = RanAngleCheck(ranAngle);
            float sec = UnityEngine.Random.Range(1.0f, 2.1f);

            float radian = ranAngle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(radian), 0f, Mathf.Sin(radian))*10f;

            myNavMesh.isStopped = true;
            //정찰영역 벗어낫는지 판단
            if (!RoamingAreaOutCheck())
            {
                myNavMesh.SetDestination(transform.position + direction);

            }
            else if (RoamingAreaOutCheck())
            {
                myNavMesh.SetDestination(roamingAreaPosition);
            }

            myNavMesh.isStopped = false;

            yield return new WaitForSeconds(sec);
        }

    }
    //정찰영역 밖으로 벗어났는지 체크
    bool RoamingAreaOutCheck()
    {
        //왼쪽아래지점
        if (transform.position.x < roamingArea[0].x || transform.position.z < roamingArea[0].z)
            return true;
        //왼쪽위지점
        else if (transform.position.x < roamingArea[1].x || transform.position.z > roamingArea[1].z)
            return true;
        //오른쪽위
        else if (transform.position.x > roamingArea[2].x || transform.position.z > roamingArea[2].z)
            return true;
        //오른쪽 아래
        else if (transform.position.x > roamingArea[3].x || transform.position.z < roamingArea[3].z)
            return true;
        else
            return false;
    }

    int RanAngleCheck(int angle)
    {
        int result = UnityEngine.Random.Range(0, 360);
        //중복제거목적
        if(result == angle)
        {
            Debug.Log("test");
            RanAngleCheck(angle);
        }

        return result;
    }
    #endregion
    private void OnDrawGizmos()
    {
        /******************정찰범위 디버그용**********************/
        if (roamingMode == RoamingMode.영역내무작위이동 && viewGizmo)
        {
            if (!gameStart)
                roamingAreaPosition = new Vector3(transform.position.x + moveRomaingAreaPosionX, 0f, transform.position.z + moveRomaingAreaPosionZ);
            else if (gameStart)
                roamingAreaPosition = new Vector3(firstPosition.x + moveRomaingAreaPosionX, 0f, firstPosition.z + moveRomaingAreaPosionZ);

            roamingArea = new Vector3[]
            {
            //왼쪽아래
            new Vector3(roamingAreaPosition.x - roamingAreaWidth, 0, roamingAreaPosition.z - roamingAreaHeight),
            //왼쪽 위
            new Vector3(roamingAreaPosition.x - roamingAreaWidth, 0, roamingAreaPosition.z + roamingAreaHeight),
            //오른쪽 위
            new Vector3(roamingAreaPosition.x + roamingAreaWidth, 0, roamingAreaPosition.z + roamingAreaHeight),
            //오른쪽 아래
            new Vector3(roamingAreaPosition.x + roamingAreaWidth, 0, roamingAreaPosition.z - roamingAreaHeight)
            };

            Handles.DrawSolidRectangleWithOutline(roamingArea, new Color(1f, 1f, 1f, 0.2f), Color.white);
        }

        else if (roamingMode == RoamingMode.지점순회 && roamingPoints.Length != 0 && viewGizmo)
        {
            for (int i=0; i< roamingPoints.Length; ++i)
            {
                if(!gameStart)
                {
                    Handles.Label(new Vector3(transform.position.x + roamingPoints[i].x, 1.5f, transform.position.z + roamingPoints[i].y), (i + 1).ToString());
                    Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                    Gizmos.DrawSphere(new Vector3(transform.position.x + roamingPoints[i].x, 0, transform.position.z + roamingPoints[i].y), 1f);
                }
                else if(gameStart)
                {
                    Handles.Label(new Vector3(firstPosition.x + roamingPoints[i].x, 1.5f, firstPosition.z + roamingPoints[i].y), (i + 1).ToString());
                    Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                    Gizmos.DrawSphere(new Vector3(firstPosition.x + roamingPoints[i].x, 0, firstPosition.z + roamingPoints[i].y), 1f);
                }

            }
        }
    }
    //미사용
    //private void Update()
    //{
    //    switch (State)
    //    {
    //        case MonsterState.Roaming:
    //            if (myNavMesh.isStopped)
    //                myNavMesh.isStopped = false;
    //            myNavMesh.speed = 5f;
    //            myNavMesh.stoppingDistance = 0.0f;
    //            switch (roamingMode)
    //            {
    //                case RoamingMode.지점순회:
    //                    if (target == null && roamingPoints.Length != 0)
    //                    {
    //                        //목표지점에 도달하면 목적지가 다음지점으로 변경
    //                        if (transform.position == myNavMesh.destination)
    //                        {
    //                            roamingPointsIndex %= roamingPoints.Length;
    //                            Vector3 roamingTarget = new Vector3(firstPosition.x + roamingPoints[roamingPointsIndex].x, 0f, firstPosition.z + roamingPoints[roamingPointsIndex].y);
    //                            myNavMesh.destination = roamingTarget;
    //                            ++roamingPointsIndex;
    //                        }
    //                    }

    //                    break;
    //                case RoamingMode.영역내무작위이동:
    //                    if (stateChange)
    //                    {
    //                        StartCoroutine(RandomMove());
    //                        stateChange = false;
    //                    }


    //                    break;
    //            }


    //            break;
    //    }
    //}
}
