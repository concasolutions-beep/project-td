using System.Diagnostics;

public static class Utils
{
    [Conditional("UNITY_EDITOR")]
    public static void DebugLog(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void WarningLog(string message)
    {
        UnityEngine.Debug.LogWarning(message);
    }
}