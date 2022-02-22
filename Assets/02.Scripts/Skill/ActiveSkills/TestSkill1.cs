using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill1 : SkillBase
{

    float time;
    public Vector3 myRot;
    BoxCollider col;

    public override void SkillEffect()
    {
        time += Time.deltaTime;

        if (time >= 2.0f)
        {
            time = 0f;
            GameManager.instance.Effect.PushEffect(SkillNum, this.gameObject);
        }
    }

    private void Awake()
    {
        myRot = new Vector3(20f, -75f, 0f);
        TryGetComponent<BoxCollider>(out col);
    }

    private void Start()
    {
        SkillType = Type.Point;

    }

    private void OnEnable()
    {
        transform.position = new Vector3(Pos.x, 2f, Pos.z) + Dir * 2f;
        transform.rotation = Quaternion.Euler(myRot.x, myRot.y + Rot.y, myRot.z);
        if(!col.enabled)
        {
            col.enabled = true;
        }
    }

    private void Update()
    {
        SkillEffect();
    }
}
