using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill1 : SkillBase
{
    public override void SkillEffect()
    {
        
    }

    private void Start()
    {
        transform.position = Pos;
    }
}
