using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChracterSelectRay : MonoBehaviour
{
    Ray ray;
    RaycastHit rayhit;

    void Update()
    {

        //캐릭터만 인식하도록
        int layerMask = (1 << LayerMask.NameToLayer("Player"));

#if UNITY_EDITOR


        ray = Camera.main.ScreenPointToRay(Input.mousePosition);        


        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.blue);
#endif

    }


}
