using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smoothFollowCam : MonoBehaviour
{
    // Start is called before the first frame update

    //따라다닐 대상
    public Transform target;
    public float distance = 10.0f;

    public float height = 5.0f;

    public float heightDamping = 2.0f;

    public float rotationDaping = 3.0f;


    // 한 프레임에 모든 Update가 실행된 후 호출되는 함수로
    // 주로 카메라의 이동이나 Update와 따로 실행되야 할 로직에 사용
    void LateUpdate()
    {

        if (!target)
            return;


        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;

        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDaping * Time.deltaTime);

        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        Vector3 tempDis = target.position;
        tempDis -= currentRotation * Vector3.forward * distance;

        tempDis.y = currentHeight;

        transform.position = tempDis;

        transform.LookAt(target);

    }

}
