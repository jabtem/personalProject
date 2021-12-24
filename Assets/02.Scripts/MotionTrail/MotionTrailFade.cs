using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrailFade : MonoBehaviour
{
    MeshRenderer meshRen;
    MotionTrailContainer mtContainer;
    float alpha;
    string _shaderValueName;
    public string shaderValueName
    {
        get
        {
            return _shaderValueName;
        }
        set
        {
            _shaderValueName = value;
        }
    }

    void Awake()
    {
        meshRen = GetComponent<MeshRenderer>();
    }


    IEnumerator FadeOut()
    {
        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            meshRen.material.SetFloat(shaderValueName, alpha);
            yield return null;
        }

        mtContainer.ActiveFalse();
        yield break;
    }
    void OnEnable()
    {
        alpha = 1f;
        meshRen.material.SetFloat(shaderValueName, alpha);
        StartCoroutine(FadeOut());
    }

    public void SetParentContainer(MotionTrailContainer mtCon)
    {
        mtContainer = mtCon;
    }
}
