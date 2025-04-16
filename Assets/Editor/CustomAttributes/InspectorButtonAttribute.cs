using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class InspectorButtonAttribute : PropertyAttribute
{
    public string ButtonLabel;

    public InspectorButtonAttribute(string label = null)
    {
        ButtonLabel = label;
    }
}
