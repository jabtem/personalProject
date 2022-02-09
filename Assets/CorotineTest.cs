using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorotineTest : MonoBehaviour
{

    void Update()
    {
        for (int i = 0; i < 10000; i++)
        {
            CoroutineManager.StartUpdateCoroutine(TestCoroutine(),this);
            //StartCoroutine(TestCoroutine());
        }
    }


    WaitForSeconds wait = new WaitForSeconds(2f);
    WaitForSeconds wait2 = new WaitForSeconds(4f);
    IEnumerator TestCoroutine()
    {
        yield return null;

    }

    IEnumerator TestCoroutine2()
    {
        while(true)
        {
            Debug.Log(2);
            yield return wait;
        }

    }
    IEnumerator TestCoroutine3()
    {
        while (true)
        {
            Debug.Log(4);
            yield return wait2;
        }

    }
}
