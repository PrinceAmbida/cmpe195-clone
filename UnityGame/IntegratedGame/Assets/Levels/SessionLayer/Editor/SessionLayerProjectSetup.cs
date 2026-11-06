using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace OSIGame.SessionLayer.EditorTools
{
    /// <summary>
    /// One-time project setup for the Session Layer minigame (Unity 2022.3):
    ///  - imports TextMeshPro's essential resources (fonts/shaders) if they're missing
    ///  - adds the scene to Build Settings and sets a fixed 1200 x 700 window
    ///  - opens the scene
    /// Runs automatically when the project opens. Menu: Tools > Session Layer.
    /// </summary>
    [InitializeOnLoad]
    public static class SessionLayerProjectSetup
    {
        public const string ScenePath = "Assets/Levels/SessionLayer/Scenes/SessionLayer.unity";
        const string BuildFolder = "Game";
        const string ExeName = "SessionControl.exe";
        const string SessionKey = "OSIGame.SessionLayer.SetupRan";

        static bool _waitingForTmp;
        static bool _openSceneWhenDone;

        // INTEGRATION: this used to run automatically on every editor launch and replace
        // Build Settings with only the Session scene. In the combined project, Build Settings
        // belong to Tools > Inside the Network, so the automatic run is disabled. The menu
        // items below still work.
        static SessionLayerProjectSetup()
        {
        }

        [MenuItem("Tools/Session Layer/Run Project Setup", false, 1)]
        static void RunSetupMenu()
        {
            RunSetup(true);
        }

        [MenuItem("Tools/Session Layer/Open Scene", false, 2)]
        static void OpenSceneMenu()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(ScenePath);
        }

        static void RunSetup(bool openScene)
        {
            _openSceneWhenDone = openScene;
            if (!TmpReady())
            {
                ImportTmp();
                return;
            }
            FinishSetup();
        }

        static void FinishSetup()
        {
            // Build Settings: just this scene
            // INTEGRATION: add the scene if it is missing; never remove the other levels.
            var scenes = EditorBuildSettings.scenes;
            if (!scenes.Any(s => s.path == ScenePath))
                EditorBuildSettings.scenes = scenes.Concat(new[] { new EditorBuildSettingsScene(ScenePath, true) }).ToArray();

            ApplyPlayerSettings();
            AssetDatabase.SaveAssets();

            // Open the scene if nothing meaningful is open yet (fresh project shows an untitled scene)
            if (_openSceneWhenDone)
            {
                var active = EditorSceneManager.GetActiveScene();
                bool emptyUntitled = string.IsNullOrEmpty(active.path) && !active.isDirty;
                if (emptyUntitled || active.path == ScenePath)
                {
                    if (active.path != ScenePath) EditorSceneManager.OpenScene(ScenePath);
                }
            }
        }

        static void ApplyPlayerSettings()
        {
            // INTEGRATION: product name is "Inside the Network" for the combined app.
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 1200;
            PlayerSettings.defaultScreenHeight = 700;
            PlayerSettings.resizableWindow = false;
            PlayerSettings.defaultWebScreenWidth = 1200;
            PlayerSettings.defaultWebScreenHeight = 700;
            PlayerSettings.runInBackground = true;
            // The scene was designed in a Linear color space project; keep colors identical.
            if (PlayerSettings.colorSpace != ColorSpace.Linear)
                PlayerSettings.colorSpace = ColorSpace.Linear;
        }

        // ---------------- TextMeshPro essentials ----------------

        static bool TmpReady()
        {
            return AssetDatabase.FindAssets("t:TMP_Settings").Length > 0;
        }

        static void ImportTmp()
        {
            string package = FindTmpEssentialsPackage();
            if (package == null)
            {
                EditorUtility.DisplayDialog("Session Layer",
                    "Please import TextMeshPro's essential resources:\n\n" +
                    "Window > TextMeshPro > Import TMP Essential Resources > Import.\n\n" +
                    "Then use Tools > Session Layer > Run Project Setup.", "OK");
                return;
            }
            if (!_waitingForTmp)
            {
                _waitingForTmp = true;
                AssetDatabase.importPackageCompleted += OnTmpImported;
                AssetDatabase.importPackageFailed += OnTmpFailed;
                AssetDatabase.importPackageCancelled += OnTmpCancelled;
            }
            Debug.Log("[Session Layer] Importing TMP Essential Resources...");
            AssetDatabase.ImportPackage(package, false);
        }

        static void StopWaiting()
        {
            _waitingForTmp = false;
            AssetDatabase.importPackageCompleted -= OnTmpImported;
            AssetDatabase.importPackageFailed -= OnTmpFailed;
            AssetDatabase.importPackageCancelled -= OnTmpCancelled;
        }

        static void OnTmpImported(string packageName)
        {
            StopWaiting();
            AssetDatabase.Refresh();
            EditorApplication.delayCall += FinishSetup;
        }

        static void OnTmpFailed(string packageName, string error)
        {
            StopWaiting();
            Debug.LogError("[Session Layer] TMP import failed: " + error);
        }

        static void OnTmpCancelled(string packageName)
        {
            StopWaiting();
        }

        static string FindTmpEssentialsPackage()
        {
            string[] packageNames = { "com.unity.textmeshpro", "com.unity.ugui" };
            foreach (var name in packageNames)
            {
                var info = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/" + name);
                string root = info != null ? info.resolvedPath : Path.GetFullPath("Packages/" + name);
                string path = Path.Combine(root, "Package Resources", "TMP Essential Resources.unitypackage");
                if (File.Exists(path)) return path;
            }
            return null;
        }

        // ---------------- Windows build ----------------

        [MenuItem("Tools/Session Layer/Build Windows Game", false, 20)]
        static void BuildWindowsGame()
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64))
            {
                EditorUtility.DisplayDialog("Session Layer",
                    "Windows Build Support isn't installed for this Unity version.\n\n" +
                    "Unity Hub > Installs > (this version) > Add modules > Windows Build Support.", "OK");
                return;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            FinishSetupWithoutOpening();

            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string outDir = Path.Combine(projectRoot, BuildFolder);
            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = Path.Combine(outDir, ExeName),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                foreach (var dir in Directory.GetDirectories(outDir).Where(d =>
                             d.EndsWith("_DoNotShip") || d.EndsWith("_ButDontShipItWithYourGame")))
                    Directory.Delete(dir, true);

                EditorUtility.RevealInFinder(Path.Combine(outDir, ExeName));
                EditorUtility.DisplayDialog("Session Layer",
                    "Build finished: " + BuildFolder + "/" + ExeName +
                    "\n\nCommit the whole Game folder (the .exe alone won't run).", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Session Layer",
                    "Build failed: " + report.summary.result + ". See the Console for details.", "OK");
            }
        }

        static void FinishSetupWithoutOpening()
        {
            _openSceneWhenDone = false;
            FinishSetup();
        }
    }
}
