using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill2 : SkillBase
{

    float time;

    public override void SkillEffect()
    {
        transform.Translate(Dir * Time.deltaTime*2);
    }

    private void OnEnable()
    {
        transform.position = new Vector3(Pos.x, 1.5f, Pos.z) + Dir * 3f;
    }

    private void Update()
    {
        time += Time.deltaTime;
        SkillEffect();

        if(time >= 3.0f)
        {
            time = 0f;
            GameManager.instance.Effect.PushEffect(SkillNum, this.gameObject);
        }
    }
}
