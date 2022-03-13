using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySubject : MonoBehaviour
{
    public delegate void KeyNotification();

    event KeyNotification keyNotification;

    //시작 키 넘버
    public int startKey;

    [SerializeField]
    int _nextKey;
    public int nextKey
    {
        get => _nextKey;
        set => _nextKey = value;
    }


    public static KeySubject Instance;

    private void Awake()
    {
        if(KeySubject.Instance == null)
        {
            KeySubject.Instance = this;
        }
    }

    public void Start()
    {
        ObserverUpdate();
    }


    public void Add(KeyNotification noti)
    {
        keyNotification += noti;
    }

    public void Remove(KeyNotification noti)
    {
        keyNotification -= noti;
    }

    public void OnEnable()
    {
        nextKey = startKey;
    }

    public void ObserverUpdate()
    {
        keyNotification();
        nextKey++;
    }

    [ContextMenu("Test")]
    public void Show()
    {
        foreach(var a in keyNotification.GetInvocationList())
        {
            Debug.Log(a);
        }
    }

}
