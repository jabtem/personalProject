using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillCtrl : MonoBehaviour
{
    int skilId;
    public SkillDataReader skillButt;

    void Awake()
    {
        //스킬데이터 읽어오는 오브젝트가 단하나이기때문에 허용
        skillButt = GameObject.FindObjectOfType<SkillDataReader>().GetComponent<SkillDataReader>();
    }

    public void UseSkill()
    {
        skilId = skillButt.GetCurrentSKillID();
        Debug.Log(skilId);
    }

}
