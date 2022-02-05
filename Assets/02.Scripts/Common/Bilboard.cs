using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour
{
    Transform myTr;

    Transform mainCameraTr;

    Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        myTr = GetComponent<Transform>();

        mainCameraTr = Camera.main.transform;

        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        //항상 카메라방향을 바라보게함
        //myTr.LookAt(mainCameraTr);

        //항상 카메라방향을보며 정면방향으로 나타나도록
        //x축은 카메라 y각도로 고정
        //y축은 몸체가 회전한만큼을 다시 빼서 정면방향으로 돌려줌(localRotation의경우)
        myTr.rotation = Quaternion.Euler(-45f, -180f + mainCameraTr.rotation.eulerAngles.y, 0f);
        //myTr.localRotation = Quaternion.Euler(-mainCameraTr.rotation.eulerAngles.y, -180f + mainCameraTr.rotation.eulerAngles.y -parent.rotation.eulerAngles.y, 0f);
    }
}
