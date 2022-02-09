using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CoroutineManager : MonoBehaviour
{
    public class MicroCoroutine
    {
        //Dictionary<IEnumerator,Component> _coroutines = new Dictionary<IEnumerator,Component>();
        List<KeyValuePair<IEnumerator, Component>> _coroutineList = new List<KeyValuePair<IEnumerator, Component>>();
        public void Add(IEnumerator enumerator, Component component)
        {
            //_coroutines.Add(enumerator, component);
            _coroutineList.Add(new KeyValuePair<IEnumerator, Component>(enumerator, component));
        }
        public void Run()
        {

            //딕셔너리 사용시
            //foreach (var co in _coroutines.ToList())
            //{
            //    if (!co.Key.MoveNext())
            //    {
            //        _coroutines.Remove(co.Key);
            //    }
            //}

            //리스트사용시
            int i = 0;
            while (i < _coroutineList.Count)
            {
                if (!_coroutineList[i].Key.MoveNext())
                {
                    _coroutineList.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }

        public void StopAllCoroutine(Component component)
        {


            //foreach (var co in _coroutines.ToList())
            //{
            //    if (co.Value.Equals(component))
            //    {
            //        //co.Key.Reset();
            //        _coroutines.Remove(co.Key);
            //    }
            //}
        }
    }

    public class MicroCoroutine2
    {
        List<IEnumerator> _coroutines = new List<IEnumerator>();

        public void AddCoroutine(IEnumerator enumerator)
        {
            _coroutines.Add(enumerator);
        }

        public void Run()
        {
            int i = 0;
            while (i < _coroutines.Count)
            {
                if (!_coroutines[i].MoveNext())
                {
                    _coroutines.RemoveAt(i);
                    continue;
                }
                i++;
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
        //StartCoroutine(RunUpdateCoroutine());
        StartCoroutine(RunTestCoroutine());
    }

    MicroCoroutine updateCoroutine = new MicroCoroutine();
    MicroCoroutine2 test = new MicroCoroutine2();
    public static void StartUpdateCoroutine(IEnumerator routine)
    {
        if (_instance == null)
            return;
        _instance.test.AddCoroutine(routine);
    }

    public static void StartUpdateCoroutine(IEnumerator coroutine,Component component)
    {
        if (_instance == null)
            return;
        _instance.updateCoroutine.Add(coroutine, component);
    }
    IEnumerator RunUpdateCoroutine()
    {
        while (true)
        {
            updateCoroutine.Run();
            yield return null;

        }
    }
    IEnumerator RunTestCoroutine()
    {
        while (true)
        {
            test.Run();
            yield return null;

        }
    }
    public static void StopAllUpdateCoroutine(Component component)
    {
        if (_instance == null)
            return;
        _instance.updateCoroutine.StopAllCoroutine(component);
    }


    //public static void StartWfsCoroutine(IEnumerator coroutine, Component component)
    //{
    //    if (_instance == null)
    //        return;
    //    _instance.wfsCoroutine.Add(coroutine, component);
    //}



    //IEnumerator RunWfsCoroutine()
    //{
    //    while(true)
    //    {
    //        wfsCoroutine.Run();
    //        yield return Wfs;
    //    }
    //}


    //public static void StopAllWfsCoroutine(Component component)
    //{
    //    if (_instance == null)
    //        return;
    //    _instance.wfsCoroutine.StopAllCoroutine(component);
    //}



}
