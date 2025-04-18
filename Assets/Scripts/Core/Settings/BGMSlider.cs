using UnityEngine;
using UnityEngine.UI;

public class BGMSlider : MonoBehaviour
{

    private Slider bgmSlider;


    private void Awake()
    {
        bgmSlider = GetComponent<Slider>();
        bgmSlider.value = AudioManager.Instance.MusicVolume;
    }

    private void Update()
    {
        AudioManager.Instance.MusicVolume = bgmSlider.value;

    }
}
