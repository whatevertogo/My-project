using System;
using UnityEngine;

namespace CDTU.Utils
{
    /// <summary>
    /// 一个简易的日志记录器，用于在Unity编辑器和开发版本中输出日志，保证不会再发布版本中输出
    /// </summary>
    public static class ULogger
    {
        [System.Diagnostics.Conditional("UNITY_EDITOR")]//
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string message)
        {
            Debug.Log(message);
        }


        [System.Diagnostics.Conditional("UNITY_EDITOR")]//
        public static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]//
        public static void LogError(string message)
        {
            Debug.LogError(message);
        }
    }
}