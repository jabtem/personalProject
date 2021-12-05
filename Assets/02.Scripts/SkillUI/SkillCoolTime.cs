using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillCoolTime : MonoBehaviour
{
    public Image coolTimeImage;

    //쿨타임 공식 fillamount = 1 - (현재시간/ 쿨타임시간);

    public void StartCoolTime(float cooltime, Action<bool> disalbeSkill)
    {
        StartCoroutine(CoolTime(cooltime,(value)=> disalbeSkill(value)));
    }


    IEnumerator CoolTime(float cooltime, Action<bool> result)
    {
        coolTimeImage.fillAmount = 1;
        float starttime = 0;

        while(starttime < cooltime)
        {
            starttime += Time.deltaTime;
            coolTimeImage.fillAmount = 1 - (starttime / cooltime);
            yield return null;
        }

        result(false);
    }

}
