using UnityEngine;
using UnityEngine.UI;

public class SFXSlider : MonoBehaviour
{
    private Slider sfxSlider;


    private void Awake()
    {
        sfxSlider = GetComponent<Slider>();
        sfxSlider.value = AudioManager.Instance.SoundVolume;
    }

    private void Update()
    {
        AudioManager.Instance.SoundVolume = sfxSlider.value;
    }

}
