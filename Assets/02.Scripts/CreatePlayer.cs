﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreatePlayer : MonoBehaviour
{
    public GameObject katana;
    public GameObject spear;
    public GameObject twoHandSword;
    public RadarMap miniMap;

    //배열 0:공격버튼
    public Button[] actionButtos;
    public EventTrigger[] aa;
    GameObject playerChracter;
    public void Create(int num)
    {


        switch (num)
        {
            case 1:
                playerChracter = GameObject.Instantiate(katana, this.gameObject.transform.position, this.gameObject.transform.rotation);
                break;
            case 2:
                playerChracter = GameObject.Instantiate(spear, this.gameObject.transform.position, this.gameObject.transform.rotation);
                break;
            case 3:
                playerChracter = GameObject.Instantiate(twoHandSword, this.gameObject.transform.position, this.gameObject.transform.rotation);
                break;

        }
        miniMap.SetPlayerPos(playerChracter);
        ActionButtonSet(playerChracter);

    }
    void ActionButtonSet(GameObject go)
    {
        if(go.GetComponent<PlayerActionCtrl>() == null)
        {
            //어택컨트롤컴포넌트가없을경우 세팅X
            return;
        }
        else
        {
            PlayerActionCtrl attCtrl = go.GetComponent<PlayerActionCtrl>();
            //버튼할당된 리스너 초기화
            for(int i=0; i< actionButtos.Length; ++i)
            {
                actionButtos[i].onClick.RemoveAllListeners();
            }

            actionButtos[0].onClick.AddListener(delegate { attCtrl.Attack(); });
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.RemoveAllListeners();
            //델리게이트(대리자)
            //entry.callback.AddListener(delegate { attCtrl.SpecialAction(); });
            //람다식
            entry.callback.AddListener((date) => { attCtrl.SpecialAction(); });
            aa[0].triggers.Add(entry);


        }
    }
}


