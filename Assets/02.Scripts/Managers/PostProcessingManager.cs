using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    PostProcessVolume pVolume;
    Vignette vignette;
    bool Trigger;


    private void Awake()
    {
        pVolume = GetComponent<PostProcessVolume>();
        pVolume.profile.TryGetSettings<Vignette>(out vignette);
        if(GameManager.instance != null)
        {
            GameManager.instance.PostProcessing = this;
        }
    }

    IEnumerator TimeSlow(float intensity)
    {

        if (intensity > 0)
        {
            vignette.active = true;
            while (vignette.intensity.value < intensity)
            {
                if (vignette.intensity.value >= intensity)
                    yield break;
                vignette.intensity.value += 0.01f;
                yield return null;
            }
        }
        if(intensity == 0)
        {
            while(vignette.intensity.value >0)
            {
                if (vignette.intensity.value <= 0)
                {
                    vignette.active = false;
                    yield break;
                }
                vignette.intensity.value -= 0.01f;
                yield return null;
            }
        }
        
    }

    public void TimeSlowEffect(float intensity)
    {
        StartCoroutine(TimeSlow(intensity));
    }

}
