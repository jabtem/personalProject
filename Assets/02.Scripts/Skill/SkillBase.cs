using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//스킬베이스용 추상클래스
public abstract class SkillBase : MonoBehaviour
{
    //스킬타입 구분용
    public enum Type {Point, Projectile , Area}


    //플레이어 위치
    Vector3 _pos;
    public Vector3 Pos
    {
        get => _pos;
        set => _pos = value;
    }

    //플레이어가 바라보는방향
    [SerializeField]
    Vector3 _dir;
    public Vector3 Dir
    {
        get => _dir;
        set => _dir = value;
    }

    Vector3 _rot;
    public Vector3 Rot
    {
        get => _rot;
        set => _rot = value;
    }

    int _skillNum;

    public int SkillNum
    {
        get => _skillNum;
        set => _skillNum = value;
    }

    Type _skillType;

    public Type SkillType
    {
        get => _skillType;
        set => _skillType = value;
    }

    //스킬별 효과구현용
    public abstract void SkillEffect();
}

