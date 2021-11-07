using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCtrl : MonoBehaviour
{
    Animator anim;
    AnimatorTransitionInfo transitionInfo;
    //콤보 가능여부
    bool comboPossible;

    //콤보 가능여부판당용 현재 애니메이션의 진행도
    float animTime = 0;

    int id;

    //해당캐릭터의 일반공격 콤보최대횟수
    public int maxComboCount;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        id = Animator.StringToHash("comboStep");
        transitionInfo = anim.GetAnimatorTransitionInfo(0);
        animTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime * 100f;
        if (anim.GetInteger(id) > 0)
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
        id = Animator.StringToHash("comboStep");

        //어택애니메이션에서만 동작하도록
        if (anim.GetInteger(id) >0 && anim.GetInteger(id)< 3)
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
        id = Animator.StringToHash("comboStep");



        if (anim.GetInteger(id) == maxComboCount)
        {
            return;
        }

        if (anim.GetInteger(id) == 0)
        {

            anim.SetInteger(id, 1);

        }
        else if(anim.GetInteger(id) != 0)
        {
            if (comboPossible)
            {
                anim.SetInteger(id, anim.GetInteger(id)+1);
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
        id = Animator.StringToHash("comboStep");
        anim.SetInteger(id, 0);

    }
}
