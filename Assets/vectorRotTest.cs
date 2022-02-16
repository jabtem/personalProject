using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vectorRotTest : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(-180f, 180f)]
    public float angle = 45f;

    Vector3 test;
    Vector3 test2;
    Vector3 test3;
    // Update is called once per frame
    void Update()
    {
        Quaternion quaternion = Quaternion.Euler(0f, angle, 0f);
        test = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward*10f;
        test2 = Quaternion.AngleAxis(angle, Vector3.right) * transform.forward*10f;
        test3 = Quaternion.AngleAxis(angle, Vector3.forward) * transform.forward*10f;

        Debug.DrawRay(transform.position, test, Color.red);
        Debug.DrawRay(transform.position, test2, Color.blue);
        Debug.DrawRay(transform.position, test3, Color.yellow);
    }
}
