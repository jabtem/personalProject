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
    int Hp;

    float maxHp;

    private void Start()
    {
        maxHp = Hp;
    }

    public void Damaged(int damage)
    {
        Hp -= damage;
        hpBar.fillAmount = Hp / maxHp;
    }
}
