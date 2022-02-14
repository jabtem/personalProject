using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effecttest : MonoBehaviour
{
    float time;
    void Update()
    {
        time += Time.deltaTime;

        if(time > 2f)
        {
            time = 0;
            GameManager.instance.Effect.PushEffect(0, this.gameObject);
        }
    }


}
