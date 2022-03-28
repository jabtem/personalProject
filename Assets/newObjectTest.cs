using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newObjectTest : MonoBehaviour
{

    private void Awake()
    {
        testfunc();
    }
    void testfunc()
    {
        GameObject go = new GameObject($"{gameObject.name}");
        go.transform.SetParent(this.transform);
    }
}
