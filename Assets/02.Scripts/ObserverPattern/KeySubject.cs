using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySubject : MonoBehaviour
{
    public delegate void KeyNotification();

    KeyNotification keyNotification;


    public void Add(KeyNotification noti)
    {
        //GetInvocationList = 델리게이트 호출목록 반환함수
        foreach (KeyNotification keyNotify in keyNotification.GetInvocationList())
        {
            if(keyNotify.Equals(noti))
                {
                return;
            }
        }

        keyNotification += noti;
    }

    public void Remove(KeyNotification noti)
    {
        keyNotification -= noti;
    }

    public void Notify()
    {
        keyNotification.Invoke();
    }

}
