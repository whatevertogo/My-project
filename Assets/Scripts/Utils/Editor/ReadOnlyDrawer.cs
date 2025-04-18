using UnityEditor;
using UnityEngine;

/// <summary>
/// 自定义 PropertyDrawer，用于在 Inspector 中以只读方式显示带有 ReadOnlyAttribute 的字段
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 禁用 GUI，使字段不可编辑
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}
