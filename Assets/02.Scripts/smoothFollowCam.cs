using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smoothFollowCam : MonoBehaviour
{
    // Start is called before the first frame update

    //따라다닐 대상
    public Transform target;
    public float distance = 10.0f;
    RadarMap map;
    //터치 불가능 영역
    List<Rect> dontTouchArea = new List<Rect>();

    //카메라 회전속도
    public float rotateSpeed = 5f;


    public float height = 5.0f;

    public float heightDamping = 2.0f;

    public float rotationDaping = 3.0f;


    private void Awake()
    {
        map = GameObject.FindGameObjectWithTag("Minimap").GetComponent<RadarMap>();
        dontTouchArea.Add(new Rect(0, 0, Screen.width * 0.5f, Screen.height));
    }

    private void Update()
    {
        CamRotation();
    }

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


    void CamRotation()
    {

        //회전할 카메라앵글
        Vector3 rot = transform.rotation.eulerAngles;
        //기존 카메라앵글
        Vector3 curRot = transform.rotation.eulerAngles;
        Vector2 tochPos = Vector2.zero;

        float x = Input.GetAxis("Mouse X");



        //모바일 카메라조작
#if (UNITY_ANDROID || UNITY_IPHONE) 

        if (Input.touchCount>0)
        {

            for(int i = 0; i< Input.touchCount; i++)
            {
                tochPos = Input.GetTouch(i).position;

                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {

                    if (!dontTouchArea[0].Contains(tochPos))
                    {
                        x = Input.touches[i].deltaPosition.x;
                    }
                }
                if (Input.GetTouch(i).phase == TouchPhase.Moved)
                {
                    if (!dontTouchArea[0].Contains(tochPos))
                    {
                        rot.y += x * rotateSpeed;
                        Quaternion q = Quaternion.Euler(rot);
                        transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f);
                        map.RoatateMapDot(rot.y - curRot.y);
                    }

                }
            }


        }

#endif

#if UNITY_EDITOR
        DebugDrawRect(dontTouchArea[0], Color.red);
        //PC및 에디터 카메라 조작
        if (Input.GetMouseButton(2))
        {

            rot.y += x * rotateSpeed;

            Quaternion q = Quaternion.Euler(rot);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f);
            map.RoatateMapDot(rot.y - curRot.y);
        }


#endif
    }
    void DebugDrawRect(Rect rect, Color color)
    {
        //아래
        Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y), color);
        //왼쪽
        Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height), color);
        //오른쪽
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y), new Vector3(rect.x + rect.width, rect.y + rect.height), color);
        //위
        Debug.DrawLine(new Vector3(rect.x, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y + rect.height), color);
    }
}
