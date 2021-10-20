using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectChacracterMove : MonoBehaviour
{

    //캐릭터가 클릭되면 이동할위치
    public Transform movePoint;

    //되돌아갈위치
    public Transform backPoint;

    bool _go;

    public int CharacterNum;

    public bool go
    {
        get
        {
            return _go;
        }
        set
        {
            _go = value;
        }
    }


    void Update()
    {
        if(_go)
        {
            transform.position = Vector3.Lerp(gameObject.transform.position, movePoint.position, 0.01f);
        }
        else if(!_go)
        {
            transform.position = Vector3.Lerp(gameObject.transform.position, backPoint.position, 0.01f);
        }
    }


}
