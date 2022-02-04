using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    //단순 데미지값 보관용 스크립트
    [SerializeField]
    int damage;

    public int DamageValue
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }
}
