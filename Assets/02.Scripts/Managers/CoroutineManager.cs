using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CoroutineManager : MonoBehaviour
{
    public class MyMicroCoroutine
    {
        Dictionary<IEnumerator,Component> _coroutines = new Dictionary<IEnumerator,Component>();

        public void Add(IEnumerator enumerator,Component component)
        {
            _coroutines.Add(enumerator, component);
        }
        public void Run()
        {
            foreach(var co in _coroutines.ToList())
            {
                if(!co.Key.MoveNext())
                {
                    _coroutines.Remove(co.Key);
                }
            }

        }

        public void StopAllCoroutine(Component component)
        {
            foreach(var co in _coroutines.ToList())
            {
                if(co.Value.Equals(component))
                {
                    //co.Key.Reset();
                    _coroutines.Remove(co.Key);
                }
            }
        }
    }

    static CoroutineManager _instance;
    public static CoroutineManager instance
    {
        get
        {
            return _instance;
        }
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

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        StartCoroutine(RunUpdateCoroutine());
    }

    MyMicroCoroutine updateCoroutine = new MyMicroCoroutine();
    public static void StartUpdateCoroutine(IEnumerator coroutine,Component component)
    {
        if (_instance == null)
            return;
        _instance.updateCoroutine.Add(coroutine, component);
    }



    IEnumerator RunUpdateCoroutine()
    {
        while(true)
        {
            updateCoroutine.Run();
            yield return null;

        }
    }

    public static void StopAllUpdateCoroutine(Component component)
    {
        if (_instance == null)
            return;
        _instance.updateCoroutine.StopAllCoroutine(component);
    }

}
