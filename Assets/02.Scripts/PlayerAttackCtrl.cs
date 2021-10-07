using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCtrl : MonoBehaviour
{
    Animator anim;
    //콤보 가능여부
    public bool comboPossible;

    //콤보 가능여부판당용
    public float curDelay =0;

    int id;
    bool timerOn;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Attack()
    {
        id = Animator.StringToHash("comboStep");

        if(anim.GetInteger(id) == 0)
        {
            timerOn = true;
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
