# Transport and Network Levels

1. Use Unity 2022.3.62f3, URP 14.0.12, Input System 1.7.0 and TextMesh Pro 3.0.7.
2. Open UnityProject, or import the two packages from LevelPackages. Read the import guides first.
3. Open Assets/Scenes/TransportLayer.unity or NetworkLayer.unity and press Play.
4. Keep Active Input Handling set to Both. Both scenes use InputSystemUIInputModule with default UI actions.
5. Game size: 1200 x 700. Music volume: 15%.
6. RESTART LEVEL, NEXT LEVEL and LEVEL MENU have no actions. Find them under Canvas/FactPanel/LevelNavigation.
7. Add button actions in the Inspector. Reset Time.timeScale to 1 before loading another scene.
8. Controls: WASD move, mouse look, Space jump, Esc release mouse, Tab lock mouse.

Transport: collect 3 TCP segments and 2 UDP datagrams while avoiding DoS enemies.
Network: reach Routers 1, 2 and 3, then the destination, while avoiding MITM enemies.

Both packages were exported with Unity 2022.3.62f3. Full gameplay and import into the team's menu project have not been tested.
Install Input System in the target project before importing: Unity packages do not install Package Manager dependencies.
Shared assets keep their original paths and GUIDs. Review shared files before replacing local changes.
Music credits are in UnityProject/Assets/Resources/LayerMusic/CREDITS.txt.

[Online demo](https://yeyuan9.itch.io/inside-the-network-transport-network-layers) · [Screenshots](docs/screenshots/README.md) · [Transport video](docs/videos/TransportLayer.mp4) · [Network video](docs/videos/NetworkLayer.mp4)

The online demo, screenshots and videos show the older Unity 6 version.
To export again, use Tools > Transport Network Layers > Prepare Team Integration in Unity 2022.3.62f3.
