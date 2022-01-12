using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MonsterMove : MonoBehaviour
{
    /****************몬스터이동로직**********************/
    //기본적인 움직임은 네비게이션메쉬 사용(장애물인식을 원할하게하기위함)
    //인스펙터에서 로밍(정찰) 영역을 지정하게하려면 어떻게해야하는가
    //최종적으로는 지정한 영역외를 벗어나진못하되 
    //지정 영역내에서는 무작위로 행동하도록 구현목적
    Vector3[] roamingArea;
    [Range(0.0f,100.0f)]
    public float roamingAreaWidth;
    [Range(0.0f, 100.0f)]
    public float roamingAreaHeight;
    [Range(-100.0f, 100.0f)]
    public float moveRomaingAreaPosionX;
    [Range(-100.0f, 100.0f)]
    public float moveRomaingAreaPosionZ;

    bool gameStart = false;
    //몬스터의 최초위치
    Vector3 roamingAreaPosition;
    Vector3 firstPosition;
    void Awake()
    {
        //최초위치
        firstPosition = transform.position;
        gameStart = true;


    }

    private void Update()
    {
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
    }
    private void OnDrawGizmos()
    {
        if(gameStart)
            Handles.DrawSolidRectangleWithOutline(roamingArea, new Color(1f,1f,1f,0.2f),Color.white);
    }

}
