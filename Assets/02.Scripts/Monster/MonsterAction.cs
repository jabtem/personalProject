using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System;

public class MonsterAction : MonoBehaviour
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
    [HideInInspector]
    public RoamingMode roamingMode;


    bool gameStart;
    public bool GameStart
    {
        get
        {
            return gameStart;
        }
        set
        {
            gameStart = value;
        }
    }
    Vector3 roamingAreaPosition;
    public int roamingPointsIndex;
    //오브젝트 시작전위치
    Vector3 firstPosition;


    Rigidbody rigid;

    MonsterFOV FoV;

    [SerializeField]
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
            myNavMesh.isStopped = true;
            myNavMesh.speed = speed;
            attackDamage.DamageValue = 0;
            switch (_state)
            {

                case MonsterState.Roaming:
                    if (myNavMesh.isStopped)
                        myNavMesh.isStopped = false;
                    FoV.SetFovActive(true);
                    myNavMesh.stoppingDistance = 0.0f;
                    anim.SetBool(moveId, true);
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
                case MonsterState.Trace:
                    if (myNavMesh.isStopped)
                        myNavMesh.isStopped = false;
                    FoV.SetFovActive(false);
                    myNavMesh.stoppingDistance = 0.0f;
                    StartCoroutine(TargetTrace());
                    break;
                case MonsterState.Battle:
                    StartCoroutine(BattleMode());
                    break;

            }


        }
    }
    NavMeshAgent myNavMesh;
    [SerializeField]
    bool viewGizmo;
    [SerializeField]
    float speed;

    [SerializeField]
    int attackRange;

    Animator anim;
    int moveId;
    int patternId;

    int IdleTrasionID;


    BoxCollider[] colliders;

    [SerializeField]
    int knockBackPower;

    [SerializeField]
    float konckBackTime;

    PlayerActionCtrl playerActionCtrl;

    MonsterHp monsterHp;

    Dictionary<float, WaitForSeconds> waitDic = new Dictionary<float, WaitForSeconds>();

    Damage attackDamage;
    void Awake()
    {
        FoV = GetComponent<MonsterFOV>();

        roamingAreaPosition = new Vector3(transform.position.x + moveRomaingAreaPosionX, 0f, transform.position.z + moveRomaingAreaPosionZ);
        colliders = gameObject.GetComponentsInChildren<BoxCollider>();
        TryGetComponent<Rigidbody>(out rigid);
        roamingPointsIndex = 0;
        firstPosition = transform.position;
        TryGetComponent<NavMeshAgent>(out myNavMesh);
        TryGetComponent<Animator>(out anim);
        moveId = Animator.StringToHash("Move");
        patternId = Animator.StringToHash("Pattern");
        IdleTrasionID = Animator.StringToHash("BackIdle");
        TryGetComponent<MonsterHp>(out monsterHp);

        attackDamage = gameObject.GetComponentInChildren<Damage>();
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
        GameStart = true;


    }
    private void Start()
    {
        State = MonsterState.Roaming;
    }



    #region 지점순회
    IEnumerator PointMove()
    {
        myNavMesh.SetDestination(transform.position);

        while(true)
        {

            if ((transform.position - myNavMesh.destination).sqrMagnitude <= 0.01f )
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
        WaitForSeconds wait;
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


            if (!waitDic.TryGetValue(sec, out wait))
            {
                waitDic.Add(sec, wait = new WaitForSeconds(sec));
            }

            yield return wait;

            //if (waitDic.ContainsKey(sec))
            //{
            //    wait = waitDic[sec];
            //    yield return wait;
            //}
            //else if (!waitDic.ContainsKey(sec))
            //{
            //    waitDic.Add(sec, new WaitForSeconds(sec));
            //    wait = waitDic[sec];
            //    yield return wait;
            //}
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
        //0~359까지 랜덤숫자
        int result = UnityEngine.Random.Range(0, 360);
        //중복제거목적
        if(result == angle)
        {
            result = RanAngleCheck(angle);
        }

        return result;
    }
    #endregion


    #region 타겟추적

    IEnumerator TargetTrace()
    {
        //추적 유지시간 측정용
        float traceTime = 0f;
        while(true)
        {
            traceTime += Time.deltaTime;
            Vector3 targetDirection = FoV.Target.transform.position - transform.position;
            //타겟이 공격사거리 내에 들어오면 전투모드로 전환
            if(targetDirection.sqrMagnitude <= FoV.TargetRadius + myNavMesh.radius +attackRange)
            {
                if(anim.GetBool(moveId))
                    anim.SetBool(moveId, false);
                myNavMesh.SetDestination(transform.position);
                State = MonsterState.Battle;
            }
            else
            {
                if (!anim.GetBool(moveId))
                    anim.SetBool(moveId, true);
                myNavMesh.SetDestination(FoV.Target.transform.position);
                
            }
            if(traceTime >=5f)
            {
                //추적시간이 5초이상 지나면 다시 정찰모드로 되돌아감
                State = MonsterState.Roaming;
            }

            yield return null;
        }

        
    }
    #endregion

    #region 전투패턴
    IEnumerator BattleMode()
    {
        //0 : 대기 1: 약공격 2: 강공격
        // 75%확률로 약공격
        // 25%확률로 강공격
        float ranPattern;
        float attackDelay = 0f;
        WaitForSeconds wait;
        while(true)
        {


            //공격할때 콜라이더활성

            Vector3 targetDirection = FoV.Target.transform.position - transform.position;
            float angle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, angle, 0);
            ranPattern = UnityEngine.Random.Range(1f, 100.0f);
            //공격이닿는거리면 공격
            if(targetDirection.sqrMagnitude <= FoV.TargetRadius + myNavMesh.radius + attackRange)
            {
                foreach (var col in colliders)
                {
                    col.enabled = true;
                }
                if (ranPattern > 25f)
                {
                    patternId = Animator.StringToHash("LightAttack");
                    attackDamage.DamageValue = 20;
                    anim.SetTrigger(patternId);
                    attackDelay = 2f;
                }
                else if (ranPattern <= 25f)
                {
                    patternId = Animator.StringToHash("HeavyAttack");
                    //헤비어택이 다단히트판정이기때문에 기본데미지는낮게
                    attackDamage.DamageValue = 5;
                    anim.SetTrigger(patternId);
                    attackDelay = 4f;
                }
            }
            //사거리를 벗어나면 다시추적
            else if(targetDirection.sqrMagnitude > FoV.TargetRadius + myNavMesh.radius + attackRange)
            {
                State = MonsterState.Trace;
            }

            if(!waitDic.TryGetValue(attackDelay,out wait))
            {
                waitDic.Add(attackDelay, wait = new WaitForSeconds(attackDelay));
            }


            yield return wait;


            //if (!waitDic.ContainsKey(attackDelay))
            //{
            //    waitDic.Add(attackDelay, wait = new WaitForSeconds(attackDelay));
            //    yield return wait;
            //}
            //else if (waitDic.ContainsKey(attackDelay))
            //{
            //    wait = waitDic[attackDelay];
            //    yield return wait;
            //}

        }

    }
    #endregion

    private void Update()
    {
        //정찰중에 시야각에 타겟이 닿았을경우 상태변경
        if (State == MonsterState.Roaming&&FoV.IsColision)
        {
            State = MonsterState.Trace;
        }

        //공격-> 대기로 돌아갈때 
        if(anim.GetAnimatorTransitionInfo(0).userNameHash == IdleTrasionID)
        {
            foreach(var col in colliders)
            {
                col.enabled = false;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag ("PlayerAttack"))
        {
            Damage dam;
            if (!other.gameObject.TryGetComponent<Damage>(out dam))
            {
                return;
            }

            //마지막타격에 맞았을때만 넉백
            if (playerActionCtrl.GetComboStep() >= 3)
            {
                StopAllCoroutines();
                StartCoroutine(KnockBack());
            }

            monsterHp.Damaged(dam.DamageValue);
            other.enabled = false;
        }
    }

    IEnumerator KnockBack()
    {

        float time = 0;
        patternId = Animator.StringToHash("Hit");
        //Vector3 start = transform.position;
        anim.SetTrigger(patternId);
        while(time < konckBackTime)
        {
            transform.Translate((FoV.TargetDirection*-1) * knockBackPower * Time.deltaTime,Space.World);
            //transform.position = Vector3.Lerp(start, start + (transform.forward * -1) * knockBackPower, time);
            time += Time.deltaTime;
            yield return null;
        }

        State = MonsterState.Trace;
        yield break;
    }
    void OnDrawGizmosSelected()
    {
        /******************정찰범위 디버그용**********************/
        if (roamingMode == RoamingMode.영역내무작위이동 && viewGizmo)
        {
            if (!GameStart)
                roamingAreaPosition = new Vector3(transform.position.x + moveRomaingAreaPosionX, 0f, transform.position.z + moveRomaingAreaPosionZ);
            else if (GameStart)
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
                if(!GameStart)
                {
                    Handles.Label(new Vector3(transform.position.x + roamingPoints[i].x, 1.5f, transform.position.z + roamingPoints[i].y), (i + 1).ToString());
                    Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                    Gizmos.DrawSphere(new Vector3(transform.position.x + roamingPoints[i].x, 0, transform.position.z + roamingPoints[i].y), 1f);
                }
                else if(GameStart)
                {
                    Handles.Label(new Vector3(firstPosition.x + roamingPoints[i].x, 1.5f, firstPosition.z + roamingPoints[i].y), (i + 1).ToString());
                    Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                    Gizmos.DrawSphere(new Vector3(firstPosition.x + roamingPoints[i].x, 0, firstPosition.z + roamingPoints[i].y), 1f);
                }

            }
        }
    }

    public void PlayerInfoSet(PlayerActionCtrl info)
    {
        playerActionCtrl = info;

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
