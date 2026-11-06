// Inside the Network - integration layer (Editor only)
// Menu: Tools > Inside the Network
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InsideTheNetwork.Integration;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace InsideTheNetwork.IntegrationEditor
{
    [InitializeOnLoad]
    public static class IntegrationTools
    {
        const string BuildFolder = "Build";
        const string ExeName = "InsideTheNetwork.exe";
        const string OpenedKey = "InsideTheNetwork.Integration.OpenedTitleScene";

        // On a fresh clone Unity opens an empty untitled scene; open Padma's title screen instead.
        static IntegrationTools()
        {
            if (SessionState.GetBool(OpenedKey, false)) return;
            SessionState.SetBool(OpenedKey, true);
            EditorApplication.delayCall += () =>
            {
                var active = EditorSceneManager.GetActiveScene();
                string title = FindScene(LevelCatalog.MainMenuScene);
                if (string.IsNullOrEmpty(active.path) && !active.isDirty && title != null)
                    EditorSceneManager.OpenScene(title);
            };
        }

        [MenuItem("Tools/Inside the Network/Open Title Scene", false, 1)]
        public static void OpenTitleScene()
        {
            string title = FindScene(LevelCatalog.MainMenuScene);
            if (title != null && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(title);
        }

        // TitleScene, LevelMenu, then every layer scene found anywhere in Assets, in OSI order 7 -> 1.
        [MenuItem("Tools/Inside the Network/Sync Build Settings", false, 20)]
        public static void SyncBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>();
            var report = new List<string>();
            foreach (string menu in new[] { LevelCatalog.MainMenuScene, LevelCatalog.LevelMenuScene })
            {
                string path = FindScene(menu);
                if (path != null) scenes.Add(new EditorBuildSettingsScene(path, true));
                report.Add(menu + ": " + (path ?? "(missing)"));
            }
            foreach (LayerInfo layer in LevelCatalog.Layers)
            {
                string path = FindScene(layer.Scene);
                if (path != null) scenes.Add(new EditorBuildSettingsScene(path, true));
                report.Add(layer.Number + " " + layer.Name + ": " + (path ?? "(no scene named " + layer.Scene + " yet)"));
            }
            EditorBuildSettings.scenes = scenes.ToArray();
            AssetDatabase.SaveAssets();
            Debug.Log("[Integration] Build Settings updated:\n" + string.Join("\n", report));
        }

        static string FindScene(string sceneName)
        {
            return AssetDatabase.FindAssets(sceneName + " t:Scene")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(p => Path.GetFileNameWithoutExtension(p) == sceneName)
                .OrderBy(p => p.Length)
                .FirstOrDefault();
        }

        [MenuItem("Tools/Inside the Network/Build Windows App", false, 40)]
        public static void BuildWindowsApp()
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64))
            {
                EditorUtility.DisplayDialog("Inside the Network",
                    "Windows Build Support isn't installed for this Unity version.\n\n" +
                    "Unity Hub > Installs > 2022.3.62f3 > Add modules > Windows Build Support (Mono).", "OK");
                return;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            string message;
            bool ok = Build(out message);
            if (ok) EditorUtility.RevealInFinder(Path.Combine(Path.GetDirectoryName(Application.dataPath), BuildFolder, ExeName));
            EditorUtility.DisplayDialog("Inside the Network", message, "OK");
        }

        // Command line: Unity -batchmode -quit -projectPath UnityGame/IntegratedGame
        //               -executeMethod InsideTheNetwork.IntegrationEditor.IntegrationTools.BuildWindowsAppBatch
        public static void BuildWindowsAppBatch()
        {
            string message;
            bool ok = Build(out message);
            Debug.Log("[Integration] " + message);
            if (!ok) EditorApplication.Exit(1);
        }

        static bool Build(out string message)
        {
            SyncBuildSettings();
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string outDir = Path.Combine(projectRoot, BuildFolder);
            var options = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                locationPathName = Path.Combine(outDir, ExeName),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                message = "Build failed: " + report.summary.result + ". See the Console.";
                return false;
            }
            foreach (string dir in Directory.GetDirectories(outDir).Where(d => d.EndsWith("_DoNotShip") || d.EndsWith("_ButDontShipItWithYourGame")))
                Directory.Delete(dir, true);

            message = "Build finished: " + BuildFolder + "/" + ExeName +
                      "\nShip the whole Build folder; the .exe can't run alone.";
            return true;
        }
    }
}
