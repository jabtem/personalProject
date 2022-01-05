using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MonsterFOVTest : MonoBehaviour
{
    // Start is called before the first frame updat;


    [Range(0f,180f)]
    public float angle = 45f;
    public float radius = 5f;
    Vector3 rightVector;
    Vector3 leftVector;

    bool isCol = false;
    bool gameStart = false;


    private void Awake()
    {
        gameStart = true;
    }
    private void Update()
    {

        rightVector = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        leftVector = new Vector3(-Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));

        Debug.DrawRay(transform.position, transform.forward * radius, Color.white);
        Debug.DrawRay(transform.position, transform.TransformDirection(rightVector.normalized) * radius, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(leftVector.normalized) * radius, Color.green);

    }

    private void OnDrawGizmos()
    {
        if(gameStart)
        {
            Handles.color = isCol ? Color.red : Color.blue;
            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angle, radius);
            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angle, radius);
        }

    }
}
