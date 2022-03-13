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

    private void OnDestroy()
    {
        KeySubject.Instance.Remove(Notify);
    }

    //해당옵저버에서 동작할 함수
    public override void Notify()
    {
        if (num.Equals(KeySubject.Instance.nextKey))
        {
            gameObject.SetActive(true);

        }
        else
            gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            KeySubject.Instance.ObserverUpdate();
        }
    }
}
