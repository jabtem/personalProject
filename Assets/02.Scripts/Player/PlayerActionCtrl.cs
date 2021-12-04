using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerActionCtrl : MonoBehaviour
{
    public enum specialAction
    {
        Dodge, Guard
    }








    Animator anim;
    AnimatorTransitionInfo transitionInfo;
    //콤보 가능여부
    public bool comboPossible;
    public specialAction SA;
    CharacterController controller;
    PlayerMoveCtrl pMove;

    //콤보 가능여부판당용 현재 애니메이션의 진행도
    float animTime = 0;
    int comboStepID;
    int specialActionID;
    int idleID;
    int skillNum;


    //해당캐릭터의 일반공격 콤보최대횟수
    [HideInInspector]
    public int maxComboCount;


    //회피거리
    [HideInInspector]
    public float dodgeTime;
    [HideInInspector]
    public float dodgeSpeed;

    //가드횟수
    [HideInInspector]
    public int guardCount;


    //스킬액션 관련변수
    int skilId;
    public SkillDataReader skillButt;

    void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        comboStepID = Animator.StringToHash("comboStep");
        skillNum = Animator.StringToHash("skillNum");
        idleID = Animator.StringToHash("Base Layer.Idle");
        pMove = GetComponent<PlayerMoveCtrl>();

        //스킬데이터 읽어오는 오브젝트가 단하나이기때문에 허용
        skillButt = GameObject.FindObjectOfType<SkillDataReader>().GetComponent<SkillDataReader>();
    }

    void Update()
    {
        transitionInfo = anim.GetAnimatorTransitionInfo(0);
        animTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime * 100f;
        if (anim.GetInteger(comboStepID) > 0)
        {
            comboDelay();

            if (animTime > 100f)
            {
                //애니메이션이벤트없이 콤보를리셋하도록 수정
                ComboReset();
            }
        }


        if(anim.GetCurrentAnimatorStateInfo(0).fullPathHash == idleID)
        {
            //pMove.canMove = true;
        }

        //현재 베이스레이어의 애니메이션의 진행상태
        //if (animTime > 100f)
        //{
        //    animTime = animTime % 100;
        //}


    }

    void comboDelay()
    {
        //애니메이션 교체가 완료되면
        //어택애니메이션에서만 동작하도록
        if (anim.GetInteger(comboStepID) >0 && anim.GetInteger(comboStepID) < 3)
        {
            if (animTime > 50f && animTime < 80f)
            {
                ComboPossible();
            }
            else if (animTime > 80f)
            {
                ComboImpossible();
            }
            else if (animTime <= 50f)
            {
                ComboImpossible();
            }

        }



    }

    public void Attack()
    {

        Debug.Log("Attack!");
        pMove.canMove = false;

        if (anim.GetInteger(comboStepID) == maxComboCount)
        {
            return;
        }




        if (anim.GetInteger(comboStepID) == 0)
        {
            //공격애니메이션이 루트모션적용하기에 적합하지않음
            //anim.applyRootMotion = true;
            anim.SetInteger(comboStepID, 1);

        }
        else if(anim.GetInteger(comboStepID) != 0)
        {
            if (comboPossible)
            {
                anim.SetInteger(comboStepID, anim.GetInteger(comboStepID) +1);
            }
        }
    }

    //특수액션 가드 of 회피
    public void SpecialAction()
    {
        if (anim.GetInteger(comboStepID) > 0)
        {
            return;
        }


        if (SA == specialAction.Dodge)
        {

            StartCoroutine(Dodge());
        }
        else if(SA == specialAction.Guard)
        {
            GuardButtDown();
        }
    }


    IEnumerator Dodge()
    {
        //회피로직
        // 속도(이동속도*회피속도)로 회피시간만큼 이동한다
        //예) doegeSpeed = 5
        //이동속도의 5배로 회피시간만큼 순간적으로빠르게이동
        specialActionID = Animator.StringToHash("dodge");
        anim.SetBool(specialActionID, true);

        Debug.Log("aa");
        float startTime = 0;

        pMove.canMove = false;

        while(startTime <dodgeTime)
        {
            startTime += Time.deltaTime;

            pMove.moveDirection.x = pMove.lastMoveDirection.x* dodgeSpeed ;
            pMove.moveDirection.z = pMove.lastMoveDirection.z* dodgeSpeed;

            yield return null;
        }
        //지정된시간만큼 회피이동을한후로 조이스틱조작을 허용하고 애니메이션 되돌림
        anim.SetBool(specialActionID, false);
        pMove.canMove = true;
    }

    void GuardButtDown()
    {
        specialActionID = Animator.StringToHash("guard");
        anim.SetBool(specialActionID, true);
    }
    public void GuardButtUp()
    {
        specialActionID = Animator.StringToHash("guard");
        anim.SetBool(specialActionID, false);
    }


    public void ComboPossible()
    {
        comboPossible = true;
    }
    public void ComboImpossible()
    {
        comboPossible = false;
    }

    public void ComboReset()
    {
        comboPossible = false;
        //anim.applyRootMotion = false;
        anim.SetInteger(comboStepID, 0);
        pMove.canMove = true;
    }

    public void UseSkill()
    {
        skilId = skillButt.GetCurrentSKilInfo();
        anim.SetInteger(skillNum, skilId % 1000);
        Debug.Log(skilId);
    }

    //스킬모션 리셋용 임시이벤트함수
    public void MontionReset()
    {
        anim.SetInteger(skillNum, -1);
    }
}
