using System.Collections;
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
    public GameObject katanaSkillSet;
    public GameObject spearSkillSet;
    public GameObject twoHandsSkillSet;



    //배열 0:공격버튼 1:특수액션
    //public Button[] actionButtos;
    public EventTrigger[] actionButtons;
    GameObject playerChracter;
    public void Create(int num)
    {


        switch (num)
        {
            case 1:
                playerChracter = GameObject.Instantiate(katana, this.gameObject.transform.position, this.gameObject.transform.rotation);
                katanaSkillSet.SetActive(true);
                break;
            case 2:
                playerChracter = GameObject.Instantiate(spear, this.gameObject.transform.position, this.gameObject.transform.rotation);
                spearSkillSet.SetActive(true);
                break;
            case 3:
                playerChracter = GameObject.Instantiate(twoHandSword, this.gameObject.transform.position, this.gameObject.transform.rotation);
                twoHandsSkillSet.SetActive(true);
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
            PlayerActionCtrl actionCtrl = go.GetComponent<PlayerActionCtrl>();
            ////버튼할당된 리스너 초기화
            //for(int i=0; i< actionButtos.Length; ++i)
            //{
            //    actionButtos[i].onClick.RemoveAllListeners();
            //}

            //actionButtos[0].onClick.AddListener(delegate { attCtrl.Attack(); });
            for(int i=0; i< actionButtons.Length; ++i)
            {
                EventTrigger.Entry triggerDownEntry = new EventTrigger.Entry();
                triggerDownEntry.eventID = EventTriggerType.PointerDown;
                triggerDownEntry.callback.RemoveAllListeners();
                switch (i)
                {
                    //공격버튼 세팅
                    case 0:

                        triggerDownEntry.callback.AddListener((data) => { actionCtrl.Attack(); });
                        break;
                    //특수액션 세팅
                    case 1:
                        //델리게이트(대리자)
                        //triggerEntry.callback.AddListener(delegate { attCtrl.SpecialAction(); });
                        //람다식
                        triggerDownEntry.callback.AddListener((data) => { actionCtrl.SpecialAction(); });
                        if(actionCtrl.SA == PlayerActionCtrl.specialAction.Guard)
                        {
                            EventTrigger.Entry triggerUpEntry = new EventTrigger.Entry();
                            triggerUpEntry.eventID = EventTriggerType.PointerUp;
                            triggerUpEntry.callback.AddListener((data) => { actionCtrl.GuardButtUp(); });
                            actionButtons[i].triggers.Add(triggerUpEntry);
                        }

                        break;
                }
                actionButtons[i].triggers.Add(triggerDownEntry);
            }
            






        }
    }
}


