using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerHealthCameraView : MonoBehaviour
{
    // test
    public PostProcessVolume volume;

    private void Start()
    {
        if (volume.sharedProfile.TryGetSettings<Vignette>(out Vignette vignette))
        {
            vignette.intensity.value = 10f;
        }
    }
}
