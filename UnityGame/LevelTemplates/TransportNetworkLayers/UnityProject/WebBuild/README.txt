Inside the Network - Browser Demo

Unity version: 6000.5.4f1
Levels: TransportLayer and NetworkLayer
Default canvas resolution: 1200 x 700 (scales down on smaller screens)
Background music: loops at 15% volume

BUILD FROM SOURCE
Install Web Build Support for Unity 6000.5.4f1.
In Unity, select Tools > Transport Network Layers > Build Web Player.
Then run from the project root:
  python WebBuild/package_web_player.py

The upload file is Exports/TransportNetworkLayers-WebGL.zip.
This is a browser game build. The .unitypackage file is for importing assets
into another Unity project, and is not an HTML5 upload.

LOCAL PREVIEW
From the project root, run:
  python -m http.server 8765 --bind 127.0.0.1 --directory Exports/WebGL
Open http://127.0.0.1:8765 in a current desktop browser.
Do not open index.html directly from the file system.

ITCH.IO UPLOAD
Create or edit a project on itch.io and choose HTML as its project type.
Upload Exports/TransportNetworkLayers-WebGL.zip.
Mark it as a file to be played in the browser.
Use an embed viewport of 1200 x 800 to include the level selection toolbar
and controls below the 1200 x 700 game canvas. Allow fullscreen.
Make sure the final game page can be opened by the grader without signing in.

CONTROLS
Choose Transport Layer or Network Layer, then click Play.
WASD: move. Mouse: look. Space: jump. Esc: release the mouse.
Click inside the game or press Tab to lock the mouse again.
Browsers may wait for a click inside the game before playing audio.
Use the page buttons to switch levels; this resets the selected level.
Use Try Again on the result screen to restart the current level.

Music sources and CC0 licensing are listed in MUSIC-CREDITS.txt in the build.
