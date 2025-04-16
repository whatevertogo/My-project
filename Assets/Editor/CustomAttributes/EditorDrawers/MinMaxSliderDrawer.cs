#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
public class MinMaxSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MinMaxSliderAttribute slider = (MinMaxSliderAttribute)attribute;

        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            Vector2 val = property.vector2Value;
            EditorGUI.MinMaxSlider(position, label, ref val.x, ref val.y, slider.Min, slider.Max);
            property.vector2Value = val;
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use only with Vector2.");
        }
    }
}
#endif
