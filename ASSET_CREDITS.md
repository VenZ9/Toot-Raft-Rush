# Toot Raft Rush — Asset Credits & License Attribution 🎨

All external and procedural assets used in **Toot Raft Rush** are commercially usable, public domain (CC0), or custom-created for this game. No copyrighted, extracted, or unauthorized game assets were used.

---

## 🐾 Characters

### 1. Blue Hippo
- **Source**: Gobkit Free Animal Pack (Alsomind Tech Co., Ltd.)
- **Download URL**: https://gobkit.itch.io/gobkit-free-animal-pack / https://gobkit.com/freebies
- **License**: CC0 1.0 Universal (Public Domain Dedication)
- **Modifications**: Recicolored with bright cartoon cyan/blue palette (`Texture_Hippo_Blue.png`) with pink snout and inner ears; adapted bone animations for idle, waddle walk, and fart thrust.

### 2. Grey Rhino
- **Source**: Gobkit Free Animal Pack (Alsomind Tech Co., Ltd.)
- **Download URL**: https://gobkit.itch.io/gobkit-free-animal-pack / https://gobkit.com/freebies
- **License**: CC0 1.0 Universal (Public Domain Dedication)
- **Modifications**: Recicolored with cool safari grey palette (`Texture_Rhino_Grey.png`) with ivory horn; integrated into the 4-player raft seating system.

### 3. Green Frog
- **Source**: Custom procedural stylized low-poly model inspired by Quaternius CC0 animal aesthetic
- **Download URL**: Built directly in project repository (`Assets/Art/Characters/Frog/Frog.obj`)
- **License**: CC0 1.0 Universal (Public Domain Dedication)
- **Description**: Features wide smiling mouth, yellow belly, crouching hind legs, and two bulging spherical frog eyes on top of the head.

### 4. Orange Capybara
- **Source**: Custom procedural stylized low-poly model inspired by Quaternius CC0 animal aesthetic
- **Download URL**: Built directly in project repository (`Assets/Art/Characters/Capybara/Capybara.obj`)
- **License**: CC0 1.0 Universal (Public Domain Dedication)
- **Description**: Features plump loaf-shaped body, blunt square snout with nostrils, rounded ears, and calm cartoon facial features.

---

## 🌴 Environment & Props

### Nature Kit (Trees, Rocks, Vegetation)
- **Source**: Kenney Nature Kit
- **Original Author**: Kenney (Asset Jesus)
- **Download URL**: https://kenney.nl/assets/nature-kit
- **License**: CC0 1.0 Universal (Public Domain Dedication)
- **Modifications**: Integrated `natureKit.fbx` with stylized URP shaders and collision meshes.

### Inflatable Raft (Circular Torus with Green Deck & Orange Rim)
- **Source**: Custom procedural 3D model (`Assets/Art/Props/Raft/Raft.obj`)
- **License**: CC0 1.0 Universal (Public Domain Dedication)
- **Description**: Circular inflatable raft featuring an orange outer buoyant torus (R=1.65m), recessed green non-slip circular floor deck (R=1.45m), 4 passenger seating positions, and outer safety grab cords.

### Winding Water Slide Track
- **Source**: Procedural spline generator (`Assets/Scripts/Utility/SplineTrackGenerator.cs`) & segment mesh (`SlideSegment.obj`)
- **License**: CC0 1.0 Universal (Public Domain Dedication)
- **Description**: Parabolic U-channel flume with safety rails (orange & green) and UV scrolling water texture.

### Obstacles (Pink Swinging Spiked Bar, Rotating Log, Moving Gate)
- **Source**: Custom procedural 3D models (`Assets/Art/Obstacles/`)
- **License**: CC0 1.0 Universal (Public Domain Dedication)
- **Modifications**: Modeled with conical spikes, wooden log textures, and warning hazard stripes.

### Food Pickups (Beans, Chili Pepper, Soda Can)
- **Source**: Custom procedural 3D models (`Assets/Art/Pickups/`)
- **License**: CC0 1.0 Universal (Public Domain Dedication)
- **Description**: Stylized cartoon kidney bean with green leaf sprout, curved hot red chili pepper, and aluminum soda can with pull tab.

---

## 🎵 Audio & SFX

### Sound Effects & Theme Music
- **Source**: Custom synthesized 44.1kHz 16-bit PCM WAV audio clips (`Assets/Art/Audio/`)
- **License**: CC0 1.0 Universal (Public Domain Dedication)
- **Clips**:
  - `toot_hippo.wav`: Resonant wet squish fart with frequency modulation.
  - `toot_rhino.wav`: Heavy bass rumble power toot.
  - `toot_frog.wav`: Bubbly high-pitched ribbit toot.
  - `toot_capybara.wav`: Squeaky balloon whistling toot.
  - `fart_charge.wav`: Rising pitch gas buildup.
  - `splash.wav`: Water wake and droplet splash.
  - `pickup_bean.wav`, `pickup_chili.wav`, `pickup_soda.wav`: Musical collection chimes and sizzle.
  - `wipeout.wav`: Cartoon slide whistle + bonk.
  - `checkpoint.wav`: C6-E6-G6 arpeggio chime.
  - `victory.wav`: Triumphant celebratory fanfare.
  - `music_theme.wav`: 120 BPM tropical calypso steelpan & marimba loop.
- **Runtime Procedural Fallback**: `ProceduralAudioSynthesizer.cs` synthesizes mathematical audio directly at runtime using Unity's `AudioClip.Create` if external audio files fail to load.

---

## 🖼️ User Interface & VFX

### Sprites & UI Graphics
- **Source**: Custom generated high-contrast pixel-perfect cartoon sprites (`Assets/Art/UI/` and `Assets/Art/VFX/`)
- **License**: CC0 1.0 Universal (Public Domain Dedication)
- **Items**: Virtual joystick base and knob, large green fart button with gas puff icon, jump button, animal portrait icons, fart meter bar fills, and particle textures (puffy green gas cloud, water droplets, confetti stars).
