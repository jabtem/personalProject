using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyObserver : Observer
{
    public int num;


    private void Start()
    {
        KeySubject.Instance.Add(Notify);
        gameObject.SetActive(false);
    }

    //해당옵저버에서 동작할 함수
    public override void Notify()
    {
        if (num.Equals(KeySubject.Instance.NextKey))
        {
            gameObject.SetActive(true);

        }
        else
            gameObject.SetActive(false);
    }
}
