using UnityEngine;
using UnityEngine.UI;

public class SFXSlider : MonoBehaviour
{
    private Slider sfxSlider;


    private void Awake()
    {
        sfxSlider = GetComponent<Slider>();
        sfxSlider.value = AudioManager.Instance.AllVolume;
    }

    private void Update()
    {
        AudioManager.Instance.AllVolume = sfxSlider.value;
    }

}
