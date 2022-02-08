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

            ///<summary>
            ///해당 스크립에서 Add한 코루틴만 모두 정지
            /// </summary>
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

    static CoroutineManager _instance;
    public static CoroutineManager instance
    {
        get
        {
            return _instance;
        }
    }
    WaitForSeconds Wfs;
    Dictionary<float, WaitForSeconds> waitDic = new Dictionary<float, WaitForSeconds>();

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
        StartCoroutine(RunWfsCoroutine());
    }

    MicroCoroutine updateCoroutine = new MicroCoroutine();
    MicroCoroutine wfsCoroutine = new MicroCoroutine();
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

    public static void StartWfsCoroutine(IEnumerator coroutine, Component component)
    {
        if (_instance == null)
            return;
        _instance.wfsCoroutine.Add(coroutine, component);
    }



    IEnumerator RunWfsCoroutine()
    {
        while(true)
        {
            wfsCoroutine.Run();
            yield return Wfs;
        }
    }

    public void SetWaitforSeconds(float time)
    {
        if (!waitDic.TryGetValue(time, out Wfs))
        {
            waitDic.Add(time, Wfs = new WaitForSeconds(time));
        }
    }

    public static void StopAllWfsCoroutine(Component component)
    {
        if (_instance == null)
            return;
        _instance.wfsCoroutine.StopAllCoroutine(component);
    }

    public static void StopAllUpdateCoroutine(Component component)
    {
        if (_instance == null)
            return;
        _instance.updateCoroutine.StopAllCoroutine(component);
    }

}
