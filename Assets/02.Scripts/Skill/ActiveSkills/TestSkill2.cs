using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill2 : SkillBase
{

    public override void SkillEffect()
    {
        transform.Translate(Dir * Time.deltaTime);
    }

    private void Start()
    {
        transform.position = new Vector3(Pos.x, 1.5f, Pos.z) + Dir * 3f;
    }

    private void Update()
    {

        SkillEffect();
    }
}
