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
            GameManager.instance.postProcessingManager = this;
        }
    }

}
