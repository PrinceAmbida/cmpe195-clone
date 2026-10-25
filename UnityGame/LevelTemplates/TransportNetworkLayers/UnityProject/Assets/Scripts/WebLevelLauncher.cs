using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace InsideTheNetwork
{
    // The browser page uses SendMessage to select either existing level.
    public sealed class WebLevelLauncher : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateLauncher()
        {
            GameObject launcher = new GameObject("WebLevelLauncher");
            DontDestroyOnLoad(launcher);
            launcher.AddComponent<WebLevelLauncher>();
        }
#endif

        [Preserve]
        public void LoadTransport()
        {
            LoadLevel("TransportLayer");
        }

        [Preserve]
        public void LoadNetwork()
        {
            LoadLevel("NetworkLayer");
        }

        private static void LoadLevel(string sceneName)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(sceneName);
            Debug.Log("WEB_LEVEL_READY: " + sceneName);
        }
    }
}
