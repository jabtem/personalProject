using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerActionCtrl : MonoBehaviour
{
    public enum specialAction
    {
        Dodge, Guard
    }


    public Animator anim;
    //콤보 가능여부

    public bool comboPossible;
    public specialAction SA;
    CharacterController controller;
    PlayerMoveCtrl pMove;
    
    //콤보나 스킬 재사용여부 판단용, 현재 애니메이션의 진행도확인용변수
    public float animTime = 0;

    //애니메이션 파라미터 ID저장용
    int comboStepID;
    int specialActionID;
    int speedID;
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

    //스킬사용가능여부
    public bool disableSkill;

    BoxCollider attackCol;


    Damage attackDamage;

    PlayerHp playerHp;

    void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        comboStepID = Animator.StringToHash("comboStep");
        skillNum = Animator.StringToHash("skillNum");
        speedID = Animator.StringToHash("speed");
        pMove = GetComponent<PlayerMoveCtrl>();
        attackCol = gameObject.GetComponentInChildren<BoxCollider>();
        attackDamage = gameObject.GetComponentInChildren<Damage>();
        TryGetComponent<PlayerHp>(out playerHp);

    }

    void Update()
    {
        //nomaralizedTime은 0~1까지 소수점이지만 관리하기쉽도록 0~100으로 표현수정
        //스킬모션과 어택모션은 루프하는애니메이션이아니므로 
        if (anim.GetInteger(comboStepID) > 0 && !anim.GetCurrentAnimatorStateInfo(0).loop)
        {
            animTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime * 100f;
            ComboDelay();
        }
        else if(anim.GetInteger(skillNum) > 0&& !anim.GetCurrentAnimatorStateInfo(0).loop)
        {
            animTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime * 100f;
            SkillMotionCheck();
        }


    }

    void ComboDelay()
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
        //트랜지션 전환시간고려
        if (animTime >= 100f)
        {
           
            ComboReset();
        }



    }


    //현재 스킬모션이 다끝낫는지 여부
    void SkillMotionCheck()
    {
        if(anim.GetInteger(skillNum) > 0)
        {
            if (animTime > 100f)
            {

                anim.SetInteger(skillNum, 0);
                pMove.canMove = true;
            }
        }
    }

    public void Attack()
    {
        //정지상태일때만 공격가능
        if (anim.GetInteger(speedID) != 0)
        {
            return;
        }
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
            attackDamage.DamageValue = 10;

        }
        else if(anim.GetInteger(comboStepID) != 0)
        {
            if (comboPossible)
            {
                anim.SetInteger(comboStepID, anim.GetInteger(comboStepID) +1);

                if(anim.GetInteger(comboStepID) == 2)
                {
                    attackDamage.DamageValue = 10;
                }
                else if(anim.GetInteger(comboStepID) == 3)
                {
                    attackDamage.DamageValue = 20;
                }
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
            CoroutineManager.StartUpdateCoroutine(Dodge(),this);
            //StartCoroutine(Dodge());
        }
        else if(SA == specialAction.Guard)
        {
            GuardButtDown();
        }
    }


    IEnumerator Dodge()
    {
        //회피로직
        // 속도(이동속도*회피속도)로 회피시간만큼 마지막으로 이동한 방향으로 회피
        //예) doegeSpeed = 5
        //이동속도의 5배로 회피시간만큼 순간적으로빠르게이동
        //퍼펙트존에만 닿은상태에서 회피행동을한경우
        if(pzoneHit && !enemyHit)
        {
            GameManager.instance.SetTimeScale(0.5f);
            GameManager.instance.postProcessingManager.TimeSlowEffect(0.7f);
            MotionTrailObjectPoolManager.instance.motionTrailOn = true;
            GameManager.instance.InvokeResetTimeSlow(3f);
           
        }

        specialActionID = Animator.StringToHash("dodge");
        anim.SetBool(specialActionID, true);
        float startTime = 0;

        pMove.canMove = false;

        while(startTime <dodgeTime)
        {
            startTime += Time.unscaledDeltaTime*GameManager.instance.playerTimeScale;

            pMove.moveDirection.x = pMove.lastMoveDirection.x* dodgeSpeed ;
            pMove.moveDirection.z = pMove.lastMoveDirection.z* dodgeSpeed;

            yield return null;
        }
        //지정된시간만큼 회피이동을한후로 조이스틱조작을 허용하고 애니메이션 되돌림
        anim.SetBool(specialActionID, false);
        pMove.canMove = true;
    }


    #region 가드
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
    #endregion


    #region 콤보체크
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
        attackDamage.DamageValue = 0;
        comboPossible = false;
        anim.SetInteger(comboStepID, 0);
        pMove.canMove = true;
        if(attackCol.enabled)
        {
            attackCol.enabled = false;
        }
    }

    #endregion

    public void UseSkill()
    {

        //정지상태일때만 스킬사용가능
        if(anim.GetInteger(speedID) != 0)
        {
            return;
        }


        //공격중엔스킬사용불가
        if (!disableSkill && anim.GetInteger(skillNum) <= 0 )
        {
            if (skilId != 0)
            {
                disableSkill = true;
                pMove.canMove = false;
            }
            //코루틴은 레퍼런스를 직접사용할수가없으므로 Action사용
            skilId = SkillDataReader.instance.GetCurrentSKilInfo((value) => disableSkill = value);
            anim.SetInteger(skillNum, skilId%1000);



        }


    }


    public bool pzoneHit = false;
    public bool enemyHit = false;


    //슬로우모션 구현용 테스트코드

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PerfectZone"))
        {
            pzoneHit = true;

        }
        else if(other.gameObject.CompareTag("EnemyAttack"))
        {
            enemyHit = true;

            Damage dam;
            if (!other.gameObject.TryGetComponent<Damage>(out dam))
            {
                return;
            }
            playerHp.Damaged(dam.DamageValue);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("PerfectZone"))
        {
            pzoneHit = false;
        }
        else if (other.gameObject.CompareTag("EnemyAttack"))
        {
            enemyHit = false;
        }
    }

    public void AttackColEnable(int value)
    {
        switch(value)
        {
            case 0:
                attackCol.enabled = false;
                break;
            case 1:
                attackCol.enabled = true;
                break;


        }
    }
    public int GetComboStep()
    {
        return anim.GetInteger(comboStepID);
    }


    [ContextMenu("StopAll")]
    public void stopTest()
    {
        CoroutineManager.StopAllUpdateCoroutine(this);
    }
}
