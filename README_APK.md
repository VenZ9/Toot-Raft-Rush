# Toot Raft Rush — Android Build & Deployment Guide 📱

This document provides instructions for compiling, building, downloading, and testing the Android APK for **Toot Raft Rush** using **GitHub Actions (GameCI)** and local Unity builds.

---

## ⚙️ Exact Android Build Configuration

The project settings have been pre-configured in `ProjectSettings/ProjectSettings.asset` and verified for modern Android hardware:

| Setting | Value |
| :--- | :--- |
| **Product Name** | `Toot Raft Rush` |
| **Package Name / Application Identifier** | `com.zayn.tootraftrush` |
| **Unity Version** | `2022.3.48f1` LTS |
| **Render Pipeline** | Universal Render Pipeline (URP 14.0.11) |
| **Minimum Android SDK** | `API Level 24` (Android 7.0 Nougat) |
| **Target Android SDK** | `API Level 34` (Android 14) |
| **Scripting Backend** | `IL2CPP` |
| **Target Architectures** | `ARM64` (`ARM64-v8a`) |
| **Screen Orientation** | `LandscapeLeft` (with auto-rotation between Landscape Left and Right) |
| **Graphics API** | Vulkan / OpenGLES3 |
| **Build System** | Gradle |
| **Texture Compression** | ASTC / ETC2 |
| **Touch Target Size** | Minimum 72px for all gameplay buttons and joystick |

---

## 🤖 Automated Builds via GitHub Actions

The repository includes an automated CI/CD pipeline at `.github/workflows/android-build.yml` powered by **GameCI `unity-builder@v4`**.

### How the Workflow Triggers
1. **Automatically**: On every `git push` to the `main` branch.
2. **Manually**: Via the GitHub Actions web interface (`workflow_dispatch`).

### How to Manually Trigger a Build
1. Open the repository on GitHub: `https://github.com/VenZ9/Toot-Raft-Rush`.
2. Click on the **Actions** tab at the top.
3. In the left sidebar, select **Build Android APK**.
4. Click the **Run workflow** dropdown button on the right, select the branch (`main`), and click **Run workflow**.

---

## 🔑 Unity Licensing Requirements (GameCI Activation)

Because Unity is commercial proprietary software, automated cloud builders (like GameCI in GitHub Actions) require valid Unity licensing credentials to run the Unity editor in headless mode on Ubuntu runners.

### Supported Activation Methods

#### Method A: Unity Personal License (.ulf file) — Recommended
1. Follow the official GameCI activation guide: https://game.ci/docs/github/activation
2. Obtain your manual activation file (`.ulf`) from [license.unity3d.com](https://license.unity3d.com).
3. In your GitHub repository:
   - Navigate to **Settings** -> **Secrets and variables** -> **Actions**.
   - Click **New repository secret**.
   - Name: `UNITY_LICENSE`
   - Value: Paste the complete XML contents of your `.ulf` file.

#### Method B: Username & Password with Serial
If using a Unity Plus, Pro, or Student subscription:
- `UNITY_EMAIL`: Your Unity account email address.
- `UNITY_PASSWORD`: Your Unity account password.
- `UNITY_SERIAL`: Your Unity serial key (e.g., `SC-XXXX-XXXX-XXXX-XXXX-XXXX`).

> ⚠️ **Important Security Rule**: Never commit Unity license files, passwords, or serials into the Git repository. Always keep them in GitHub Secrets.

---

## 📥 How to Download the APK Artifact

Once the GitHub Actions workflow finishes building:
1. Go to the **Actions** tab of the repository.
2. Click on the latest completed workflow run (**Build Android APK**).
3. Scroll down to the **Artifacts** section at the bottom of the summary page.
4. Click on **`TootRaftRush-Android-APK`** to download the ZIP file containing `TootRaftRush.apk`.
5. Artifacts are automatically retained for **7 days**.

---

## 📲 How to Install and Test the APK on Android

### Method 1: Direct USB Installation via ADB
1. Connect your Android device to your computer with USB debugging enabled.
2. Run:
   ```bash
   adb install -r TootRaftRush.apk
   ```
3. Launch the app directly from your app drawer.

### Method 2: Direct Device Download
1. Send or download the `TootRaftRush.apk` file directly to your Android device (via browser, Google Drive, or USB file transfer).
2. Tap the APK file in your device's file manager.
3. If prompted, allow "Install from Unknown Sources".
4. Tap **Install** and then **Open**.

---

## 🧪 Local Testing via Unity Editor

You can also run and test the complete game inside the Unity Editor:
1. Open the project in Unity **2022.3 LTS**.
2. Open `Assets/Scenes/MainMenu.unity`.
3. In the **Game** view, select a **16:9 Landscape** resolution (e.g. `1920x1080` or `1280x720`).
4. Press **Play**:
   - `W`, `A`, `S`, `D` or **Arrow Keys**: Move character on raft deck.
   - `Space`: Jump.
   - **Hold & Release `F`**: Charge and fire directional fart propulsion!
   - Touch input simulation is supported in the Unity Game view.

---

## 🔧 Troubleshooting

| Issue | Cause | Resolution |
| :--- | :--- | :--- |
| **"UNITY_LICENSE secret is not yet configured" warning in CI** | The GitHub repository has not yet had Unity credentials added to Secrets. | Follow the Unity Licensing section above to add `UNITY_LICENSE` in Repo Settings. The preflight integrity test will still pass. |
| **App fails to install on device** | Target device is 32-bit only (`armeabi-v7a`). | The project is built for 64-bit (`ARM64-v8a`) as required by modern Google Play guidelines (Android 14+). Ensure your device supports ARM64. |
| **Camera shake or particle lag on low-end device** | High quality shadow cascades. | Open the in-game **Settings** menu and switch Graphics Quality to **Fast**. |
| **No audio heard on device** | Device media volume is muted or physical mute switch is on. | Check device volume; Toot Raft Rush also includes a dynamic procedural synthesizer that runs in software if device hardware decoders stall. |
