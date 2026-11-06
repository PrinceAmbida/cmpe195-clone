// Inside the Network - integration layer
// Finishes Padma's LevelMenu at runtime, without editing her scene.
//
// Her menu has one button per OSI layer (MenuUI/LevelList/AppButton, PreButton, ...).
// Presentation and Data Link open her info panels (PhishingLevelUI, SpoofingLevelUI) with a
// "Start Game" button. The other layer buttons were stubs. For each of those layers this
// script clones her Phishing panel, fills in that layer's title / threat / instructions,
// and points the layer button and the panel's Start button at the right scene.
// Every panel (hers included) also gets a Back button that returns to the layer list.
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace InsideTheNetwork.Integration
{
    public static class LevelMenuBridge
    {
        const string TemplatePanel = "PhishingLevelUI";

        // Layers that already have Padma's own panel.
        static readonly Dictionary<string, string> ExistingPanels = new Dictionary<string, string>
        {
            { "PreButton", "PhishingLevelUI" },
            { "LinButton", "SpoofingLevelUI" },
        };

        public static void Apply(Scene scene)
        {
            GameObject template = FindInScene(scene, TemplatePanel);
            GameObject levelList = FindInScene(scene, "LevelList");
            if (template == null || levelList == null)
            {
                Debug.LogWarning("[Integration] LevelMenu layout not recognised (" + TemplatePanel + " / LevelList missing).");
                return;
            }

            foreach (LayerInfo layer in LevelCatalog.Layers)
            {
                GameObject buttonObject = FindChild(levelList.transform, layer.MenuButton);
                if (buttonObject == null) continue;

                GameObject panel;
                string existing;
                if (ExistingPanels.TryGetValue(layer.MenuButton, out existing))
                {
                    panel = FindInScene(scene, existing);
                    if (panel == null) continue;
                    // Padma's own panel and buttons are left exactly as she wired them.
                }
                else
                {
                    panel = Object.Instantiate(template, template.transform.parent);
                    panel.name = layer.Name.Replace(" ", "") + "LevelUI";
                    panel.SetActive(false);
                    // Runtime listeners are not copied by Instantiate, so drop any Back button
                    // cloned from the template; AddBackButton below creates a working one.
                    GameObject clonedBack = FindChild(panel.transform, "BackButton");
                    if (clonedBack != null) Object.DestroyImmediate(clonedBack);
                    SetText(panel, "Title", layer.Threat);
                    SetText(panel, "Subheading", layer.Subheading);
                    SetText(panel, "Instructions", layer.Instructions);

                    Button start = FindButton(panel, "StartButton");
                    if (start != null)
                    {
                        bool playable = LevelLoader.IsPlayable(layer);
                        LayerInfo captured = layer;
                        start.onClick = new Button.ButtonClickedEvent();
                        start.onClick.AddListener(() => LevelLoader.Play(captured));
                        start.interactable = playable;
                        SetText(start.gameObject, null, playable ? "Start Game" : "Coming Soon");
                    }

                    Button layerButton = buttonObject.GetComponent<Button>();
                    if (layerButton != null)
                    {
                        GameObject target = panel;
                        layerButton.onClick = new Button.ButtonClickedEvent();
                        layerButton.onClick.AddListener(() =>
                        {
                            levelList.SetActive(false);
                            target.SetActive(true);
                        });
                    }
                }

                AddBackButton(panel, levelList);
            }
        }

        // A copy of the panel's Start button, placed under it, that returns to the layer list.
        static void AddBackButton(GameObject panel, GameObject levelList)
        {
            Button start = FindButton(panel, "StartButton");
            if (start == null || FindChild(panel.transform, "BackButton") != null) return;

            GameObject back = Object.Instantiate(start.gameObject, start.transform.parent);
            back.name = "BackButton";
            RectTransform rt = (RectTransform)back.transform;
            RectTransform startRt = (RectTransform)start.transform;
            rt.anchoredPosition = startRt.anchoredPosition + new Vector2(0f, -(startRt.sizeDelta.y + 15f));

            Button button = back.GetComponent<Button>();
            button.interactable = true;
            button.onClick = new Button.ButtonClickedEvent();
            GameObject target = panel;
            button.onClick.AddListener(() =>
            {
                target.SetActive(false);
                levelList.SetActive(true);
            });
            SetText(back, null, "Back");
        }

        // ---------------- helpers ----------------

        static void SetText(GameObject root, string childName, string value)
        {
            TMP_Text text = null;
            if (childName == null)
            {
                text = root.GetComponentInChildren<TMP_Text>(true);
            }
            else
            {
                GameObject child = FindChild(root.transform, childName);
                if (child != null) text = child.GetComponent<TMP_Text>();
            }
            if (text != null) text.text = value;
        }

        static Button FindButton(GameObject root, string name)
        {
            GameObject child = FindChild(root.transform, name);
            return child != null ? child.GetComponent<Button>() : null;
        }

        static GameObject FindChild(Transform root, string name)
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                if (t.name == name) return t.gameObject;
            return null;
        }

        static GameObject FindInScene(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                GameObject found = FindChild(root.transform, name);
                if (found != null) return found;
            }
            return null;
        }
    }
}
