using UnityEngine;

public class MinMaxSliderAttribute : PropertyAttribute
{
    public float Min;
    public float Max;

    public MinMaxSliderAttribute(float min, float max)
    {
        this.Min = min;
        this.Max = max;
    }
}
