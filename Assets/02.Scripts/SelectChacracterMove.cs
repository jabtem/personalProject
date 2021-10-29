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

    //애니메이션 파라메터 ID
    int parameterID;
    //애니메이션 속도 ID
    int animSpeed;

    Animator anim;
    Rigidbody rigid;


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

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        parameterID = Animator.StringToHash("move");
        animSpeed = Animator.StringToHash("animSpeed");
    }


    void Update()
    {
        if(_go)
        {
            transform.position = Vector3.Lerp(gameObject.transform.position, movePoint.position, 0.01f);
            if(Mathf.Abs(transform.position.x - movePoint.position.x) + Mathf.Abs(transform.position.z - movePoint.position.z) < 0.5f)
            {
                anim.SetBool(parameterID, false);
            }
            else
            {
                anim.SetBool(parameterID, true);
                anim.SetFloat(animSpeed, 1f);
            }
        }
        else if(!_go)
        {
            transform.position = Vector3.Lerp(gameObject.transform.position, backPoint.position, 0.01f);
            if (Mathf.Abs(transform.position.x - backPoint.position.x) + Mathf.Abs(transform.position.z - backPoint.position.z) < 0.5f)
            {
                anim.SetBool(parameterID, false);
            }
            else
            {
                anim.SetBool(parameterID, true);
                anim.SetFloat(animSpeed, -1f);
            }
        }

    }


}
