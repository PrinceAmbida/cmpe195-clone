# Game Design Document

## Inside the Network: A Packet's Perspective

**Version:** 0.1 (Draft)  
**Date:** March 2026  
**Authors:** Prince Erzel Ambida, Ye Yuan, Padmavathy Elencheliyan

---

## 1. Concept

The player takes on the role of a data packet traveling through a smart home network. The game is a 3D platformer built in Unity where each IoT device in the home represents a different networking theme.

## 2. Smart Home Layout

The player hub is a laptop within a house. Each room contains one or more IoT devices:

| Room | Device(s) | Network Theme |
|------|-----------|---------------|
| Living Room | Smart Speaker, Smart TV | Audio/Video streaming, bandwidth |
| Front Door | Smart Lock, Security Camera | Encryption, authentication |
| Hallway | Thermostat | Sensor data, low-bandwidth protocols |
| Kitchen | Smart Appliances | Device-to-device communication |
| Bedroom | Smart Lighting | Low-latency control signals |

> Note: Room and device assignments are preliminary and subject to change.

## 3. Level Structure

Each smart device has approximately **14 levels**:
- **Levels 1–7:** Prepare and send a data packet (encoding, packetizing, routing)
- **Levels 8–14:** Receive and process a data packet (decoding, error checking, delivery)

### Level Completion Criteria
- Finish before the timer runs out
- Collect enough data items for that level's threshold

### Post-Level
- A networking fact is displayed after completing each level

## 4. Player Mechanics

- **Movement:** Standard 3D platformer controls (run, jump)
- **Collection:** Player collects data bits/items scattered across platforms
- **Timer:** Each level has a countdown timer adding urgency
- **Lives/Retry:** TBD

## 5. Visual Style

- Primary target: 3D graphics
- Fallback: 2D if 3D asset creation proves too time-consuming
- Inspiration: Yooka-Laylee style colorful platformer

## 6. Networking Concepts Covered (Planned)

- Physical layer (cables, signals)
- Data link layer (framing, error detection)
- Network layer (IP addressing, routing)
- Transport layer (TCP vs UDP, reliability vs speed)
- Application layer (HTTP, DNS)
- IoT-specific protocols (MQTT, Zigbee, Z-Wave)
- Encryption and authentication basics

## 7. Tools & Technologies

- **Engine:** Unity
- **Language:** C#
- **Version Control:** Git / GitHub
- **Asset Creation:** TBD (Blender, Unity Asset Store, or 2D alternatives)

## 8. Open Questions

- [ ] Final decision on 3D vs 2D
- [ ] Specific networking facts for each level
- [ ] Scoring system details
- [ ] Sound design approach
- [ ] Accessibility considerations
