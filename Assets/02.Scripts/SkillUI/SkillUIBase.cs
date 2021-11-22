using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUIBase : MonoBehaviour
{
    bool SkillUIActiavted = false;
    [SerializeField]
    //스킬 창
    private GameObject goSkillBase;


    public void OnBtn()
    {
        SkillUIActiavted = !SkillUIActiavted;
        if (SkillUIActiavted)
            OpenUI();
        else
            CloseUI();
    }

    void OpenUI()
    {
        goSkillBase.SetActive(true);
    }

    void CloseUI()
    {
        goSkillBase.SetActive(false);
    }
}
