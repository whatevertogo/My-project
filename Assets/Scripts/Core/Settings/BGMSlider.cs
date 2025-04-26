using UnityEngine;
using UnityEngine.UI;

public class BGMSlider : MonoBehaviour
{

    private Slider bgmSlider;


    private void Awake()
    {
        bgmSlider = GetComponent<Slider>();
        bgmSlider.value = AudioManager.Instance.BGMVolume;
    }

    private void Update()
    {
        AudioManager.Instance.BGMVolume = bgmSlider.value;
    }
}
