// Inside the Network - integration layer
// Every scene change made by the integration goes through here, so each level always
// starts clean (time running, cursor free) no matter how the previous level ended.
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InsideTheNetwork.Integration
{
    public static class LevelLoader
    {
        public static bool IsSceneInBuild(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                if (Path.GetFileNameWithoutExtension(path) == sceneName) return true;
            }
            return false;
        }

        public static bool IsPlayable(LayerInfo layer)
        {
            return IsSceneInBuild(layer.Scene);
        }

        public static LayerInfo CurrentLayer
        {
            get { return LevelCatalog.FindByScene(SceneManager.GetActiveScene().name); }
        }

        public static void LoadScene(string sceneName)
        {
            // Transport/Network pause with timeScale 0 and both FPS levels lock the cursor.
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(sceneName);
        }

        public static void MainMenu() { LoadScene(LevelCatalog.MainMenuScene); }

        public static void LevelMenu() { LoadScene(LevelCatalog.LevelMenuScene); }

        public static void Play(LayerInfo layer)
        {
            if (IsPlayable(layer)) LoadScene(layer.Scene);
            else Debug.Log("[Integration] " + layer.Name + " layer is not available yet.");
        }

        public static void RestartCurrent()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        // Next layer down the stack that is in the build; back to the level menu after the last one.
        public static void NextAfterCurrent()
        {
            LayerInfo current = CurrentLayer;
            int start = current == null ? 0 : LevelCatalog.IndexOf(current) + 1;
            for (int i = start; i < LevelCatalog.Layers.Length; i++)
            {
                if (IsPlayable(LevelCatalog.Layers[i]))
                {
                    LoadScene(LevelCatalog.Layers[i].Scene);
                    return;
                }
            }
            LevelMenu();
        }
    }
}
