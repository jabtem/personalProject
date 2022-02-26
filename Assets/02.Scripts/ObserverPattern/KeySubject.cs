using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySubject : MonoBehaviour
{
    public delegate void KeyNotification();

    event KeyNotification keyNotification;

    public int NextKey;

    static KeySubject _instance;

    public static KeySubject Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != null)
        {
            Destroy(this.gameObject);
        }
    }

    public void Add(KeyNotification noti)
    {

        keyNotification += noti;
    }

    public void Remove(KeyNotification noti)
    {
        keyNotification -= noti;
    }

    public void ObserverUpdate()
    {
        keyNotification();
        NextKey++;
    }

}
