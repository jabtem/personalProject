using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCheck : MonoBehaviour
{
    float frame;
    float timeElap;
    float frameTime;

    public float FPS;
    public float msFrameTime;

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR

        ++frame;
        timeElap += Time.unscaledDeltaTime;
        if(timeElap > 1f)
        {
            frameTime = timeElap / frame;
            timeElap -= 1f;
            FPS = frame;
            msFrameTime = frameTime * 1000f;
            frame = 0;
        }

#endif

    }
}
