using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//스킬베이스용 추상클래스
public abstract class SkillBase : MonoBehaviour
{
    //스킬출력용 기본좌표받기위함
    [SerializeField]
    Vector3 _pos;
    public Vector3 Pos
    {
        get => _pos;
        set => _pos = value;
    }
    //스킬별 효과구현용

    public abstract void SkillEffect();
}

