using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerHealthCameraView : MonoBehaviour
{
    public PostProcessVolume volume;
    public Player player;

    private Vignette vignette;
    private AmbientOcclusion ambientOcclusion;

    private void Start()
    {
        vignette = volume.sharedProfile.GetSetting<Vignette>();
        ambientOcclusion = volume.sharedProfile.GetSetting<AmbientOcclusion>();

        UpdateVolume();
        player.onTakeDamage += UpdateVolume;
        player.onDie += Unsubscribe;
    }

    private void Unsubscribe()
    {
        player.onTakeDamage -= UpdateVolume;
        player.onDie -= Unsubscribe;
    }

    public void UpdateVolume()
    {
        if (vignette != null)
        {
            vignette.intensity.value = 1f - ((float)player.currentHp / player.maxHp);
        }
        
        if (ambientOcclusion != null)
        {
            ambientOcclusion.intensity.value = 4f - ((float)player.currentHp / player.maxHp * 4);
        }
    }
}
