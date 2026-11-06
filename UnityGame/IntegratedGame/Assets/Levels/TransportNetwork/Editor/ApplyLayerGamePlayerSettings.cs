using UnityEditor;
using UnityEngine;

public static class ApplyLayerGamePlayerSettings
{
    [MenuItem("Tools/Transport Network Layers/Set Game Size to 1200 x 700")]
    private static void Apply()
    {
        PlayerSettings.defaultScreenWidth = 1200;
        PlayerSettings.defaultScreenHeight = 700;
        PlayerSettings.defaultWebScreenWidth = 1200;
        PlayerSettings.defaultWebScreenHeight = 700;
        PlayerSettings.defaultIsNativeResolution = false;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;

        EditorUtility.DisplayDialog(
            "Transport Network Layers",
            "Default game size set to 1200 x 700 for Web and desktop builds.",
            "OK");
    }
}
