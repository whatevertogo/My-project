#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 保存之前的 GUI 启用状态
        bool previousGUIState = GUI.enabled;

        // 禁用 GUI
        GUI.enabled = false;

        // 绘制属性字段
        EditorGUI.PropertyField(position, property, label, true);

        // 恢复之前的 GUI 启用状态
        GUI.enabled = previousGUIState;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 返回属性的默认高度
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif