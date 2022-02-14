using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    public class MicroCoroutine
    {
        //Original
        Dictionary<IEnumerator,Component> _coroutines = new Dictionary<IEnumerator,Component>();
        //Copy
        Dictionary<IEnumerator, Component> _cash = new Dictionary<IEnumerator, Component>();
        //DeleteData Collect
        HashSet<IEnumerator> removeKey = new HashSet<IEnumerator>();
        HashSet<IEnumerator> stopCoroutineKey = new HashSet<IEnumerator>();
        Component removeComponent;
        public void Add(IEnumerator enumerator, Component component)
        {
            _coroutines.Add(enumerator, component);
        }

        public void Show()
        {
            int index = 0;
            foreach(var a in _coroutines)
            {
                Debug.Log($"{index++}{a.Key}");
            }
            foreach (var a in _cash)
            {
                Debug.Log($"{index}{a.Key}");
            }
        }
        public void Remove()
        {
            if(stopCoroutineKey.Count>0)
            {
                foreach (IEnumerator remove in stopCoroutineKey)
                {
                    if (_coroutines.ContainsKey(remove))
                        _coroutines.Remove(remove);
                }
                stopCoroutineKey.Clear();
            }
        }

        public void Run()
        {

            //Dictionary DeepCopy
            foreach(KeyValuePair<IEnumerator,Component> coroutine in _coroutines)
            {
                _cash.Add(coroutine.Key, coroutine.Value);
            }

            foreach (KeyValuePair<IEnumerator,Component> co in _cash)
            {
                if (!co.Key.MoveNext())
                {
                    removeKey.Add(co.Key);
                }
                else
                {
                    continue;
                }
            }

            _cash.Clear();

            foreach(IEnumerator remove in removeKey)
            {
                if(_coroutines.TryGetValue(remove,out removeComponent))
                {
                    _coroutines.Remove(remove);
                }

            }
            removeKey.Clear();
            //stopCoroutineKey.Clear();


        }

        public void StopAllCoroutine(Component component)
        {

            //stopCoroutineKey.Clear();
            foreach (KeyValuePair<IEnumerator, Component> co in _coroutines)
            {
                if (co.Value.Equals(component))
                {
                    stopCoroutineKey.Add(co.Key);
                }
            }

            //int i = 0;
            //while (i < _coroutineList.Count)
            //{
            //    if (_coroutineList[i].Value.Equals(component))
            //    {
            //        _coroutineList.RemoveAt(i);
            //        continue;
            //    }
            //    i++;
            //}


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

    [ContextMenu("test")]
    public void Test()
    {
        instance.updateCoroutine.Show();
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
    public static void StarTestCoroutine(IEnumerator routine)
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
            updateCoroutine.Remove();
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




   







    //구현하려했으나 실패 더손대기엔 시간이너무많이걸릴것같으니 나중을기약
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
