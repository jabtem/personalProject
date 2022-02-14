using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class CreatePlayer : MonoBehaviour
{
    public RadarMap miniMap;
    public GameObject katanaSkillSet;
    public GameObject spearSkillSet;
    public GameObject twoHandsSkillSet;

    GameObject[] playerObjs;

    string[] paths;


    public void Awake()
    {
        //프리펩 확장자 파일경로
        paths = System.IO.Directory.GetFiles("Assets/03.Prefabs/Player","*.prefab");

        playerObjs = new GameObject[paths.Length];


        for(int i= 0; i< paths.Length; ++i)
        {
            playerObjs[i] = (GameObject)AssetDatabase.LoadAssetAtPath(paths[i], typeof(GameObject));
        }

        //katana = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/03.Prefabs/Player/katanaPlayer.prefab", typeof(GameObject));
        //spear = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/03.Prefabs/Player/spearPlayer.prefab", typeof(GameObject));
        //twoHandSword = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/03.Prefabs/Player/twoHandPlayer.prefab", typeof(GameObject));
    }


    //배열 0:공격버튼 1:특수액션
    //public Button[] actionButtos;
    public EventTrigger[] actionButtons;
    GameObject playerChracter;
    public void Create(int num)
    {


        switch (num)
        {
            case 1:
                playerChracter = GameObject.Instantiate(playerObjs[0], this.gameObject.transform.position, this.gameObject.transform.rotation);
                katanaSkillSet.SetActive(true);
                break;
            case 2:
                playerChracter = GameObject.Instantiate(playerObjs[1], this.gameObject.transform.position, this.gameObject.transform.rotation);
                spearSkillSet.SetActive(true);
                break;
            case 3:
                playerChracter = GameObject.Instantiate(playerObjs[2], this.gameObject.transform.position, this.gameObject.transform.rotation);
                twoHandsSkillSet.SetActive(true);
                break;

        }
        miniMap.SetPlayerPos(playerChracter);
        ActionButtonSet(playerChracter);
        GameManager.instance.SetPlayerAnim(playerChracter.GetComponent<Animator>());

    }
    void ActionButtonSet(GameObject go)
    {
        PlayerActionCtrl actionCtrl;

        if (!go.TryGetComponent<PlayerActionCtrl>(out actionCtrl) )
        {
            //어택컨트롤컴포넌트가없을경우 세팅X
            return;
        }
        ////버튼할당된 리스너 초기화
        //for(int i=0; i< actionButtos.Length; ++i)
        //{
        //    actionButtos[i].onClick.RemoveAllListeners();
        //}

        //actionButtos[0].onClick.AddListener(delegate { attCtrl.Attack(); });
        for (int i = 0; i < actionButtons.Length; ++i)
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
                    if (actionCtrl.SA == PlayerActionCtrl.specialAction.Guard)
                    {
                        EventTrigger.Entry triggerUpEntry = new EventTrigger.Entry();
                        triggerUpEntry.eventID = EventTriggerType.PointerUp;
                        triggerUpEntry.callback.AddListener((data) => { actionCtrl.GuardButtUp(); });
                        actionButtons[i].triggers.Add(triggerUpEntry);
                    }

                    break;
                case 2:
                    triggerDownEntry.callback.AddListener((data) => { actionCtrl.UseSkill(); });
                    break;
            }
            actionButtons[i].triggers.Add(triggerDownEntry);
        }

    }
}


