# QuickBuilds — Blish HUD Module

[![Blish HUD Module](https://img.shields.io/badge/Blish%20HUD-Module-blue?logo=guildwars2&logoColor=white)](https://blishhud.com/)
[![Target Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A lightweight, fast, and easy-to-use build template manager for **Guild Wars 2** running inside the [Blish HUD](https://blishhud.com/) overlay. Organize unlimited builds categorized by profession and game mode, preview gear notes, and copy chat codes with a single click.

---

## Why QuickBuilds?

In-game build storage is restricted to 24 account-wide slots and costs 500 gems per 3 slots. **QuickBuilds** removes that restriction by giving you unlimited, locally stored build templates in a lightweight, responsive overlay interface that opens in 0ms with zero game stutter.

---

## Features

- **Categorized by Profession**: Dedicated tabs for all 9 professions (*Necromancer, Guardian, Warrior, Engineer, Ranger, Thief, Elementalist, Mesmer, Revenant*) plus an "All" view.
- **Smart Active Character Detection**: Uses MumbleLink to automatically identify which character and profession you are currently playing and highlights that profession tab when you open the window.
- **Game Mode Filters**: Filter your builds instantly by *Open World*, *Raid*, *Strike*, *Fractal*, *WvW*, or *PvP*.
- **One-Click Copy**: A clean "Copy" button right next to each build. Clicking it immediately copies the `[&DQY...]` chat code to your clipboard and temporarily flashes "Copied!" for 2 seconds.
- **Build Inspector & Gear Notes**: Displays the complete chat code, elite specialization, category, and gear recommendations (weapons, stats, runes, relics, and rotation tips).
- **Simple Build Editor**: Click "+ New Build" to paste a chat code, enter a title, select its profession and category, and write gear notes.
- **Persistent Local JSON Storage**: All your builds are saved in an easily portable `builds.json` file located in `%USERPROFILE%\Documents\Guild Wars 2\addons\blishhud\quick-builds\builds.json`.
- **Corner Icon & Hotkey Support**: Quickly toggle the window via the Blish HUD top menu icon or assign a custom keyboard shortcut in the module settings.

---

## Downloads & Releases

Pre-compiled and signed `.bhm` module packages are published on the GitHub Releases page:

👉 **[Download Latest QuickBuilds.bhm Release](https://github.com/Mahargip/quickbuilds-gw2/releases)**

---

## How to Install

### Method 1: Manual Installation (Recommended)
1. Download the latest `QuickBuilds.bhm` file from the [Releases](https://github.com/Mahargip/quickbuilds-gw2/releases) page.
2. Open Windows Run (`Win + R`), paste the following path, and hit **Enter**:
   ```
   %USERPROFILE%\Documents\Guild Wars 2\addons\blishhud\modules\
   ```
3. Drop the `QuickBuilds.bhm` file directly into this folder.
4. Launch **Guild Wars 2** and **Blish HUD** (or restart Blish HUD if it is already open).
5. Click the Blish HUD helmet icon (top-left) $\rightarrow$ **Manage Modules**.
6. Find **QuickBuilds** in the list and click **Enable Module**.
7. Click the new QuickBuilds icon in the top corner bar to open the manager!

### Method 2: Development / From Source
If you are compiling from source code using Visual Studio or `dotnet build`, the custom MSBuild target automatically packages and deploys the `.bhm` directly to your local Blish HUD directory upon every build.

---

## How It Works in Game

1. Find the build you want to play in **QuickBuilds**.
2. Click **Copy** next to the build name.
3. Open your in-game hero panel (`H`) $\rightarrow$ **Build** tab.
4. Press `Ctrl + V` (or right-click your build tab and select Paste). The traits and skills will load instantly!

---

## Building from Source

### Prerequisites
- Visual Studio 2022 (with the **.NET desktop development** workload)
- .NET Framework 4.7.2 or 4.8 Targeting Pack
- .NET SDK 8.0+

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/Mahargip/quickbuilds-gw2.git
   cd quickbuilds-gw2
   ```
2. Open `QuickBuilds.sln` in Visual Studio 2022 or build via the command line:
   ```bash
   dotnet build
   ```
3. The custom MSBuild build target will automatically compile the binaries, package `QuickBuilds.bhm`, and deploy it directly to your local Blish HUD modules directory for instant testing.

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
