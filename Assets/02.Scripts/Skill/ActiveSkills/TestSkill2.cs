using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill2 : SkillBase
{
    public Vector3 myRot;
    float time;

    public override void SkillEffect()
    {
        time += Time.deltaTime;

        transform.Translate(Vector3.forward * Time.deltaTime*5);

        if (time >= 3.0f)
        {
            time = 0f;
            GameManager.instance.Effect.PushEffect(SkillNum, this.gameObject);
        }
    }
    private void Awake()
    {
        myRot = new Vector3(0f, 0f, 90f);
    }


    private void Start()
    {
        SkillType = Type.Projectile;
    }

    private void OnEnable()
    {
        transform.position = new Vector3(Pos.x, 1.5f, Pos.z) + Dir * 3f;
        transform.rotation = Quaternion.Euler(myRot.x, myRot.y + Rot.y, myRot.z);
    }

    private void Update()
    {
        SkillEffect();
    }
}
