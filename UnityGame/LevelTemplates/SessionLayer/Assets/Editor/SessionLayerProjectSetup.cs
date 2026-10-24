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
        public const string ScenePath = "Assets/Scenes/SessionLayer.unity";
        const string BuildFolder = "Game";
        const string ExeName = "SessionControl.exe";
        const string SessionKey = "OSIGame.SessionLayer.SetupRan";

        static bool _waitingForTmp;
        static bool _openSceneWhenDone;

        static SessionLayerProjectSetup()
        {
            if (SessionState.GetBool(SessionKey, false)) return;
            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += () => RunSetup(true);
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
            var scenes = EditorBuildSettings.scenes;
            if (scenes.Length != 1 || scenes[0].path != ScenePath)
                EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

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
            PlayerSettings.productName = "Session Control";
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
