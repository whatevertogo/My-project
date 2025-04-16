using UnityEngine;

public class ProgressBarAttribute : PropertyAttribute
{
    public float Min;
    public float Max;

    public ProgressBarAttribute(float min, float max)
    {
        Min = min;
        Max = max;
    }
}
