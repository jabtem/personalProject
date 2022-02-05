using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    [SerializeField]
    RectTransform hpBar;
    [SerializeField]
    Text hpValue;

    [SerializeField]
    int Hp;

    float hpBarWidth;
    float maxHp;

    private void Awake()
    {
        hpBar = GameObject.FindGameObjectWithTag("PlayerHp").GetComponent<RectTransform>();
        hpValue = GameObject.FindGameObjectWithTag("PlayerHpText").GetComponent<Text>();
        hpBarWidth = hpBar.rect.width;
        maxHp = Hp;
    }

    private void Start()
    {
        hpValue.text = Hp.ToString();
    }

    public void Damaged(int damage)
    {
        Hp -= damage;
        hpValue.text = Hp.ToString();
        hpBar.sizeDelta = new Vector2((Hp / maxHp) * hpBarWidth, hpBar.sizeDelta.y);

    }


}
