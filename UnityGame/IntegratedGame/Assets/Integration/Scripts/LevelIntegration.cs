// Inside the Network - integration layer
// Created automatically before the first scene loads and kept for the whole run.
// Each time a level scene loads, it connects that level's own end-of-level UI to the
// shared menu flow, so none of the teammates' scenes had to be edited by hand:
//
//   Transport / Network : RestartLevelButton, NextLevelButton, LevelMenuButton (Ye's empty buttons)
//   Session             : GameFlow.onBackToMenu (Prince's "Back to menu" button)
//   Application         : AppLayerGameManager.onMinigameComplete -> adds Restart / Next / Menu buttons
//   Presentation / Link : Padma's empty "NextLevel" buttons (her Retry / Level Menu already work)
//   Padma's LevelMenu   : LevelMenuBridge adds panels for the layers that were stubs
//   Every level         : F1 or the on-screen "Level Menu" button returns to the level menu
using OSIGame.SessionLayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace InsideTheNetwork.Integration
{
    public class LevelIntegration : MonoBehaviour
    {
        public const KeyCode MenuKey = KeyCode.F1;

        LayerInfo currentLayer;
        GUIStyle overlayStyle;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            GameObject go = new GameObject("[Inside the Network Integration]");
            DontDestroyOnLoad(go);
            go.AddComponent<LevelIntegration>();
        }

        void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }

        void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Single) return;
            currentLayer = LevelCatalog.FindByScene(scene.name);
            if (scene.name == LevelCatalog.LevelMenuScene) LevelMenuBridge.Apply(scene);
            if (currentLayer == null) return; // a menu scene

            WireNamedButtons(scene);
            WireSessionLayer();
            WireApplicationLayer(scene);
            Debug.Log("[Integration] Loaded " + currentLayer.Name + " layer (" + scene.name + ")");
        }

        // ---------------- Transport / Network / Padma's levels (and any future scene that uses the same names) ----------------

        static void WireNamedButtons(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Button button in root.GetComponentsInChildren<Button>(true))
                {
                    switch (button.name)
                    {
                        case "RestartLevelButton": button.onClick.AddListener(LevelLoader.RestartCurrent); break;
                        case "NextLevelButton": button.onClick.AddListener(LevelLoader.NextAfterCurrent); break;
                        case "LevelMenuButton": button.onClick.AddListener(LevelLoader.LevelMenu); break;
                        case "NextLevel": // Padma's levels; only if she hasn't wired it herself
                            if (button.onClick.GetPersistentEventCount() == 0)
                                button.onClick.AddListener(LevelLoader.NextAfterCurrent);
                            break;
                    }
                }
            }
        }

        // ---------------- Session ----------------

        static void WireSessionLayer()
        {
            GameFlow flow = FindObjectOfType<GameFlow>(true);
            if (flow == null) return;
            if (flow.onBackToMenu == null) flow.onBackToMenu = new UnityEvent();
            flow.onBackToMenu.AddListener(LevelLoader.LevelMenu);
        }

        // ---------------- Application ----------------

        static void WireApplicationLayer(Scene scene)
        {
            AppLayerGameManager manager = FindObjectOfType<AppLayerGameManager>(true);
            if (manager == null) return;

            Transform summaryPanel = FindInScene(scene, "SummaryPanel");
            if (manager.onMinigameComplete == null) manager.onMinigameComplete = new UnityEvent<bool>();
            manager.onMinigameComplete.AddListener(passed => AddApplicationButtons(summaryPanel));
        }

        static void AddApplicationButtons(Transform summaryPanel)
        {
            Canvas canvas = summaryPanel != null ? summaryPanel.GetComponentInParent<Canvas>() : null;
            Transform parent;
            if (canvas != null)
            {
                canvas = canvas.rootCanvas;
                parent = canvas.transform;
            }
            else
            {
                parent = MenuUI.CreateCanvas("LevelNavigationCanvas", 50);
            }
            if (parent.Find("LevelNavigation") != null) return;

            RectTransform row = MenuUI.Rect(parent, "LevelNavigation");
            MenuUI.Place(row, new Vector2(0.5f, 0f), new Vector2(780, 56), new Vector2(0, 50));
            row.SetAsLastSibling();

            Button restart = MenuUI.Button(row, "RESTART LEVEL", MenuUI.Blue, LevelLoader.RestartCurrent, 20);
            MenuUI.Place((RectTransform)restart.transform, new Vector2(0.5f, 0.5f), new Vector2(240, 56), new Vector2(-260, 0));
            Button next = MenuUI.Button(row, "NEXT LEVEL", MenuUI.Teal, LevelLoader.NextAfterCurrent, 20);
            MenuUI.Place((RectTransform)next.transform, new Vector2(0.5f, 0.5f), new Vector2(240, 56), new Vector2(0, 0));
            Button menu = MenuUI.Button(row, "LEVEL MENU", MenuUI.Purple, LevelLoader.LevelMenu, 20);
            MenuUI.Place((RectTransform)menu.transform, new Vector2(0.5f, 0.5f), new Vector2(240, 56), new Vector2(260, 0));

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        static Transform FindInScene(Scene scene, string objectName)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name == objectName) return root.transform;
                foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                    if (child.name == objectName) return child;
            }
            return null;
        }

        // ---------------- Always-available way back ----------------

        void Update()
        {
            if (currentLayer != null && Input.GetKeyDown(MenuKey))
                LevelLoader.LevelMenu();
        }

        void OnGUI()
        {
            if (currentLayer == null) return;
            // The FPS levels lock the mouse while playing; Esc frees it and reveals the button.
            if (Cursor.lockState == CursorLockMode.Locked) return;

            if (overlayStyle == null)
            {
                overlayStyle = new GUIStyle(GUI.skin.button) { fontSize = 14, fontStyle = FontStyle.Bold };
            }
            Rect rect = new Rect(10, Screen.height - 42, 190, 32);
            if (GUI.Button(rect, "< Level Menu  (" + MenuKey + ")", overlayStyle))
                LevelLoader.LevelMenu();
        }
    }
}
