#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true)]
public class InspectorButtonDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var mono = target as MonoBehaviour;
        var methods = mono.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            var attrs = method.GetCustomAttributes(typeof(InspectorButtonAttribute), true);
            foreach (InspectorButtonAttribute attr in attrs)
            {
                string label = string.IsNullOrEmpty(attr.ButtonLabel) ? method.Name : attr.ButtonLabel;
                if (GUILayout.Button(label))
                {
                    method.Invoke(mono, null);
                }
            }
        }
    }
}
#endif
