# Transport and Network — team integration update

Target Unity Editor: **2022.3.62f3**. URP **14.0.12**, TextMesh Pro **3.0.7**.
The complete editable project is in `UnityProject`.
The independent `TransportLayer.unitypackage` and `NetworkLayer.unitypackage` files are in `LevelPackages`.
Read the corresponding Import Guide in `LevelPackages` before merging into the group menu project.

## Changes
- Use Unity 2022's Rigidbody.velocity API instead of Unity 6's linearVelocity.
- Use the matching URP version and re-save materials/scenes in the target editor.
- Use a 1200 x 700 reference layout and default window/Web canvas size.
- Save three styled UI Buttons into each result panel, with empty On Click lists:
  RestartLevelButton, NextLevelButton and LevelMenuButton.
- Keep button behavior for the menu integrator; no navigation handlers are assigned.
- Preserve scene gameplay, instructional labels and looping music at 15%.

## Integration
Open `UnityProject` in the exact target editor, or import one/both packages into the existing URP 14 project.
Select `Canvas/FactPanel/LevelNavigation` in each scene to find the buttons (FactPanel starts hidden).
Wire handlers in the Inspector; reset Time.timeScale before leaving a completed/timed-out level.
Both packages share original script/material GUIDs. Review existing shared assets before overwriting local changes.

## Build/export
In the target editor, use Tools > Transport Network Layers > Prepare Team Integration.
This saves the two scenes and exports independent packages into the parent directory.
It can also be called with `-batchmode -projectPath <UnityProject> -executeMethod PrepareTeamIntegration.Prepare -quit`.
No automated gameplay tests are included or claimed.

## Earlier demonstrations
The existing [online demo](https://yeyuan9.itch.io/inside-the-network-transport-network-layers)
and the supplied short recordings show the earlier Unity 6 release, not this integration update.
The short recordings do not replace the 3–5 minute narrated group demonstration.


## Conversion evidence and limits
Both scenes were saved and both packages were natively exported by Unity **2022.3.62f3**.
The final editor process exited with code 0; its log reported `TEAM_INTEGRATION_EXPORTED: 2022.3.62f3`.
The final conversion log contained no C# or shader compilation errors.
Official TMP 3.0.7 Essential Resources, including shader .cginc dependencies, are included.
Full gameplay and integration in the teammate's menu project have not been tested.
No button navigation behavior is implemented, as requested.

## Earlier recordings and screenshots
- [Transport recording](docs/videos/TransportLayer.mp4)
- [Network recording](docs/videos/NetworkLayer.mp4)
- [Screenshots](docs/screenshots/README.md)
These show the earlier Unity 6 release. They do not show the new placeholder buttons or certify the new handoff.
