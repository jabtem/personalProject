using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingTest : MonoBehaviour
{
    PostProcessVolume pVolume;
    Vignette vignette;
    bool test;
    private void Awake()
    {
        pVolume = GetComponent<PostProcessVolume>();
        pVolume.profile.TryGetSettings<Vignette>(out vignette);
    }

    private void Start()
    {
        vignette.intensity.value = 0.5f;
    }

    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            test = true;

        }

        if(test)
        {
            if(vignette.intensity.value <= 0.9)
                vignette.intensity.value += Time.deltaTime;
        }
    }
}
