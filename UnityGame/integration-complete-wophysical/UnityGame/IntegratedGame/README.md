# Inside the Network: combined application

This Unity **2022.3.62f3** project is the whole game in one app. It starts on **Padma's title screen and level
menu**, and every layer that exists so far plays inside it.

| OSI layer | Threat | Made by | In this project |
|---|---|---|---|
| — | Title screen + level menu | Padma | `Assets/Levels/PresentationDataLink/Scenes/TitleScene`, `LevelMenu` |
| 7 Application | Exploit | Prince | `Assets/Levels/ApplicationLayer` (scene `Application_layer`) |
| 6 Presentation | Phishing | Padma | `Assets/Levels/PresentationDataLink` (scene `PhishingScene`) |
| 5 Session | Hijacking | Prince | `Assets/Levels/SessionLayer` (scene `SessionLayer`) |
| 4 Transport | Denial of Service | Ye | `Assets/Scenes/TransportLayer` (Ye's original paths) |
| 3 Network | Man-in-the-Middle | Ye | `Assets/Scenes/NetworkLayer` (Ye's original paths) |
| 2 Data Link | Spoofing | Padma | `Assets/Levels/PresentationDataLink` (scene `SpoofingScene`) |
| 1 Physical | Sniffing | Padma (started) | Only `SniffMovement.cs` exists. The menu shows "Coming Soon" until a `SniffingScene` is added |

## Start it

### In the Unity Editor
1. Install **Unity 2022.3.62f3**. To build the .exe, also install **Windows Build Support (Mono)**.
2. Unity Hub > **Add > Add project from disk** > select `UnityGame/IntegratedGame`.
3. The first open takes a few minutes while Unity downloads packages (URP 14, Input System 1.7, TMP 3.0.7).
   - If Unity asks to **enable the new Input System backends**, click **No**. Active Input Handling must stay **Both**.
   - If Unity says it will reset the window layout, click **Continue**. That only rearranges editor panels.
   - If Unity offers to upgrade the project version, click **Cancel** and open it with 2022.3.62f3.
4. `TitleScene` opens automatically. If it doesn't, use **Tools > Inside the Network > Open Title Scene**.
5. Set the Game view to **1200 x 700** and press **Play**.

Always press Play from `TitleScene`.

### As a Windows app
**Tools > Inside the Network > Build Windows App** builds `UnityGame/IntegratedGame/Build/InsideTheNetwork.exe`.
To share it, zip the **whole `Build` folder**.

Command line (also used by `impl.yaml`):
```
Unity -batchmode -quit -projectPath UnityGame/IntegratedGame -executeMethod InsideTheNetwork.IntegrationEditor.IntegrationTools.BuildWindowsAppBatch -logFile -
```

### Game flow
Title screen > **Start** > Level menu > pick a layer > info panel (threat, instructions) > **Start Game** > level.
When a level ends, its own buttons go to the level menu, retry, or the **next layer down the stack**.
**F1** always returns to the level menu. In the first-person levels, press Esc first to free the mouse,
and a **Level Menu** button also appears in the bottom-left corner.

## Where Padma's part came from
Her Unity project was never committed to any branch, only builds and copies of her scripts. Her menus and levels
were recovered from her newest build (`UnityGame/CurrentImplementation`, Oct 25) with **AssetRipper** and then repaired:

- **Her scripts:** the decompiled copies were replaced with her original scripts from `UnityGame/SourceCode`.
  The originals match the build, apart from decompiler artifacts. The recovered `.meta` IDs were kept, so her
  scenes still connect to them. `SniffMovement.cs` (her Physical-layer start) has no original, so it is the recovered version.
- **Package components:** AssetRipper exported the packages she used as loose DLLs. Every component in her
  scenes (TextMesh Pro text, UI images and buttons, URP camera/light data, post-processing volume) was pointed back
  at the real package scripts. Each match was verified by Unity's class-ID hash and against the IDs that the other
  2022.3 scenes use.
- **Shaders and fonts:** her materials use URP Lit again, and her text uses the project's TextMesh Pro LiberationSans font.
- **Removed:** AssetRipper's generated lighting data, which Unity rebuilds. The extra copies of engine, package and
  shader code were never brought in.
- **Kept:** her custom "Floor" layer, added to the project's layers.
- **Unchanged:** her scenes are otherwise as she built them, including button wiring (Start > LevelMenu,
  Retry, Level Menu), audio, materials and layout.

## How the integration works
Nothing in the teammates' level scenes was edited by hand. The glue is in `Assets/Integration/Scripts`:

| File | Job |
|---|---|
| `LevelCatalog.cs` | One list of all seven layers: name, threat, scene, instructions, and the matching button in Padma's level menu. **Edit this to add or rename a level.** |
| `LevelLoader.cs` | Scene changes for Restart / Next / Level Menu. Resets `Time.timeScale` and the cursor, because Transport/Network pause the game and the FPS levels lock the mouse. |
| `LevelMenuBridge.cs` | Finishes Padma's level menu while the game runs. Her Application, Session, Transport, Network and Physical buttons were stubs. For each one, it clones her Phishing info panel, fills in that layer's text, and wires **Start Game**. It also adds a **Back** button to every panel, hers included. |
| `LevelIntegration.cs` | Starts with the game and runs on every scene load. It wires each level's end screen: Ye's Restart/Next/Level Menu buttons, Padma's empty **Next Level** buttons, Session's "Back to menu", and new buttons on Application's summary. It also handles F1 and the corner button. |
| `MenuUI.cs` | Small helper that builds the end-of-round buttons for Application. |
| `../Editor/IntegrationTools.cs` | **Tools > Inside the Network**: Open Title Scene, Sync Build Settings, Build Windows App. |

**Next Level** goes 7 > 6 > 5 > 4 > 3 > 2 and returns to the level menu after Data Link (Physical isn't built yet).

Build Settings order: TitleScene, LevelMenu, Application_layer, PhishingScene, SessionLayer, TransportLayer,
NetworkLayer, SpoofingScene. **Sync Build Settings** rebuilds this list automatically from `LevelCatalog`.

Other small changes:
- Padma's `SwitchScenes.cs`: `MainMenu()` now loads `TitleScene`. Her map button used to load a `MainMenu` scene
  that didn't exist. The layer methods use the real scene names, and Physical uses `SniffingScene`.
  The same file is updated in `UnityGame/SourceCode`.
- Prince's Session editor script no longer resets Build Settings to only the Session scene when the editor opens.
- Ye's `PrepareTeamIntegration.cs` and `BuildLayerWebPlayer.cs` were left out of this project. The first rewrites
  every material in `Assets/`, so it would also change other people's levels. Both are still in
  `LevelTemplates/TransportNetworkLayers`.

## Adding the Physical layer (or any new level)
1. Put its assets in `Assets/Levels/<Name>/`, keeping the `.meta` files.
2. Name the scene as in `LevelCatalog.cs`. Physical expects **`SniffingScene`**, the name `SniffMovement` already uses.
3. **Tools > Inside the Network > Sync Build Settings.** The Physical panel's button changes from "Coming Soon" to "Start Game".
4. For automatic end-of-level wiring, name its buttons `NextLevel` / `NextLevelButton`, `RestartLevelButton` and
   `LevelMenuButton`, or call `SwitchScenes.LevelMenu()` as Padma's levels do.

## Check on first open
These files were assembled and checked outside Unity: every asset reference resolves, there are no duplicate IDs,
and every scene file parses. The project hasn't been opened in Unity yet. On the first open:
1. Check the Console for red compile errors.
2. Play through: title > level menu > each layer's panel > each level > its end-screen buttons.
3. Look for things recovery can affect: a material that looks wrong (reassign it to *Universal Render
   Pipeline/Lit*), or text that's sized differently.
