using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill1 : SkillBase
{

    float time;
    public override void SkillEffect()
    {
        time += Time.deltaTime;

        if (time >= 2.0f)
        {
            time = 0f;
            GameManager.instance.Effect.PushEffect(SkillNum, this.gameObject);
        }
    }

    private void Start()
    {
        SkillType = Type.Point;
    }

    private void OnEnable()
    {
        transform.position = new Vector3(Pos.x, 1.5f, Pos.z) + Dir * 1.5f;
    }

    private void Update()
    {
        SkillEffect();
    }
}
