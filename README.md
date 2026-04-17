# Inside the Network

## CMPE 195A - Senior Design Project I, Spring 2026

**Team:** Prince Erzel Ambida, Ye Yuan, Padmavathy Elencheliyan  
**Advisor:** Ahmed Banafa

---

## Project Overview

A network-themed 3D platformer game built in Unity (C#) where the player controls data, which obtains/loses headers and increases/decreases in size, as it is transmitted from an IoT device to a server, demonstrating how IoT devices function in a smart home environment. The game teaches introductory computer networking concepts through interactive gameplay.

## Problem

Many computer engineering students struggle to grasp abstract networking concepts through traditional lectures alone. With the increase in IoT devices used by people on a daily basis, understanding networking concepts is also important for the general public and future engineers. This project aims to make learning more engaging by turning networking topics into hands-on gameplay.

## Target Users

Anyone who wants to learn about networks, including future engineers (current middle and high schoolers) and undergraduate students in introductory computer networking courses.

## Game Design

- **Setting:** A smart home hub (laptop), with rooms containing IoT devices
- **Player Role:** A data packet collecting information and completing challenges
- **Devices:** Smart speakers, security cameras, thermostats, lighting, locks, appliances
- **Levels:** ~14 levels per device (7 sending, 7 receiving)
- **Mechanics:** Timer-based completion, data collection goals, networking facts after each level
- **Theme Variation:** Each device has a unique theme based on whether it prioritizes speed or data integrity

## Project Description
A PC video game with minigames for each level of the OSI model, built in Unity with 3D graphics, demonstrating how data is transferred from IoT devices. This educational game will help players understand how network data transfer works as well as the threats each OSI model level encounters. A companion website with AI chatbot gives players personalized support and additional network learning information.

## Proof of Concept Scope
This PoC demonstrates a basic concept for one of the levels, specifically phishing threats on the presentation layer of the OSI model. The PoC is not a fully working game or level, but shows the maze game design.

## Prerequisites
The PoC is restricted to running on Windows PCs, so additional software may be needed to run on other operating systems.

## Installation
Assuming a Windows operating system, download ZIP of all files in this project.

## Running the PoC
After downloading and extracting the ZIP, navigate to the PresentationLayer folder (UnityGame/LevelTemplates/PresentationLayer).
Double-click the InsideTheNetworkAPP.exe file to start the game.
Controls are WASD or arrow keys to move the player (cube) forward, left, backward, and right, respectively.
Objective of the game is to navigate through the maze, without colliding with walls.

## Demo
[Link to demo](https://drive.google.com/file/d/1X-WRFqb0j2bj2xNkC1na61A_gTuiVYHt/view?usp=sharing)

## Technical Stack

| Component | Technology |
|-----------|------------|
| Game Engine | Unity |
| Language | C# |
| Graphics | 3D |
| Platform | PC |

## What's Next (195B)
Add minigames for the remaining levels of the OSI model based on their respective threats:
- Exploit-based game for Application layer
- Hijacking-based game for Session layer
- Denial of Service (DoS)-based game for Transport layer
- Man-in-the-Middle-based game for Network layer
- Spoofing-based game for Data Link layer
- Sniffing-based game for Physical layer
Finalize complete game with main menu showing IoT devices, which link to variations of minigames.
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

## Timeline

| Phase | Semester | Description |
|-------|----------|-------------|
| Research & Design | Spring 2026 (195A) | Problem analysis, competitor research, game design document |
| Development | Fall 2026 (195B) | Unity development, level design, testing |

## References

1. SkyQuest, "IOT Smart Homes Market Growth Analysis," 2025. [Link](https://www.skyquestt.com/report/iot-smart-homes-market)
2. M. Prvan and J. Ožegović, "Methods in Teaching Computer Networks: A Literature Review," *ACM Trans. Comput. Educ.*, vol. 20, no. 3, 2020. [Link](https://dl.acm.org/doi/10.1145/3394963)
3. I. Miljanovic and T. Bradbury, "Game-based learning in computer science education: a scoping literature review," *Int. J. STEM Educ.*, vol. 10, no. 54, 2023. [Link](https://stemeducationjournal.springeropen.com/articles/10.1186/s40594-023-00447-2)
