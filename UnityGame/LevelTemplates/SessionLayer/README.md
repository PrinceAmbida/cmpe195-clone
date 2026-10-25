# Session Layer minigame – "Session Control"

A 2D UI minigame for the OSI **session layer** (Layer 5), sized for a **1200 × 700** window.
The player is a session manager working four "shifts":

| Shift | Session-layer job | What the player does |
|---|---|---|
| 1. Open the Session | Establishment (request, authenticate, negotiate, confirm with a session ID) | Run 5 clients through the setup steps in order; reject clients that fail authentication |
| 2. Pass the Token | Dialog control (half-duplex token) | Only the token holder may send; pass the token before an outbox overflows |
| 3. Checkpoint It | Synchronization + termination | Insert checkpoints during a flaky 100 MB transfer, then choose how to close the session |
| 4. Debrief | Review | 5-question quiz with explanations |

Max score: 400.

This folder is a **Unity 2022.3.62f3** project (the version in the main README). The scene is the hand-built layout,
converted from the original Unity 6 project.

## Open and play

1. **Unity Hub → Add → Add project from disk** → choose this `SessionLayer` folder. Open it with **2022.3.62f3**.
2. The first import takes a few minutes. The project then sets itself up automatically:
   it imports TextMeshPro's fonts, adds the scene to Build Settings, sets the 1200 × 700 window, and opens
   `Assets/Scenes/SessionLayer.unity`.
   If that doesn't happen, use **Tools → Session Layer → Run Project Setup**.
3. Game view size dropdown → **+** → Fixed Resolution **1200 × 700**, then press **Play**.

## Make the Windows .exe (like the other layers)

**Tools → Session Layer → Build Windows Game.** The output is `SessionLayer/Game/SessionControl.exe`.
Commit the **whole `Game` folder**: the .exe can't run without the files next to it.

## Commit / don't commit

Commit `Assets/` (with all `.meta` files), `Packages/`, `ProjectSettings/`, `Game/`, `.gitignore`, `.vsconfig`, this README.
Never commit `Library/`, `Temp/`, `Logs/`, `obj/`, `UserSettings/`, `.vs/`, `*.sln`, `*.csproj` (the `.gitignore` excludes them).
Use GitHub Desktop or git, not the website upload (it rejects files over 25 MB and more than 100 files at once).

## Files

```
SessionLayer/
├─ Assets/
│  ├─ Scenes/SessionLayer.unity   the game scene
│  ├─ Scripts/                    GameFlow, ShiftBase, one script per shift
│  ├─ Prefab/UIButton.prefab      button used throughout the UI
│  └─ Editor/                     project setup + Windows build menu (editor only)
├─ Packages/manifest.json
├─ ProjectSettings/
└─ Game/                          Windows build (after "Build Windows Game")
```

## Troubleshooting

| Problem | Fix |
|---|---|
| Text is missing or pink | Window → TextMeshPro → Import TMP Essential Resources, then reopen the scene |
| "Missing script" on objects | Make sure each `.cs` file in `Assets/Scripts` still has its `.cs.meta` file next to it |
| Buttons don't respond | Select `EventSystem`; it should have a **Standalone Input Module** |
| Unity offers to upgrade the project | Click **Cancel** and open it with 2022.3.62f3 instead, so it matches the team |
