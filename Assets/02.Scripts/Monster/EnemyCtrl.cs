using System.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[System.Serializable]

public class Anim // 이렇게 외부 변수로 클래스를 정의하면 다른 스크립트에서도 아래 목록을 가진 이 클래스를 사용할 수 있음.
{
    public AnimationClip idle1;
    public AnimationClip idle2;
    public AnimationClip idle3;
    public AnimationClip idle4;
    public AnimationClip move;
    public AnimationClip surprise; // 몬스터가 나를 발견하면 놀라도록 표시.
    public AnimationClip attack1;
    public AnimationClip attack2;
    public AnimationClip attack3;
    public AnimationClip attack4;
    public AnimationClip hit1;
    public AnimationClip hit2;
    public AnimationClip eat;
    public AnimationClip sleep;
    public AnimationClip die;
}


[RequireComponent(typeof(AudioSource))] // 오디오 소스 컴포넌트 요구 ( 이 스크립트를 넣으면 오디오소스가 같이 컴포넌트 된다. )
[RequireComponent(typeof(NavMeshAgent))] // 오디오 소스 컴포넌트 요구 ( 이 스크립트를 넣으면 오디오소스가 같이 컴포넌트 된다. )
[RequireComponent(typeof(CapsuleCollider))] // 오디오 소스 컴포넌트 요구 ( 이 스크립트를 넣으면 오디오소스가 같이 컴포넌트 된다. )

public class TargetInfo
{
    Vector3 _target;
    public Vector3 target
    {
        get
        {
            return _target;
        }
        set
        {
            _target = value;
        }
    }
}
public class EnemyCtrl : MonoBehaviour
{

    private NavMeshAgent myTraceAgent; //NavMeshAgent 연결 레퍼런스

    public Anim anim; // 위에서 설정한 Anim 클래스.

    private Animation childAnim;  //하위 자식계층에 있는 모델의 Animation 컴포넌트에 접근하기 위한 레퍼런스

    AnimationState animState;  //애니메이션 상태 저장 

    //애니메이션 셀렉트(랜덤한 연출 )
    private float randAnimTime;
    private int randAnim;

    public AudioClip dieSound;


    //자신과 타겟 Transform 참조 변수
    public Transform myTr; // 나의 위치
    public Transform traceTarget; // 쫓아갈 타겟의 위치

    //추적을 위한 변수
    private bool traceObject;
    private bool traceAttack;

    // 로밍을위한 시간 변수
    public float hungryTime; // 헝그리 타임동안 로밍을 하고
    public float nonHungryTime; // 논헝그리 타임일때 잠을 잔다? 같은 로직을 완성할수 있음. ( 쫌더 보완이 필요 )

    //추적 대상 거리체크 변수 
    float dist1; // 나와 플레이어의 거리
    float dist2; // 나와 베이스의 거리

    //플레이어를 찾기 위한 배열 
    GameObject[] players; // 네트워크를 구현할 예정이므로 플레이어 배열
    private Transform playerTarget;

    //추적 대상인 베이스 캠프
    GameObject[] otherPlayers;
    private Transform otherTarget;

    //로밍 장소 
    private Transform[] roamingCheckPoints; // 몬스터가 순찰하기 위해 이동할 목표 지점 여러개
    //로밍 장소 중복해서 안가게...
    private int roamingRandcheckPos; // 한곳만 중복해서 로밍하지 않도록 막아준다.
    //로밍 타겟
    private Transform roamingTarget; // 결국 이동할 로밍 위치

    public bool isRoamingCheck = true;//로밍체크 가능여부

    [HideInInspector]
    public bool isDie; // 나중에 하이드 처리

    public enum MODE_STATE { IDLE = 1, MOVE, SURPRISE, TRACE, ATTACK, HIT, EAT, SLEEP, DIE };

    public enum MODE_KIND { ENEMY1 = 1, ENEMY2, ENEMYBOSS };


    [Header("STATE")]
    [Space(20)]
    public MODE_STATE enemyMode = MODE_STATE.IDLE; // 적의 애니메이션 상태 확인

    [Header("SETTING")]
    public MODE_KIND enemyKind = MODE_KIND.ENEMY1; // 적의 존재 확인

    [Header("몬스터 인공지능(능력치)")]
    [Space(5)]

    [Tooltip("몬스터의 HP")] // 아래 변수에 툴팁을 작성
    [Range(0, 1000)] public int hp = 100;
    [Tooltip("몬스터의 속도")]
    [Range(1f, 15000f)] public float speed = 6.0f;

    [Tooltip("몬스터 발견 거리")]
    [Range(1f, 15000f)] [SerializeField] float findDist = 10f;
    [Tooltip("몬스터 추적 거리")]
    [Range(1f, 25000f)] [SerializeField] float traceDist = 15f;
    [Tooltip("몬스터 공격 거리")]
    [Range(2f, 500f)] [SerializeField] float attackDist = 2.5f;
    [Tooltip("몬스터 로밍 시간")]
    [Range(0f, 3600f)] [SerializeField] float hungryTimeSet = 5f;
    [Tooltip("몬스터 로밍 대기시간")]
    [Range(0f, 3600f)] [SerializeField] float nonHungryTimeSet = 5f;

    [Header("Test")]
    [SerializeField] bool isHit;
    [SerializeField] bool hungry;

    [SerializeField] bool sleep;
    [Space(5)]
    [SerializeField] bool nonHungry;
    [SerializeField] float isHitTime;


    public GameObject enemyDestroyEffect;




    // 포톤 추가///////////////////////////////////////////////////////////////////////

    //참조할 컴포넌트를 할당할 레퍼런스 (미리 할당하는게 좋음)
    Rigidbody myRbody;

    //PhotonView 컴포넌트를 할당할 레퍼런스 
    //PhotonView pv = null;

    //위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정 
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    // 애니메이션 동기화를 위한 변수
    // RPC로 처리해도 됨...선택은 상황에 따라서...
    int net_Aim;

    public TargetInfo targetInfo = new TargetInfo();

    bool isTargetChange = false;

    PlayerMoveCtrl playerState;







    private void Awake()
    {
        myTraceAgent = GetComponent<NavMeshAgent>();
        childAnim = GetComponentInChildren<Animation>(); // 자식에 있는 애니메이션을 연결 s 아님
        myTr = GetComponent<Transform>(); // 나의 위치값을 레퍼런스              


        players = GameObject.FindGameObjectsWithTag("Player");
        //players = GameObject.FindGameObjectsWithTag("Player");


        // 로밍 위치 얻기
        roamingCheckPoints = GameObject.Find("RoamingPoint").GetComponentsInChildren<Transform>();

        net_Aim = 0;
        //myRbody = GetComponent<Rigidbody>();
        //pv = GetComponent<PhotonView>();

        //pv.ObservedComponents[0] = this;
        //pv.synchronization = ViewSynchronization.UnreliableOnChange;
        ////  * UnreliableOnChange             Unreliable(UDP 프로토콜) 옵션과 같지만 변경사항이 발생했을 경우에만 전송한다


        //if (!PhotonNetwork.isMasterClient)
        //{
        //    //원격 네트워크 유저의 아바타는 물리력을 안받게 처리하고
        //    //또한, 물리엔진으로 이동 처리하지 않고(Rigidbody로 이동 처리시...)
        //    //실시간 위치값을 전송받아 처리 한다 그러므로 Rigidbody 컴포넌트의
        //    //isKinematic 옵션을 체크해주자. 한마디로 물리엔진의 영향에서 벗어나게 하여
        //    //불필요한 물리연산을 하지 않게 해주자...

        //    //원격 네트워크 플레이어의 아바타는 물리력을 이용하지 않음 (마스터 클라이언트가 아닐때)
        //    //(원래 게임이 이렇다는거다...우리건 안해도 체크 돼있음...)
        //    myRbody.isKinematic = true;
        //    //네비게이션도 중지
        //    //myTraceAgent.isStopped = true; 이걸로 하면 off Mesh Link 에서 에러 발생 그냥 비활성 하자
        //    myTraceAgent.enabled = false;
        //}

        // 원격 플래이어의 위치 및 회전 값을 처리할 변수의 초기값 설정 
        // 잘 생각해보자 이런처리 안하면 순간이동 현상을 목격
        currPos = myTr.position;
        currRot = myTr.rotation;


        hungry = false;
    }



    // Start is called before the first frame update
    //IEnumerator Start()
    //{


               
    //    childAnim.clip = anim.idle1;  //Animation 컴포넌트의 clip속성에 idle1 애니메이션 클립 지정 
    //    childAnim.Play();  //지정한 애니메이션 클립(애니메이션) 실행 
        
    //    if (PhotonNetwork.isMasterClient)
    //    {
    //        traceTarget = players[0].transform; // //일단 첫 Base의 Transform만 연결

    //        //traceTarget = baseAll[0].transform;
    //        //추적하는 대상의 위치(Vector3)를 셋팅하면 바로 추적 시작 (가독성이 좋다)
    //        myTraceAgent.SetDestination(traceTarget.position);
    //        // 위와 같은 동작을 수행하지만...가독성이 별로다
    //        // myTraceAgent.destination = traceObj.position;

    //        // 정해진 시간 간격으로 Enemy의 Ai 변화 상태를 셋팅하는 코루틴
    //        StartCoroutine(ModeSet());

    //        // Enemy의 상태 변화에 따라 일정 행동을 수행하는 코루틴
    //        StartCoroutine(ModeAction());

    //        // 일정 간격으로 주변의 가장 가까운 Base와 플레이어를 찾는 코루틴 
    //        StartCoroutine(this.TargetSetting());

    //        // 로밍 루트 설정
    //        RoamingCheckStart();
    //    }

    //    else
    //    {
    //        StartCoroutine(this.NetAnim());
    //    }

    //    yield return null;
               

    //}

    void Update()
    {
        
        //myTraceAgent.SetDestination(traceTarget.position);

        //랜덤 애니메이션 선택 
        if (Time.time > randAnimTime)
        {
            randAnim = UnityEngine.Random.Range(0, 4); // 
            randAnimTime = Time.time + 5.0f;
        }

        //Enemy 배고픈 시간 (추적 시간) => 이 로직은 코루틴으로 처리하면 더 효율적
        if (!hungry) // 헝그리가 false일때 로밍을 시작.. 뭔가 이상하지만 일단 공부용
        {
            if (Time.time > hungryTime) // // 지정한 로밍 시간보다 타임 값이 커지면 ( 로망시간이 다 되면 )
            {

                // 이걸 안하면 몬스터 로밍박스에서 idle 할때 다음 목적지로 가지 못한다.

                RoamingCheckStart();



                // 변수 순서가 중요하다.
                hungry = true;  // 헝그리를 트루값으로 바꿔 로밍을 끝내고
                nonHungryTime = Time.time + nonHungryTimeSet + UnityEngine.Random.Range(0f, 0f); // 논헝그리타임에 여태 흐른 시간 + 논타이밍 시간 + 랜덤 추가 시간을 준다.
                nonHungry = true; // 논헝그리를 트루값으로 바꿔서 다음 로직을 타게 함.

            }
        }

        // 로밍 중지 로직 
        if (nonHungry) // 위에 코드로 논헝그리가 트루가 되면
        {
            if (Time.time > nonHungryTime) // 흐른 시간이 논헝그리 시간보다 커질때 ( 논헝그리 타임셋 + 랜덤 값보다 시간이 더 흐르면 )
            {
                // 변수 순서가 중요하다.
                nonHungry = false; // 논헝그리 종료
                hungryTime = Time.time + hungryTimeSet + UnityEngine.Random.Range(0f, 0f); // 헝그리 시간에 다시 값을 준다. ( 이러면 다시 타임값이 흐른 만큼 )
                hungry = false;
            }

        }


        //공격 받았을 경우 
        if (isHit)
        {
            if (Time.time > isHitTime)
            {
                isHit = false;
            }
        }


        //적을 봐라봄
        /*  if (enemyLook)
            {
                if (Time.time > enemyLookTime)
                {
                    //    enemyLookRotation = Quaternion.LookRotation(-(enemyTr.forward)); // - 해줘야 바라봄
                    enemyLookRotation = Quaternion.LookRotation(enemyTr.position - myTr.position); // - 해줘야 바라봄
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 10.0f);
                    enemyLookTime = Time.time + 0.01f;
                }
            }*/



        //포톤 추가
        // 마스터 클라이언트가 직접 Ai 및 애니메이션 로직 수행
        // pv.isMine 해도 됨
        //if (PhotonNetwork.isMasterClient)
        //{
        //    // 무언가 특별한 처리를 더 추가하고 싶다면 여기서 추가할 수 있다.
        //    //타겟의 변경이 있을때만 호출
        //    if(isTargetChange)
        //    {
        //        Debug.Log("targetchange");
        //        SetTarget(myTraceAgent.destination);
        //        pv.RPC("SetTarget", PhotonTargets.OthersBuffered, myTraceAgent.destination);
        //        isTargetChange = false;
        //    }

        //}
        //포톤 추가
        //원격 플레이어일 때 수행
        else
        {
            //원격 플레이어의 아바타를 수신받은 위치까지 부드럽게 이동시키자
            myTr.position = Vector3.Lerp(myTr.position, currPos, Time.deltaTime * 3.0f);
            //원격 플레이어의 아바타를 수신받은 각도만큼 부드럽게 회전시키자
            myTr.rotation = Quaternion.Slerp(myTr.rotation, currRot, Time.deltaTime * 3.0f);
        }

    }


    //IEnumerator ModeSet()
    //{

 
    //    while (!isDie)
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //        //자신과 Player의 거리 셋팅 

    //        float dist = Vector3.Distance(myTr.position, traceTarget.position); // 나와 타겟의 거리 ( 타겟은 플레이어일수도, 베이스일수도 있음 )

    //        if(traceTarget.gameObject.tag == "Player" || traceTarget.gameObject.tag == "TeamPlayer")
    //            playerState = traceTarget.GetComponent<PlayerMoveCtrl>();

    //        // 순서 중요 
    //        if (isHit)  //공격 받았을시 무조건 적으로 애니메이션 실행
    //        {
    //            enemyMode = MODE_STATE.HIT;
    //        }
    //        else if (dist <= attackDist) // Attack 사거리에 들어왔는지 ?? 공격받았을때 말고는 실행
    //        {
    //            //대상이 살아있으면 공격
    //            if (playerState.playerInfo.isAlive)
    //                enemyMode = MODE_STATE.ATTACK;
    //            //죽어있으면 로밍
    //            else
    //            {
    //                enemyMode = MODE_STATE.MOVE;
    //            }
    //        }
    //        else if (traceAttack)
    //        /* 몬스터를 추적중이라면... 트레이스 애니메이션이 2가지인 이유는 서프라이즈 애니메이션때 거리가 벌어져 추적이 끝나면 다시 또 놀라기 때문에 몇초동안 
    //        무조건 적으로 타겟을 쫓아가는 로직을 만든다. */
    //        {
    //            //대상이 살아있으면 추적
    //            if (playerState.playerInfo.isAlive)
    //                enemyMode = MODE_STATE.TRACE;
    //            //죽어있으면 로밍
    //            else
    //            {
    //                enemyMode = MODE_STATE.MOVE;
    //            }

    //        }
    //        else if (dist <= traceDist) // Trace 사거리에 들어왔는지 ??
    //        {
    //            //대상이 살아있으면 추적
    //            if (playerState.playerInfo.isAlive)
    //                enemyMode = MODE_STATE.TRACE;
    //            //죽어있으면 로밍
    //            else
    //            {
    //                enemyMode = MODE_STATE.MOVE;
    //            }
    //        }
    //        else if (dist <= findDist) // Find 사거리에 들어왔는지 ??
    //        {
    //            //대상이 살아있으면 놀람애니메이션
    //            if (playerState.playerInfo.isAlive)
    //                enemyMode = MODE_STATE.SURPRISE;
    //            //죽어있으면 로밍
    //            else
    //            {
    //                enemyMode = MODE_STATE.MOVE;
    //            }
    //        }
    //        else if (hungry) //  배고플 때 (주인공 찾는다)
    //        {
    //            enemyMode = MODE_STATE.MOVE; //몬스터의 상태를 이동으로 설정 
    //        }
    //        else if (sleep) // 잠잘 때 
    //        {
    //            enemyMode = MODE_STATE.SLEEP; //몬스터의 상태를 취침으로 설정 
    //        }
    //        else
    //        {
    //            enemyMode = MODE_STATE.IDLE; //몬스터의 상태를 idle 모드로 설정 
    //        }

           
    //    }
    //}

    IEnumerator ModeAction()
    {
        while (!isDie)
        {
            switch (enemyMode)
            {
                //Enemy가 Idle 상태 일때... 
                case MODE_STATE.IDLE:

                    //네비게이션 멈추고 (추적 중지)
                    //myTraceAgent.speed = 0f;
                    //myTraceAgent.angularSpeed = 0f;
                    //myTraceAgent.acceleration = 0f;

                    myTraceAgent.isStopped = true; // 활성화 된 동안에 속도 const 5?



                    if (randAnim == 0) // 이렇게 하나의 애니메이션 상태일때도 여러개의 애니메이션 적용이 가능.
                    {
                        //idle1 애니메이션 
                        childAnim.CrossFade(anim.idle1.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 0;

                    }
                    else if (randAnim == 1)
                    {
                        //idle2 애니메이션 
                        childAnim.CrossFade(anim.idle2.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 1;
                    }
                    else if (randAnim == 2)
                    {
                        //idle3 애니메이션 
                        childAnim.CrossFade(anim.idle3.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 2;
                    }
                    else if (randAnim == 3)
                    {
                        //idle3 애니메이션 
                        childAnim.CrossFade(anim.idle4.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 3;
                    }
                    break;

                //Enemy가 Trace 상태 일때... 
                case MODE_STATE.TRACE:

                    // 네비게이션 재시작(추적)
                    myTraceAgent.isStopped = false;
                    myTraceAgent.speed = 8f;
                    myTraceAgent.angularSpeed = 200f;
                    myTraceAgent.acceleration = 200f;

                    // 추적대상 설정(플레이어)
                    myTraceAgent.destination = traceTarget.position;

                    //네비속도 및 애니메이션 속도 제어
                    if (enemyKind == MODE_KIND.ENEMYBOSS) // 보스일땐 기존 적보다 이동속도가 뛰어남. 
                    {
                        // 네비게이션의 추적 속도를 현재보다 1.8배
                        myTraceAgent.speed = speed * 1.8f;

                        //애니메이션 속도 변경
                        childAnim[anim.move.name].speed = 1.8f;    /// _anim[]

                        //run 애니메이션 
                        childAnim.CrossFade(anim.move.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 4;

                    }
                    else
                    {
                        // 네비게이션의 추적 속도를 현재보다 1.5배
                        myTraceAgent.speed = speed * 1.5f;

                        //애니메이션 속도 변경
                        childAnim[anim.move.name].speed = 1.5f;

                        //run 애니메이션 
                        childAnim.CrossFade(anim.move.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 5;
                    }
                    break;

                //공격 상태
                case MODE_STATE.ATTACK:

                    //사운드 (공격)

                    //추적 중지(선택사항)
                    //네비게이션 멈추고 (추적 중지) 
                    myTraceAgent.isStopped = true;

                    //공격할때 적을 봐라 봐야함 
                    // myTr.LookAt(traceTarget.position); // 바로 봐라봄
                    Quaternion enemyLookRotation = Quaternion.LookRotation(traceTarget.position - myTr.position); // - 해줘야 바라봄  
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 10.0f);

                    if (randAnim == 0)
                    {
                        //attack1 애니메이션 
                        childAnim.CrossFade(anim.attack1.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 6;
                    }
                    else if (randAnim == 1)
                    {
                        //attack2 애니메이션 
                        childAnim.CrossFade(anim.attack2.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 7;
                    }
                    else if (randAnim == 2)
                    {
                        //attack3 애니메이션 
                        childAnim.CrossFade(anim.attack3.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 8;
                    }
                    else if (randAnim == 3)
                    {
                        //attack3 애니메이션 
                        childAnim.CrossFade(anim.attack4.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 9;
                    }
                    break;

                //이동 상태 
                case MODE_STATE.MOVE:

                    //Debug.Log("Move");
                    // 네비게이션 재시작(추적)
                    myTraceAgent.isStopped = false;
                    // 추적대상 설정(로밍장소)
                    myTraceAgent.destination = roamingTarget.position;

                    //네비속도 및 애니메이션 속도 제어
                    if (enemyKind == MODE_KIND.ENEMYBOSS)
                    {
                        // 네비게이션의 추적 속도를 현재보다 1.2배
                        myTraceAgent.speed = speed * 1.2f;

                        //애니메이션 속도 변경
                        childAnim[anim.move.name].speed = 1.2f;

                        //run 애니메이션 
                        childAnim.CrossFade(anim.move.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 10;

                    }
                    else
                    {
                        // 네비게이션의 추적 속도를 현재 속도로...
                        myTraceAgent.speed = speed;

                        //애니메이션 속도 변경
                        childAnim[anim.move.name].speed = 1.0f;

                        //walk 애니메이션 
                        childAnim.CrossFade(anim.move.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 11;

                    }
                    break;

                //놀람,적발견 상태 (여러가지 생각 해야함)
                case MODE_STATE.SURPRISE:

                    if (!traceObject)
                    {
                        traceObject = true;

                        //추적 중지 (선택사항)
                        //네비게이션 멈추고 (추적 중지) 
                        myTraceAgent.isStopped = true;

                        //roar 애니메이션 
                        childAnim.CrossFade(anim.surprise.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 12;

                        StartCoroutine(this.TraceObject());
                    }

                    break;

                //sleep 상태 
                case MODE_STATE.SLEEP:

                    //사운드 


                    //네비게이션 멈추고 (추적 중지) 
                    myTraceAgent.isStopped = true;

                    //sleep 애니메이션 
                    childAnim.CrossFade(anim.sleep.name, 0.3f);
                    // 포톤 추가
                    // 애니메이션 동기화.
                    net_Aim = 13;
                    break;

                //hit 상태  (여러가지 생각해야함 )
                //Enemy가 hit 상태 일때... 
                case MODE_STATE.HIT:

                    //네비게이션 멈추고 (추적 중지)
                    myTraceAgent.isStopped = true;

                    if (randAnim == 0 || randAnim == 1)
                    {
                        // hit1 애니메이션 
                        childAnim.CrossFade(anim.hit1.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 14;
                    }
                    else if (randAnim == 1 || randAnim == 2)
                    {
                        // hit2 애니메이션 
                        childAnim.CrossFade(anim.hit2.name, 0.3f);
                        // 포톤 추가
                        // 애니메이션 동기화.
                        net_Aim = 15;
                    }


                    break;

            }


            yield return null;
        }
    }

    IEnumerator TraceObject()
    {
        yield return new WaitForSeconds(2.5f); // 2.5초 대기F
        traceAttack = true; // 추적 상태로 바꿈.

        yield return new WaitForSeconds(5.5f); // 5.5초 대기 이때동안은 계속 추적 상태
        traceAttack = false; // 이제 추적을 false로 바꿈
        traceObject = false;
    }



    IEnumerator TargetSetting()
    {
        while (!isDie)
        {

            yield return new WaitForSeconds(0.2f);

            // 자신과 가장 가까운 플레이어 찾음
            players = GameObject.FindGameObjectsWithTag("Player");

            //플레이어가 있을경우 
            if (players.Length != 0)
            {
                playerTarget = players[0].transform;
                dist1 = (playerTarget.position - myTr.position).sqrMagnitude;
                foreach (GameObject _players in players)
                {
 
                    if ((_players.transform.position - myTr.position).sqrMagnitude < dist1)
                    {
                        playerTarget = _players.transform;
                        dist1 = (playerTarget.position - myTr.position).sqrMagnitude;
                    }
                }
            }



            // 자신과 가장 가까운 팀플레이어찾음
            if (GameObject.FindGameObjectsWithTag("TeamPlayer") != null)
                otherPlayers = GameObject.FindGameObjectsWithTag("TeamPlayer");

            if (otherPlayers.Length != 0)
            {

                otherTarget = otherPlayers[0].transform;
                dist2 = (otherTarget.position - myTr.position).sqrMagnitude;
                foreach (GameObject _otherTarget in otherPlayers)
                {
                    if ((_otherTarget.transform.position - myTr.position).sqrMagnitude < dist2 )
                    {
                        otherTarget = _otherTarget.transform;
                        dist2 = (otherTarget.position - myTr.position).sqrMagnitude;
                    }
                }


            }



            // 플레이어가 팀보다 우선순위가 높게 셋팅 (게임마다 틀리다 즉 자기 맘)
            if (dist1 <= dist2)
            {
                PlayerMoveCtrl p = playerTarget.GetComponent<PlayerMoveCtrl>();
                //if (p.playerInfo.isAlive)
                    traceTarget = playerTarget;

                isTargetChange = true;
            }
            else
            {
                PlayerMoveCtrl p = playerTarget.GetComponent<PlayerMoveCtrl>();

                if (otherTarget != null)
                {
                    PlayerMoveCtrl o = otherTarget.GetComponent<PlayerMoveCtrl>();
                    //if (o.playerInfo.isAlive)
                        traceTarget = otherTarget;

                }
                //else if (p.playerInfo.isAlive)
                else
                    traceTarget = playerTarget;
                isTargetChange = true;

            }


        }
    }


    public void RoamingCheckStart()
    {
        if(isRoamingCheck)
        {
            isRoamingCheck = false;
            StartCoroutine(this.RoamingCheck(roamingRandcheckPos));
        }

    }

    public void CanRoamingCheckStart()
    {
        if(!isRoamingCheck)
        {
            StartCoroutine(this.CanRomaingCheck());
        }
    }

    IEnumerator CanRomaingCheck()
    {
        yield return new WaitForSeconds(0.5f);
        isRoamingCheck = true;
    }


    IEnumerator RoamingCheck(int pos)
    {
        roamingRandcheckPos = UnityEngine.Random.Range(1, roamingCheckPoints.Length);

        //같은 자리 안가게....
        if (roamingRandcheckPos == pos)
        {
            //중복값을 막기위하여 재귀함수 호출
            RoamingCheckStart();

            yield break;

        }

        //로밍 타겟 셋팅
        roamingTarget = roamingCheckPoints[roamingRandcheckPos];


        //연속적으로 로밍타겟을변경하지않도록 설정
        // Debug.Log("Checking1");
    }




    public void EnemyDie()
    {
        // 포톤 추가
        //if (pv.isMine)
        //{
        //    StartCoroutine(this.Die());
        //}
    }



    IEnumerator Die()
    {
        

        isDie = true; // 죽었는지를 확인하는 변수를 true 처리
        //죽는 애니메이션 시작
        childAnim.CrossFade(anim.die.name, 0.3f);
        // 포톤 추가
        // 애니메이션 동기화.
        net_Aim = 16;

        //Enemy의 모드를 die로 설정
        enemyMode = MODE_STATE.DIE;
        //Enemy의 태그를 Untagged로 변경하여 더이상 플레이어랑 포탑이 찾지 못함
        this.gameObject.tag = "Untagged";
        this.gameObject.transform.Find("EnemyBody").tag = "Untagged"; // Find는 위치에 관련된 함수이므로 transform. 안에 있다. [ 이름 주의 ]
        //네비게이션 멈추고 (추적 중지) 
        myTraceAgent.isStopped = true;


        //Enemy에 추가된 모든 Collider를 비활성화(모든 충돌체는 Collider를 상속했음 따라서 다음과 같이 추출 가능)
        foreach (Collider coll in gameObject.GetComponentsInChildren<Collider>()) // 더이상 다른 대상과 충돌하지 못하게
        {
            coll.enabled = false;
        }

        // PhotonNetwork.Destroy(gameObject);

        //if (!StageManager.instance.day && !StageManager.instance.gameEnd)
        //    SoundManager.Instance.PlayEffect(dieSound, this.gameObject);
        //4.5 초후 오브젝트 삭제
        yield return new WaitForSeconds(0.1f);
        //Destroy(gameObject);

        GameObject enemyblood1 = Instantiate(enemyDestroyEffect, myTr.transform.position, Quaternion.identity) as GameObject;
        //밤에죽는것만 효과음재생 낮되서 자동으로 삭제될땐 재생X


        //PhotonNetwork.Destroy(gameObject);

        ///// 중요 내용 /////
        // 자신과 네트워크상의 아바타들까지 모두 소멸
        // PhotonNetwork.Destroy(gameObject);
    }



    void OnDestroy() // 디스트로이 될때 동작하는 함수
    {
        //Debug.Log("OnDestroy");


        



        StopAllCoroutines(); // 모든 코루틴을 정지.

    }




    public void HitEnemy()
    {
        int rand = UnityEngine.Random.Range(0, 100);
        if (rand < 30)
        {
            if (randAnim == 0 || randAnim == 1)
            {
                isHitTime = Time.time + anim.hit1.length + 0.2f;
                isHit = true;
            }
            else if (randAnim == 1 || randAnim == 2)
            {
                isHitTime = Time.time + anim.hit1.length + 0.2f;
                isHit = true;
            }
        }

        // animator.SetTrigger("IsHit");
        // SetTrigger 로 Is Hit을 


    }


    //IEnumerator NetAnim()
    //{
    //    while (!isDie)
    //    {
    //        yield return new WaitForSeconds(0.2f);

    //        if (!PhotonNetwork.isMasterClient)
    //        {
    //            if (net_Aim == 0)
    //            {
    //                childAnim.CrossFade(anim.idle1.name, 0.3f);
    //            }
    //            else if (net_Aim == 1)
    //            {
    //                childAnim.CrossFade(anim.idle2.name, 0.3f);
    //            }
    //            else if (net_Aim == 2)
    //            {
    //                childAnim.CrossFade(anim.idle3.name, 0.3f);
    //            }
    //            else if (net_Aim == 3)
    //            {
    //                childAnim.CrossFade(anim.idle4.name, 0.3f);
    //            }
    //            else if (net_Aim == 4)
    //            {
    //                //애니메이션 속도 변경
    //                childAnim[anim.move.name].speed = 1.8f;

    //                //run 애니메이션 
    //                childAnim.CrossFade(anim.move.name, 0.3f);
    //            }
    //            else if (net_Aim == 5)
    //            {
    //                //애니메이션 속도 변경
    //                childAnim[anim.move.name].speed = 1.5f;

    //                //run 애니메이션 
    //                childAnim.CrossFade(anim.move.name, 0.3f);
    //            }
    //            else if (net_Aim == 6)
    //            {
    //                //attack1 애니메이션 
    //                childAnim.CrossFade(anim.attack1.name, 0.3f);
    //            }
    //            else if (net_Aim == 7)
    //            {
    //                //attack2 애니메이션 
    //                childAnim.CrossFade(anim.attack2.name, 0.3f);
    //            }
    //            else if (net_Aim == 8)
    //            {
    //                //attack3 애니메이션 
    //                childAnim.CrossFade(anim.attack3.name, 0.3f);
    //            }
    //            else if (net_Aim == 9)
    //            {
    //                //attack4 애니메이션 
    //                childAnim.CrossFade(anim.attack4.name, 0.3f);
    //            }
    //            else if (net_Aim == 10)
    //            {
    //                //애니메이션 속도 변경
    //                childAnim[anim.move.name].speed = 1.2f;

    //                //run 애니메이션 
    //                childAnim.CrossFade(anim.move.name, 0.3f);
    //            }
    //            else if (net_Aim == 11)
    //            {
    //                //애니메이션 속도 변경
    //                childAnim[anim.move.name].speed = 1.0f;

    //                //walk 애니메이션 
    //                childAnim.CrossFade(anim.move.name, 0.3f);
    //            }
    //            else if (net_Aim == 12)
    //            {
    //                //roar 애니메이션 
    //                childAnim.CrossFade(anim.surprise.name, 0.3f);
    //            }
    //            else if (net_Aim == 13)
    //            {
    //                //sleep 애니메이션 
    //                childAnim.CrossFade(anim.sleep.name, 0.3f);
    //            }
    //            else if (net_Aim == 14)
    //            {
    //                // hit1 애니메이션 
    //                childAnim.CrossFade(anim.hit1.name, 0.3f);
    //            }
    //            else if (net_Aim == 15)
    //            {
    //                // hit2 애니메이션 
    //                childAnim.CrossFade(anim.hit2.name, 0.3f);
    //            }
    //            else if (net_Aim == 16)
    //            {
    //                //죽는 애니메이션 시작
    //                childAnim.CrossFade(anim.die.name, 0.3f);

    //                // 코루틴 함수를 빠져나감(종료)
    //                yield break;
    //            }

    //        }
    //    }

    //}

    //void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    //로컬 플레이어의 위치 정보를 송신
    //    if (stream.isWriting)
    //    {
    //        //박싱
    //        stream.SendNext(myTr.position);
    //        stream.SendNext(myTr.rotation);
    //        stream.SendNext(net_Aim);
    //    }
    //    //원격 플레이어의 위치 정보를 수신
    //    else
    //    {
    //        //언박싱
    //        currPos = (Vector3)stream.ReceiveNext();
    //        currRot = (Quaternion)stream.ReceiveNext();
    //        net_Aim = (int)stream.ReceiveNext();
    //    }

    //}

    //void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    //{
    //    if (PhotonNetwork.isMasterClient)
    //    {
    //        this.gameObject.GetComponent<PhotonView>().TransferOwnership(newMasterClient.ID);
    //        //일단 첫 Base의 Transform만 연결
    //        traceTarget = players[0].transform;
    //        isTargetChange = true;
    //        myTraceAgent.enabled = true;
    //        myRbody.isKinematic = false;
    //        //추적하는 대상의 위치(Vector3)를 셋팅하면 바로 추적 시작 (가독성이 좋다)
    //        myTraceAgent.SetDestination(traceTarget.position);
    //        // 위와 같은 동작을 수행하지만...가독성이 별로다
    //        // myTraceAgent.destination = traceObj.position;

    //        // 정해진 시간 간격으로 Enemy의 Ai 변화 상태를 셋팅하는 코루틴
    //        StartCoroutine(ModeSet());

    //        // Enemy의 상태 변화에 따라 일정 행동을 수행하는 코루틴
    //        StartCoroutine(ModeAction());

    //        // 일정 간격으로 주변의 가장 가까운 Base와 플레이어를 찾는 코루틴 
    //        StartCoroutine(this.TargetSetting());

    //        // 로밍 루트 설정
    //        RoamingCheckStart();

    //        //myRbody.isKinematic = false;
    //        //네비게이션도 실행

    //    }
    //}


    //void SetTarget(Vector3 pos)
    //{
    //    targetInfo.target = pos;
    //}
}
