using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CoroutineManager : MonoBehaviour
{
    public class MicroCoroutine
    {
        HashSet<IEnumerator> _coroutines = new HashSet<IEnumerator>();

        public void Add(IEnumerator enumerator)
        {
            _coroutines.Add(enumerator);
        }
        public void Run()
        {
            foreach(var co in _coroutines.ToList())
            {
                if(!co.MoveNext())
                {
                    _coroutines.Remove(co);
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

    MicroCoroutine updateCoroutine = new MicroCoroutine();

    public static void StartUpdateCoroutine(IEnumerator coroutine)
    {
        if (_instance == null)
            return;
        _instance.updateCoroutine.Add(coroutine);
    }

    IEnumerator RunUpdateCoroutine()
    {
        while(true)
        {
            yield return null;
            updateCoroutine.Run();
        }
    }
}
