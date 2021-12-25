using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrailContainer : MonoBehaviour
{


    Material motionTrailMat;

    string colorValueName;
    string alphaValueNmae;
    int key;

    Transform targetTr { get; set; }
    Color color;

    GameObject[] skinMeshTagObj;
    //원하는 메쉬에 대해서만 잔상을 만들기위해
    SkinnedMeshRenderer[] skinnedMeshRenderer;

    GameObject[] motionTrailObj;



    private void Awake()
    {
        skinMeshTagObj = GameObject.FindGameObjectsWithTag("SkinMesh");
        TargetSetting(GameObject.FindGameObjectWithTag("Player").transform);


        if(skinMeshTagObj != null)
        {
            skinnedMeshRenderer = new SkinnedMeshRenderer[skinMeshTagObj.Length];
            for (int i=0; i< skinMeshTagObj.Length; i++)
            {
                skinnedMeshRenderer[i] = skinMeshTagObj[i].GetComponent<SkinnedMeshRenderer>();
            }
        }



    }


    void OnEnable()
    {
        if (motionTrailObj != null)
        {
            ReuseMotionTrailGroup();
        }
        else if(motionTrailObj == null)
        {
            CreateMotionTrailGroup();
        }
    }


    /************* CreateMotionTrail******************
    ************* 잔상 생성로직 **********************/
    public void CreateMotionTrailGroup()
    {
        
        motionTrailObj = new GameObject[skinnedMeshRenderer.Length];

        for (int i =0; i< skinnedMeshRenderer.Length; i++)
        {
            Mesh mesh = new Mesh();
            skinnedMeshRenderer[i].BakeMesh(mesh);

            GameObject obj = new GameObject($"{targetTr.gameObject.name} MotionTrail");
            motionTrailObj[i] = obj;
            MeshFilter mf = obj.AddComponent<MeshFilter>();
            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            MotionTrailFade mtFade = obj.AddComponent<MotionTrailFade>();
            mtFade.shaderValueName = alphaValueNmae;
            mtFade.SetParentContainer(this);
            mf.mesh = mesh;
            mr.material = motionTrailMat;
            mr.material.SetColor(colorValueName, color);
            obj.transform.position = targetTr.position;
            obj.transform.rotation = targetTr.rotation;
            obj.transform.SetParent(this.transform);
        }
    }



    /****************ReuseMotionTrail*******************
     ****************잔상 재사용 로직*******************/
    public void ReuseMotionTrailGroup()
    {
        for (int i = 0; i < skinnedMeshRenderer.Length; i++)
        {
            Mesh mesh = new Mesh();
            skinnedMeshRenderer[i].BakeMesh(mesh);

            MeshFilter mf = motionTrailObj[i].GetComponent<MeshFilter>();
            MeshRenderer mr = motionTrailObj[i].GetComponent<MeshRenderer>();
            MotionTrailFade mtFade = motionTrailObj[i].GetComponent<MotionTrailFade>();
            mtFade.shaderValueName = alphaValueNmae;
            mtFade.SetParentContainer(this);
            mf.mesh = mesh;
            mr.material = motionTrailMat;
            mr.material.SetColor(colorValueName, color);
            motionTrailObj[i].transform.position = targetTr.position;
            motionTrailObj[i].transform.rotation = targetTr.rotation;
        }


    }



    public void TargetSetting(Transform tr)
    {
        targetTr = tr;
    }

    public void ActiveFalse()
    {
        gameObject.SetActive(false);
        MotionTrailObjectPoolManager.instance.SetMotionTrailContainer(gameObject);
    }

    public void MotionTrailContainerSetting(int _key, Material _motionTrailMat, string _colorValueName, string _alphaValueName)
    {
        key = _key;
        motionTrailMat = _motionTrailMat;
        colorValueName = _colorValueName;
        alphaValueNmae = _alphaValueName;

    }


    public void ColorSet(Color _color)
    {
        color = _color;
    }

}
