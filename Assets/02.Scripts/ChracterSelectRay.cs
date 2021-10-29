using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChracterSelectRay : MonoBehaviour
{
    Ray ray;
    RaycastHit rayhit;

    GameObject PlayerCharacter;


    void Update()
    {

        //캐릭터만 인식하도록
        int layerMask = (1 << LayerMask.NameToLayer("Player"));

#if UNITY_EDITOR


        ray = Camera.main.ScreenPointToRay(Input.mousePosition);        


        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.blue);

        if(Input.GetMouseButtonDown(0))
        {
            if(Physics.Raycast(ray, out rayhit, 150f, layerMask))
            {
                var objs = GameObject.FindObjectsOfType<SelectChacracterMove>();

                foreach(var obj in objs)
                {
                    obj.go = false;
                }

                SelectChacracterMove chracter = rayhit.collider.gameObject.GetComponent<SelectChacracterMove>();

                chracter.go = true;

                if(GameManager.instance !=null)
                {
                    GameManager.instance.CharacterNum = chracter.CharacterNum;
                }

            }
        }

#endif

    }


}
