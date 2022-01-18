using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//미니맵 각도계산에 참조하기위한 플레이어의 x,z
public class PlayerInfo
{

    Vector3 _playerVector;


    public Vector3 playerVector
    {
        get
        {
            return _playerVector;
        }
        set
        {
            _playerVector = value;
        }
    }

}
// 캐릭터 컨트롤러 조작방식
public class PlayerMoveCtrl : MonoBehaviour
{
    GameObject actionBtn;
    Text btnText;

    // CharacterController 컴포넌트를 위한 레퍼런스
    CharacterController controller;
    Transform myTr;
    Ray ray;
    RaycastHit hitInfo;
    // 중력 
    public float gravity = 20.0f;

    // 케릭터 이동속도
    public float moveSpeed = 5.0f;
    // 케릭터 회전속도
    public float rotSpeed = 120.0f;
    //케릭터 점프 속도
    public float jumpSpeed = 10.0f;

    //애니메이션
    Animator anim;

    //애니메이션 파라메터 id해쉬값
    int id;

    // 케릭터 이동 방향
    public Vector3 moveDirection;
    //조이스틱을이용해 마지막으로 이동했던 방향
    public Vector3 lastMoveDirection;

    //정지시 이전 캐릭터방향 미니맵마커 유지용
    Vector3 preMoveDirection;

    //이동가능여부 판단
    //공격중 조이스틱조작이나 구르기중 조이스틱 조작차단용
    public bool canMove;

    public PlayerInfo playerInfo = new PlayerInfo();
    void Awake()
    {
        // 레퍼런스 연결
        myTr = GetComponent<Transform>();
        controller = GetComponent<CharacterController>();
        Camera.main.GetComponent<smoothFollowCam>().target = this.transform;
        anim = GetComponent<Animator>();
        id = Animator.StringToHash("speed");
        canMove = true;
    }

    void Update()
    {
        //데카르트좌표계를 유니티와 일치시킨다 
        float ang = Mathf.Atan2(playerInfo.playerVector.z, playerInfo.playerVector.x) * Mathf.Rad2Deg *-1;
        Quaternion rot = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0f);//카메라의앵글에따라 변환
        if (controller.isGrounded)
        {

            float v = Input.GetAxis("Vertical") + UltimateJoystick.GetVerticalAxis("Joystick");
            float h = Input.GetAxis("Horizontal") + UltimateJoystick.GetHorizontalAxis("Joystick");

            if (canMove)
            {

                moveDirection = new Vector3(h, 0, v);
                //벡터를 정규화 = 조이스틱 움직이는 정도의 상관없이 일정한속도로 이동하게하기위함
                moveDirection = moveDirection.normalized;

                if (Mathf.Abs(v) > 0 || Mathf.Abs(h) > 0)
                {
                    //캐릭터가 바라보는방향을 데카르트좌표계 
                    //카메라각도가 바뀌더라도 조이스틱 조작방향이 변경없도록 카메라각도를 더함
                    transform.rotation = Quaternion.Euler(0, ang +Camera.main.transform.rotation.eulerAngles.y, 0);

                    lastMoveDirection = moveDirection;
                    anim.SetInteger(id, 1);

                }
                else
                    anim.SetInteger(id, 0);
                playerInfo.playerVector = moveDirection;

            }

 
        }
        else
        {
            //지면이아니면 중력적용
            moveDirection.y -= gravity * Time.unscaledDeltaTime * GameManager.instance.playerTimeScale;
        }



        controller.Move(rot * moveDirection * moveSpeed * Time.unscaledDeltaTime* GameManager.instance.playerTimeScale);

    }

}


/* 만약 CharacterController를 안쓸경우

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMoveCtrl : MonoBehaviour {

    // 이동 관련 변수
    private float ang = 0.0f;
    private float ver = 0.0f;
    
    // 자기 자신의 트렌스폼을 가리킴 
    private Transform myTr;
    //이동 속도 변수 
    public float moveSpeed = 10.0f;
    
    //회전 속도 변수
    public float rotSpeed = 100.0f;
         

    
    void Awake () {
        
        //자기 자신의 Transform 컴포넌트 할당
        myTr = GetComponent<Transform>();

    }
    
    void Update () {
        ang = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
        
        //Debug.Log("ang = " + h.ToString());
        //Debug.Log("ver = " + v.ToString());
        
        //전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * ver) + (Vector3.right * ang);
        
        //Translate(이동 방향 * Time.deltaTime * 변위값 * 속도, 기준좌표)
        tr.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);
        
        //Vector3.up 축을 기준으로 rotSpeed만큼의 속도로 회전 (마우스를 이용한 회전)
        //자동 게임에선 Input.GetAxis("Mouse X") 이거 대신 적 케릭터 위치값 넣으면 된다.
        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));
        
    }
    

 
}
 */
