﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill2 : SkillBase
{

    public override void SkillEffect()
    {

    }

    private void Start()
    {
        transform.position = Pos + Dir * 10f;
    }

    private void Update()
    {

    }
}
