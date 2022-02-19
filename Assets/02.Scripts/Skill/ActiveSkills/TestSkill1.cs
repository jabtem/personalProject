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
        transform.position = new Vector3(Pos.x, 1.5f, Pos.z) + Dir * 3f;
    }

    private void Update()
    {
        
    }
}
