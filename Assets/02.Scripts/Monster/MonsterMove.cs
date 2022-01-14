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
        Roaming, Trace, Attack, Die
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

    public RoamingMode roamingMode;
    MonsterState state;
    NavMeshAgent myNavMesh;
    Transform target;
    
    void Awake()
    {
        roamingAreaPosition = new Vector3(transform.position.x + moveRomaingAreaPosionX, 0f, transform.position.z + moveRomaingAreaPosionZ);
        myNavMesh = GetComponent<NavMeshAgent>();
        MonsterState state = MonsterState.Roaming;
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
    private void Update()
    {
        switch(state)
        {
            case MonsterState.Roaming:
                switch(roamingMode)
                {
                    case RoamingMode.지점순회:
                        if(target !=null && roamingPoints.Length !=0)
                        {
                            myNavMesh.destination = roamingPoints[0];
                        }

                        break;
                    case RoamingMode.영역내무작위이동:
                        break;
                }


                break;
        }
    }


    private void OnDrawGizmos()
    {
        /******************정찰범위 디버그용**********************/
        if (roamingMode == RoamingMode.영역내무작위이동 && !gameStart)
        {
            roamingAreaPosition = new Vector3(transform.position.x + moveRomaingAreaPosionX, 0f, transform.position.z + moveRomaingAreaPosionZ);

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

        else if (roamingMode == RoamingMode.지점순회 && roamingPoints.Length != 0 && !gameStart)
        {
            for (int i=0; i< roamingPoints.Length; ++i)
            {
                Handles.Label(new Vector3(transform.position.x + roamingPoints[i].x, 1.5f, transform.position.z + roamingPoints[i].y), (i+1).ToString());
                Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                Gizmos.DrawSphere(new Vector3(transform.position.x + roamingPoints[i].x, 0, transform.position.z + roamingPoints[i].y), 1f);


            }
        }
    }

}
