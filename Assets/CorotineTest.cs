using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorotineTest : MonoBehaviour
{

    void Update()
    {
        for (int i = 0; i < 1000; i++)
        {
            //CoroutineManager.StartWfsCoroutine(TestCoroutine(), this);
            StartCoroutine(TestCoroutine());
        }
    }

    WaitForSeconds wait = new WaitForSeconds(2f);
    IEnumerator TestCoroutine()
    {

        Debug.Log(2);
        //CoroutineManager.instance.SetWaitforSeconds(2f);
        yield return wait;
    }
}
