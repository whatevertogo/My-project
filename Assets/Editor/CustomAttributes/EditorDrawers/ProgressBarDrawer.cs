#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ProgressBarAttribute))]
public class ProgressBarDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ProgressBarAttribute progress = (ProgressBarAttribute)attribute;
        float value = property.floatValue;
        float percentage = Mathf.InverseLerp(progress.Min, progress.Max, value);
        EditorGUI.ProgressBar(position, percentage, $"{label.text}: {value:F1}/{progress.Max}");
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
#endif
