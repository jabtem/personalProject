using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorotineTest : MonoBehaviour
{

    void Update()
    {
        for (int i = 0; i < 10000; i++)
        {
            CoroutineManager.StartUpdateCoroutine(TestCoroutine());
            //StartCoroutine(TestCoroutine());
        }
    }


    WaitForSeconds wait = new WaitForSeconds(2f);
    WaitForSeconds wait2 = new WaitForSeconds(4f);
    IEnumerator TestCoroutine()
    {
        //CoroutineManager.instance.SetWaitforSeconds(2f);
        yield return null;

    }

    IEnumerator TestCoroutine2()
    {
        while(true)
        {
            Debug.Log(2);
            //CoroutineManager.instance.SetWaitforSeconds(2f);
            yield return wait;
        }

    }
    IEnumerator TestCoroutine3()
    {
        while (true)
        {
            Debug.Log(4);
            //CoroutineManager.instance.SetWaitforSeconds(4f);
            yield return wait2;
        }

    }
}
