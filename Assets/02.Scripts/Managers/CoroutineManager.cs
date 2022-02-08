using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CoroutineManager : MonoBehaviour
{
    public class MicroCoroutine
    {
        Dictionary<IEnumerator,Component> _coroutines = new Dictionary<IEnumerator,Component>();

        public void Add(IEnumerator enumerator,Component component)
        {
            _coroutines.Add(enumerator, component);
        }
        public void Run()
        {
            //이부분에서 리스트로 변환하면서 가비지생성 원인파악필요
            var remove = _coroutines.Where((data) => !data.Key.MoveNext()).ToList();

            foreach (var removeData in remove)
            {
                _coroutines.Remove(removeData.Key);
            }


            //foreach (var co in _coroutines.ToList())
            //{
            //    if(!co.Key.MoveNext())
            //    {
            //        _coroutines.Remove(co.Key);
            //    }
            //}

        }

        public void StopAllCoroutine(Component component)
        {

            //var remove = _coroutines.Where((key) => key.Value.Equals(component)).ToList();

            //foreach(var removeData in remove)
            //{
            //    _coroutines.Remove(removeData.Key);
            //}

            foreach (var co in _coroutines.ToList())
            {
                if (co.Value.Equals(component))
                {
                    //co.Key.Reset();
                    _coroutines.Remove(co.Key);
                }
            }
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
        StartCoroutine(RunUpdateCoroutine());
        //StartCoroutine(RunTestCoroutine());
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
