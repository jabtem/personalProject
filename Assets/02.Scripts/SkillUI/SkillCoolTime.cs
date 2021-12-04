using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillCoolTime : MonoBehaviour
{
    public Image coolTimeImage;

    //쿨타임 공식 fillamount = 1 - (현재시간/ 쿨타임시간);


    IEnumerator CoolTime(float time)
    {

        yield return new WaitForSeconds(time);
    }

}
