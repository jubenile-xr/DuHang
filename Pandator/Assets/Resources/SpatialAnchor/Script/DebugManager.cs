using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private static bool debugMode = true;

    public static bool GetDebugMode() => debugMode;

    public static void SetDebugMode(bool mode)
    {
        Debug.Log("Debug mode: " + mode);
        debugMode = mode;
    }
}
