using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    PostProcessVolume pVolume;
    Vignette vignette;
    bool test;
    private void Awake()
    {
        pVolume = GetComponent<PostProcessVolume>();
        pVolume.profile.TryGetSettings<Vignette>(out vignette);
    }

    void Start()
    {
        vignette.intensity.value = 0.5f;
    }

    void FixedUpdate()
    {

    }
}
