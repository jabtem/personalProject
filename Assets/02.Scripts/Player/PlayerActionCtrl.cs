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

    //콤보 가능여부판당용 현재 애니메이션의 진행도
    float animTime = 0;
    int comboStepID;


    //해당캐릭터의 일반공격 콤보최대횟수
    [HideInInspector]
    public int maxComboCount;


    //회피거리
    [HideInInspector]
    public float dodgeDistance;

    //가드횟수
    [HideInInspector]
    public int guardCount;

    void Awake()
    {
        anim = GetComponent<Animator>();
        comboStepID = Animator.StringToHash("comboStep");
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

        if (anim.GetInteger(comboStepID) == maxComboCount)
        {
            return;
        }

        if (anim.GetInteger(comboStepID) == 0)
        {

            anim.applyRootMotion = true;
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
        anim.applyRootMotion = false;
        anim.SetInteger(comboStepID, 0);

    }
}
