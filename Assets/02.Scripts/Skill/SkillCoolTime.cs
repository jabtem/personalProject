using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillCoolTime : MonoBehaviour
{
    public Image coolTimeImage;
    public Text coolTimeText;

    //쿨타임 공식 fillamount = 1 - (현재시간/ 쿨타임시간);

    public void StartCoolTime(float cooltime, Action<bool> disalbeSkill)
    {
        StartCoroutine(CoolTime(cooltime, (value) => disalbeSkill(value)));
        //CoroutineManager.StartUpdateCoroutine(CoolTime(cooltime, (value) => disalbeSkill(value)),this);

    }


    IEnumerator CoolTime(float cooltime, Action<bool> result)
    {
        SkillSlot skillButt = GetComponent<SkillSlot>();

        //쿨타임도는동안 스킬교체 불가능
        skillButt.canDrop = false;

        coolTimeImage.gameObject.SetActive(true);
        coolTimeImage.fillAmount = 1;
        coolTimeText.text = cooltime.ToString();
        float startTime = 0;

        while(startTime < cooltime)
        {
            startTime += Time.unscaledDeltaTime;
            coolTimeImage.fillAmount = 1 - (startTime / cooltime);
            coolTimeText.text = string.Format("{0:0.#}", cooltime - startTime);
            yield return null;
        }
        coolTimeImage.gameObject.SetActive(false);
        skillButt.canDrop = true;
        result(false);
        yield break;
    }

}
