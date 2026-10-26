# Inside the Network

## CMPE 195A - Senior Design Project I, Spring 2026

**Team:** Prince Erzel Ambida, Ye Yuan, Padmavathy Elencheliyan  
**Advisor:** Ahmed Banafa

---

## Project Description

A network-themed 3D platformer game built in Unity (C#) where the player controls data, which obtains/loses headers and increases/decreases in size, as it is transmitted from an IoT device to a server, demonstrating how IoT devices function in a smart home environment. The game teaches introductory computer networking concepts through interactive gameplay.

## Prototype Scope

Currently, almost all layers have fully functional games. The presentation, network, transport, and data link layers have games in WebGL for testing. Other level games will be finalized and all layer games will be combined into a single application by Expo.

## System Architecture



## Prerequisites
The PoC files are restricted to Windows PCs, based on build settings. Source code and WebGL demos can be viewed on any computer.

## Installation
Assuming a Windows operating system, download ZIP of all files in this project.

## Running the PoC
After downloading and extracting the ZIP, the executable or files of each level will be present in their respective folders.
Currently, all running games are on WebGL.
Source code (C# scripts) is under UnityGame\SourceCode.

## Demo
WebGL has playable games for presentation, transport, network, and data link layers.
[Presentation and data link layers](https://padmae.itch.io/inside-the-network)
[Transport and network layers](https://yeyuan9.itch.io/inside-the-network-transport-network-layers)

## Technical Stack

| Component | Technology |
|-----------|------------|
| Game Engine | Unity (version 2022.3.62f3) |
| Language | C# |
| Graphics | 3D |
| Platform | PC |

## What's Next
Finalize and combine remaining games into a single application. Application executable for Windows will be posted here, and WebGL will be updated to include entire app.
Complete companion website with AI chatbot.

## Project Structure

```
/
├── README.md
├── docs/
│   └── design-document.md
├── AIChatbot/           # Code used for AI chatbot and website
│   └── FolderInfo.md
├── UnityGame/           # All Unity related code and projects
│   └── LevelTemplates/  # For prototype of each basic level
│       └── LevelInfo.md
│       └── PresentationLayer/  # Contains prototype of phishing maze
└── references/          # Research and reference materials
```

## References

1. SkyQuest, "IOT Smart Homes Market Growth Analysis," 2025. [Link](https://www.skyquestt.com/report/iot-smart-homes-market)
2. M. Prvan and J. Ožegović, "Methods in Teaching Computer Networks: A Literature Review," *ACM Trans. Comput. Educ.*, vol. 20, no. 3, 2020. [Link](https://dl.acm.org/doi/10.1145/3394963)
3. I. Miljanovic and T. Bradbury, "Game-based learning in computer science education: a scoping literature review," *Int. J. STEM Educ.*, vol. 10, no. 54, 2023. [Link](https://stemeducationjournal.springeropen.com/articles/10.1186/s40594-023-00447-2)


## Transport and Network integration update (Ye Yuan)

The Unity 2022.3.62f3 handoff, independent import packages, and button-wiring instructions are in
[TransportNetworkLayers](UnityGame/LevelTemplates/TransportNetworkLayers/README.md).
The three styled result buttons in each saved scene have empty On Click handlers for menu integration.
The earlier [browser demo](https://yeyuan9.itch.io/inside-the-network-transport-network-layers)
remains a separate Unity 6 release and does not demonstrate this update.
