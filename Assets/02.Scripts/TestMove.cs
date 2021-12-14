using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestMove : MonoBehaviour
{
    float t;
    Vector3 now;

    void Awake()
    {
        now = this.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        transform.position = new Vector3(now.x, now.y, now.z + Mathf.Sin(t)*10);

    }
}
