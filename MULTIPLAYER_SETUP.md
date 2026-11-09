# Toot Raft Rush — Online Multiplayer Setup Guide 🌐

This document outlines the networking architecture, room configuration, state synchronization protocol, and testing instructions for **Toot Raft Rush**.

---

## 🏗️ Networking Architecture

**Toot Raft Rush** implements a client-authoritative host topology designed for fast-paced physics-driven cooperative party games. It is built to interface directly with **Photon PUN 2 (Photon Unity Networking)** or compatible WebRTC/WebSocket backends.

```
       [Room Host / Master Client]
       - Runs primary water & raft physics
       - Validates checkpoints & timer
       - Broadcasts Raft state (20 Hz)
                 ▲
                 │
       ┌─────────┴─────────┐
       ▼                   ▼
 [Client 2]           [Client 3]
 - Sends player input - Sends player input
 - Fires fart RPCs    - Fires fart RPCs
 - Interpolates raft  - Interpolates raft
```

### State Synchronization Components
1. **Raft State Replication (`BroadcastRaftState`)**:
   - Packets include: `Vector3 position`, `Quaternion rotation`, `Vector3 velocity`, and `float timestamp`.
   - Non-host clients use Hermite / Lerp interpolation to ensure silky-smooth raft sliding even under 150ms latency.
2. **Fart Impulse RPC (`SendFartRPC`)**:
   - When any player fires a fart, an RPC (`RPC_TriggerFart`) is dispatched across all room members.
   - All clients immediately spawn the green gas puff VFX, play character-specific toot audio with pitch variation, and render screen shake.
3. **Game State Synchronization**:
   - Master client synchronizes level start countdown, active checkpoint index, total team deaths, and win screen triggers.
4. **Seamless Offline Fallback**:
   - When running in Single Player mode or if Photon credentials are not set, `NetworkManager.cs` gracefully operates in **Local / Offline Simulation Mode**.
   - Local AI companions automatically replace remote players, ensuring the game is 100% playable out of the box without any network configuration!

---

## 🔑 Required Credentials (Photon PUN 2)

To enable live online multiplayer between separate Android devices or across the internet:

1. Create a free account at [Photon Engine](https://www.photonengine.com/).
2. In the Photon Dashboard, create a new application:
   - **Photon Type**: `Photon Realtime` or `PUN`
   - **Name**: `Toot Raft Rush`
3. Copy your **AppId** (a 36-character GUID, e.g. `12345678-abcd-ef01-2345-6789abcdef01`).
4. In the Unity project:
   - Select the `NetworkManager` object in `Assets/Scenes/Lobby.unity` or `MainMenu.unity`.
   - Paste your AppId into the `Photon App Id` field in the Inspector.
   - (Or configure `PhotonServerSettings.asset` if using the Photon PUN 2 asset package).

> ⚠️ **Note**: No credentials are hardcoded into the repository. The project will run in offline co-op mode until an AppId is provided.

---

## 🏠 Room Configuration & Lobby Flow

- **Room Code**: 4-letter alphanumeric string (e.g., `RUSH`, `TOOT`, `RAFT`).
  - Omits ambiguous characters (`0`, `O`, `1`, `I`) to prevent confusion on mobile screens.
- **Max Players**: 4 players per raft.
- **Player Slots**:
  - Slot 0: Host (Blue Hippo by default)
  - Slot 1: Client 1 (Grey Rhino)
  - Slot 2: Client 2 (Green Frog)
  - Slot 3: Client 3 (Orange Capybara)
- **Ready System**: All joined clients must toggle "READY!" before the Host can launch the match.
- **Disconnect Handling**: If a remote client disconnects, `NetworkManager.HandleDisconnect()` automatically spawns an AI companion to seamlessly take over the vacant player slot without stopping the raft!

---

## 🧪 Testing Instructions

### 1. Local Testing in Unity Editor
1. Open `Assets/Scenes/Lobby.unity` in the Unity Editor.
2. Click **Create Room**:
   - A random 4-letter room code will be generated.
   - Slot 0 is assigned to you, and Slots 1-3 are populated with ready AI companions.
3. Click **Start Match** to launch `SunnySlide.unity`.

### 2. Multi-Client Testing (Editor + Android Device)
1. Build the APK and install it on an Android phone.
2. Start the game on the phone and open the **Lobby**.
3. Tap **Create Room** and note the 4-letter code (e.g. `COOL`).
4. On your PC (in Unity Editor or a standalone build), open the **Lobby**, enter `COOL` in the input field, and click **Join Room**.
5. Both devices will appear in the player list.
6. Toggle **Ready** on both, and click **Start Match**.
7. Confirm that:
   - Raft movement is synchronized across both screens.
   - Farting on one device spawns green gas particles and plays toot sounds on both devices.
   - Wiping out on an obstacle resets both devices to the last checkpoint.
