using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCtrl : MonoBehaviour
{
    Animator anim;
    //콤보 가능여부
    public bool comboPossible;

    //콤보 가능여부판당용
    public float animTime = 0;

    int id;

    //이전 애니메이션 해쉬값
    int preAnimHash;


    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    { 
        comboDelay();
        //현재 베이스레이어의 애니메이션의 진행상태
        animTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime * 100f;
        if (animTime > 100f)
            animTime = 100f;

    }

    void comboDelay()
    {
        //애니메이션 교체가 완료되면
        if(preAnimHash != anim.GetCurrentAnimatorStateInfo(0).fullPathHash)
        {
            Debug.Log("AnimChange!");
        }


        if (animTime > 50f && animTime < 80f)
        {
            ComboPossible();
        }
        else if(animTime > 80f)
        {
            ComboImpossible();
        }
    }

    public void Attack()
    {
        id = Animator.StringToHash("comboStep");


        if (anim.GetInteger(id) == 0)
        {
            id = Animator.StringToHash("Base Layer.AttackA");
            anim.Play(id);
            id = Animator.StringToHash("comboStep");
            anim.SetInteger(id, 1);

        }
        else if(anim.GetInteger(id) != 0)
        {
            if(comboPossible)
            {
                preAnimHash = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
                comboPossible = false;
                id = Animator.StringToHash("comboStep");
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
