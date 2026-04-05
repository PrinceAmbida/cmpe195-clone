# Inside the Network

## CMPE 195A - Senior Design Project I, Spring 2026

**Team:** Prince Erzel Ambida, Ye Yuan, Padmavathy Elencheliyan  
**Advisor:** Ahmed Banafa

---

## Project Overview

A network-themed 3D platformer game built in Unity (C#) where the player controls data, which obtains/loses headers and increases/decreases in size, as it is transmitted from an IoT device to a server, demonstrating how IoT devices function in a smart home environment. The game teaches introductory computer networking concepts through interactive gameplay.

## Problem

Many computer engineering students struggle to grasp abstract networking concepts through traditional lectures alone. This project aims to make learning more engaging by turning networking topics into hands-on gameplay.

## Target Users

Undergraduate students in introductory computer networking courses.

## Game Design

- **Setting:** A smart home hub (laptop), with rooms containing IoT devices
- **Player Role:** A data packet collecting information and completing challenges
- **Devices:** Smart speakers, security cameras, thermostats, lighting, locks, appliances
- **Levels:** ~14 levels per device (7 sending, 7 receiving)
- **Mechanics:** Timer-based completion, data collection goals, networking facts after each level
- **Theme Variation:** Each device has a unique theme based on whether it prioritizes speed or data integrity

## Tech Stack

| Component | Technology |
|-----------|------------|
| Game Engine | Unity |
| Language | C# |
| Graphics | 3D (fallback to 2D if needed) |
| Platform | PC |

## Project Structure

```
/
├── README.md
├── docs/
│   └── design-document.md
├── unity-project/       # Unity project files (TBD)
├── assets/              # Game assets (TBD)
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
