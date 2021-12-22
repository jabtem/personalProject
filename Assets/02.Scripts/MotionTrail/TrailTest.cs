using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTest : MonoBehaviour
{


    public Material motionTrailMat;
    public Gradient motionTrailGradient = new Gradient()
    {
        colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(new Color(1.0f, 0.0f, 0.0f), 0.00f),
            new GradientColorKey(new Color(0.0f, 1.0f, 0.0f), 0.50f),
            new GradientColorKey(new Color(0.0f, 0.0f, 1.0f), 1.0f),
        }
    };


    [Header("잔상색상 변화속도")]
    [Range(0.01f, 1f)]
    public float colorChangeSpeed =1f;

    [Header("잔상생성 주기")]
    [Range(0.1f, 10f)]
    public float motionTrailCycle = 0.1f;

    float motionTrailTime;
    float colorChangeTime;

    GameObject[] skinMeshTagObj;
    //원하는 메쉬에 대해서만 잔상을 만들기위해
    SkinnedMeshRenderer[] skinnedMeshRenderer;
    private void Awake()
    {
        skinMeshTagObj = GameObject.FindGameObjectsWithTag("SkinMesh");

        if(skinMeshTagObj != null)
        {
            skinnedMeshRenderer = new SkinnedMeshRenderer[skinMeshTagObj.Length];
            for (int i=0; i< skinMeshTagObj.Length; i++)
            {
                skinnedMeshRenderer[i] = skinMeshTagObj[i].GetComponent<SkinnedMeshRenderer>();
            }
        }

    }

    private void Update()
    {
        motionTrailTime += Time.deltaTime;

        if (motionTrailTime > motionTrailCycle)
        {
            if (colorChangeTime > 1.0f)
            {
                colorChangeTime = 0f;
            }
            Color color = motionTrailGradient.Evaluate(colorChangeTime);
            colorChangeTime += colorChangeSpeed;

            foreach(var v in skinnedMeshRenderer)
            {
                Mesh mesh = new Mesh();
                v.BakeMesh(mesh);

                GameObject obj = new GameObject($"{gameObject.name} TrailTest");
                MeshFilter mf = obj.AddComponent<MeshFilter>();
                MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                mf.mesh = mesh;
                mr.material = motionTrailMat;
                mr.material.SetColor("_Color", color);
                obj.transform.position = this.transform.position;
                obj.transform.rotation = this.transform.rotation;
                //테스트용이므로 오브젝트풀링x
                Destroy(obj, 1f);
                motionTrailTime = 0f;
            }

        }

        
    }


}
