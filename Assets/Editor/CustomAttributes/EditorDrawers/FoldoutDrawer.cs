#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(FoldoutAttribute))]
public class FoldoutDrawer : PropertyDrawer
{
    private static Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        FoldoutAttribute foldout = (FoldoutAttribute)attribute;

        if (!foldoutStates.ContainsKey(foldout.GroupName))
            foldoutStates[foldout.GroupName] = true;

        foldoutStates[foldout.GroupName] = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutStates[foldout.GroupName], foldout.GroupName);
        if (foldoutStates[foldout.GroupName])
        {
            EditorGUILayout.PropertyField(property, label, true);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        FoldoutAttribute foldout = (FoldoutAttribute)attribute;
        if (!foldoutStates.TryGetValue(foldout.GroupName, out bool expanded) || expanded)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        return -EditorGUIUtility.standardVerticalSpacing;
    }
}
#endif
