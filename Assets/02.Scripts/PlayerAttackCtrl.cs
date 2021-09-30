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
        id = Animator.StringToHash("comboStep");

        if(anim.GetInteger(id) == 0)
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
    public void Combo()
    {
        id = Animator.StringToHash("comboStep");

        if (anim.GetInteger(id) == 2)
        {
            id = Animator.StringToHash("Base Layer.AttackB");
            anim.Play(id);
        }
        else if (anim.GetInteger(id) == 3)
        {
            id = Animator.StringToHash("Base Layer.AttackC");
            anim.Play(id);
        }
    }

    public void ComboReset()
    {
        comboPossible = false;
        id = Animator.StringToHash("comboStep");
        anim.SetInteger(id, 0);
    }
}
