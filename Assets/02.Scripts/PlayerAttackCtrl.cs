using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCtrl : MonoBehaviour
{
    public Animator anim;
    //콤보 가능여부
    public bool comboPossible;

    //콤보스템
    public int step;
    int id;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Attack()
    {

        if(step == 0)
        {
            id = Animator.StringToHash("Base Layer.AttackA");
            anim.Play(id);
            step = 1;
        }
        if(step !=0)
        {
            if(comboPossible)
            {
                comboPossible = false;
                step += 1;
            }
        }
    }

    public void ComboPossible()
    {
        comboPossible = true;
    }
    public void Combo()
    {
        if (step == 2)
        {
            id = Animator.StringToHash("Base Layer.AttackB");
            anim.Play(id);
        }
        if (step == 3)
        {
            id = Animator.StringToHash("Base Layer.AttackC");
            anim.Play(id);
        }
    }

    public void ComboReset()
    {
        comboPossible = false;
        step = 0;
    }
}
