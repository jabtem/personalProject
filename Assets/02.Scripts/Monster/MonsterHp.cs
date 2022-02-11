using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHp : MonoBehaviour
{

    [SerializeField]
    Image hpBar;
    [SerializeField]
    Canvas hudCanvas;

    [SerializeField]
    int _hp;
    //public int Hp
    //{
    //    //expression-bodied members
    //    //식 본문 멤버 기존get set을축약해서 표현가능
    //    get => _hp;
    //    set => _hp = value;
    //}
    //READONLLY
    public int Hp => _hp;

    float maxHp;

    private void Start()
    {
        maxHp = _hp;
    }

    public void Damaged(int damage)
    {
        _hp -= damage;
        hpBar.fillAmount = _hp / maxHp;
    }
}
