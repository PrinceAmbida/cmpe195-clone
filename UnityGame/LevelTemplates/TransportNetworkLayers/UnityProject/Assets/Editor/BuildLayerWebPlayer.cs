using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildLayerWebPlayer
{
    [MenuItem("Tools/Transport Network Layers/Build Web Player")]
    public static void Build()
    {
        string output = Path.GetFullPath("Exports/WebGL");
        string[] arguments = Environment.GetCommandLineArgs();
        for (int i = 0; i < arguments.Length - 1; i++)
        {
            if (arguments[i] == "-buildOutput")
                output = Path.GetFullPath(arguments[i + 1]);
        }

        PlayerSettings.defaultWebScreenWidth = 1200;
        PlayerSettings.defaultWebScreenHeight = 700;
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.WebGL.threadsSupport = false;
        PlayerSettings.WebGL.initialMemorySize = 128;
        PlayerSettings.WebGL.template = "APPLICATION:Default";
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.WebGL, ScriptingImplementation.IL2CPP);

        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[]
            {
                "Assets/Scenes/TransportLayer.unity",
                "Assets/Scenes/NetworkLayer.unity"
            },
            locationPathName = output,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        });

        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Web build failed: " + report.summary.result);

        Debug.Log("WEB_BUILD_SUCCEEDED: " + output + " (" + report.summary.totalSize + " bytes)");
    }
}
