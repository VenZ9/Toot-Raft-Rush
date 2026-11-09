# Toot Raft Rush 💨🛟

**Toot Raft Rush** is an original 3D cooperative cartoon party game built in **Unity 2022.3 LTS** with the **Universal Render Pipeline (URP)** for **Android**.

Players take control of cute, expressive cartoon animals aboard a circular inflatable raft navigating winding water slides and obstacle-laden canals. With no motor or engine, propulsion and steering are driven entirely by directional fart blasts! Team up with three intelligent AI animal companions or friends via online multiplayer to blast, steer, dodge hazards, and rush across the finish line!

---

## 🌟 Key Features

- **Directional Fart Propulsion Physics**:
  - Pure physics-based movement: no engines or oars. Directional farts propel the raft forward.
  - Edge steering: shifting position to the raft edges generates rotational torque to steer around tight bends.
  - Multi-Player Co-op Combos: simultaneous farts trigger synchronized burst multipliers!
- **Juicy Hold-to-Charge Mechanic**:
  - Hold the large green FART button to compress and charge up to 2.4x propulsion power.
  - Dynamic camera FOV kick, screen shake, green gas puff VFX, expanding shockwave rings, and squash-and-stretch cartoon animations!
- **Playable Character Roster (4 Original Animals)**:
  1. **Blue Hippo**: Heavyweight blaster with wide thrust cone and high momentum.
  2. **Grey Rhino**: Raw horsepower with deep bass power rumble.
  3. **Green Frog**: Agile amphibian with rapid meter recovery and responsive steering.
  4. **Orange Capybara**: Zen party captain with high stability and explosive charged boosts.
- **Intelligent Cooperative AI Companions**:
  - In Single Player mode, three autonomous animal companions actively cooperate:
    - Maintain forward cruising momentum when speed drops.
    - Anticipate upcoming obstacles and shift weight to steer the raft away from danger.
    - Match player charges for synchronized Co-op Combos!
- **3 Playable Spline-Based Levels**:
  1. **Sunny Slide** (Easy Tutorial): Gentle slopes, wide turns, introductory swinging bars and beans.
  2. **Spiky Canyon** (Medium Difficulty): Narrow canyon canals, rotating spiked logs, moving gates, and fiery chili pickups.
  3. **Turbo Rapids** (Hard Difficulty): Steep drops, high-speed zig-zags, speed boost pads, and complex hazard gauntlets.
- **Interactive Hazards & Pickups**:
  - **Hazards**: Pink swinging spiked bars, rotating spiked timber logs, sliding hydraulic gates. Collisions knock players off for a hilarious 2-second "WIPED OUT" sequence before checkpoint respawn.
  - **Pickups**: 
    - 🫘 **Beans**: +35 Fart meter points.
    - 🌶️ **Chili Pepper**: +60 Fart meter points and fiery Supercharge mode (2x power).
    - 🥤 **Soda Can**: +100 Fart meter points (instant full recharge) and speed boost.
- **Touch-Optimized Android Controls**:
  - Ergonomic landscape-only HUD.
  - Floating virtual joystick on the left.
  - Large green FART button (min 72px touch target) on the right with radial charge indicator.
  - Dedicated Jump button.
- **Dynamic Cartoon Audio Engine**:
  - Character-specific comical toot sounds (Hippo squish, Rhino bass rumble, Frog bubble ribbit, Capybara balloon whistle).
  - Looping tropical calypso theme music and cartoon sound effects.
  - Built-in `ProceduralAudioSynthesizer` C# fallback guaranteeing the game never goes silent.
- **Online Multiplayer Architecture**:
  - Photon PUN 2 compatible networking architecture.
  - 4-letter room code generation, player slot lobbies, raft state interpolation, and fart RPC replication with seamless offline single-player fallback.

---

## 🛠️ Technology Stack

| Component | Specification |
| :--- | :--- |
| **Game Engine** | Unity 2022.3 LTS (`2022.3.48f1`) |
| **Render Pipeline** | Universal Render Pipeline (URP 14.0.11) |
| **Target Platform** | Android (API Level 24+ Min, Target API 34) |
| **Scripting Backend** | IL2CPP / ARM64 |
| **Orientation** | Landscape |
| **Architecture** | Component-based C# (`TootRaftRush.asmdef`), Object Pooling, Procedural Splines |
| **CI/CD** | GitHub Actions with GameCI `unity-builder@v4` & `unity-test-runner@v4` |

---

## 📁 Project Structure

```
Toot-Raft-Rush/
├── .github/workflows/
│   ├── android-build.yml       # GameCI automated Android APK build pipeline
│   └── tests.yml               # EditMode automated tests & integrity validation
├── Assets/
│   ├── Art/
│   │   ├── Characters/         # Hippo, Rhino, Frog, Capybara models & textures
│   │   ├── Environment/        # NatureKit FBX (trees, rocks) & WaterSlide assets
│   │   ├── Props/              # Inflatable Raft model & SpeedPad
│   │   ├── Obstacles/          # Swinging Pink Spiked Bar, Rotating Log, Moving Gate
│   │   ├── Pickups/            # Beans, Chili Pepper, Soda Can
│   │   ├── VFX/                # Green gas puff, water splash, stars, confetti
│   │   ├── Audio/              # 13 44.1kHz WAV sound effects and music
│   │   ├── Materials/          # Stylized URP Lit cartoon materials
│   │   └── UI/                 # Joystick, buttons, portraits, and meter sprites
│   ├── Prefabs/                # 14 core prefabs (Raft, Animals, Obstacles, Pickups)
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   ├── CharacterSelection.unity
│   │   ├── Lobby.unity
│   │   ├── SunnySlide.unity    # Level 1
│   │   ├── SpikyCanyon.unity   # Level 2
│   │   └── TurboRapids.unity   # Level 3
│   ├── Scripts/
│   │   ├── AI/                 # AICompanion.cs
│   │   ├── Audio/              # AudioManager.cs, ProceduralAudioSynthesizer.cs
│   │   ├── Core/               # GameManager.cs
│   │   ├── Gameplay/           # RaftController.cs, PlayerController.cs, FartSystem.cs, CheckpointManager.cs
│   │   ├── Multiplayer/        # NetworkManager.cs
│   │   ├── Obstacles/          # ObstacleSwing.cs, ObstacleRotate.cs, ObstacleMove.cs, HazardTrigger.cs
│   │   ├── Pickups/            # FoodPickup.cs, SpeedPad.cs
│   │   ├── UI/                 # UIManager.cs, MainMenuController.cs, CharacterSelectController.cs, LobbyController.cs, VirtualJoystick.cs, FartButton.cs, SettingsManager.cs
│   │   └── Utility/            # SplineTrackGenerator.cs, CameraController.cs, CharacterVisualController.cs
│   ├── Settings/               # URP Pipeline Asset & Renderer
│   └── Tests/EditMode/         # NUnit test suites (FartSystem, RaftPhysics, Checkpoints, Integrity)
├── Packages/manifest.json      # Unity package dependencies
├── ProjectSettings/            # Android configuration, EditorBuildSettings, TagManager, QualitySettings
├── README.md
├── README_APK.md
├── ASSET_CREDITS.md
└── MULTIPLAYER_SETUP.md
```

---

## 🚀 Getting Started (Unity Editor)

1. Open **Unity Hub** and select Unity **2022.3 LTS** (e.g. `2022.3.48f1`).
2. Add and open the folder `Toot-Raft-Rush/`.
3. In the Project window, navigate to `Assets/Scenes/MainMenu.unity` and press **Play**.
4. Use `WASD` or Arrow Keys to move on the raft deck, `Space` to Jump, and **Hold & Release `F`** to charge and fire directional farts!

---

## 📜 Development Status

- ✅ **Physics Engine**: Raft buoyant bobbing, directional fart impulses, torque steering, co-op combo multipliers.
- ✅ **Character Roster**: Complete 3D low-poly models, cartoon textures, and visual controllers for Hippo, Rhino, Frog, and Capybara.
- ✅ **AI Companions**: Fully implemented autonomous co-op companions with obstacle avoidance and propulsion logic.
- ✅ **Levels & Obstacles**: 3 complete levels (Sunny Slide, Spiky Canyon, Turbo Rapids) with procedural spline tracks, swinging pink spiked bars, rotating logs, moving gates, and food pickups.
- ✅ **Audio & Juice**: 13 WAV sound effects/music + procedural runtime audio synthesizer fallback.
- ✅ **Automated Build Pipeline**: GameCI Android APK build workflow with automated preflight validation.
