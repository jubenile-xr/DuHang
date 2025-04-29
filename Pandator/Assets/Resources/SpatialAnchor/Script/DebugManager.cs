using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private static bool debugMode = false;

    public static bool GetDebugMode() => debugMode;

    public static void SetDebugMode(bool mode)
    {
        debugMode = mode;
    }
}
