# 🐚 Pearl Diver: Bahrain's Treasure

> *A 3D underwater adventure rooted in Bahrain's UNESCO-recognised pearling heritage*

![Unity](https://img.shields.io/badge/Engine-Unity%206-black?style=flat-square&logo=unity)
![Platform](https://img.shields.io/badge/Platform-PC%20%7C%20WebGL-0af?style=flat-square)
![Language](https://img.shields.io/badge/Language-C%23-purple?style=flat-square&logo=csharp)
![Course](https://img.shields.io/badge/Course-IT8101%20Games%20Development-teal?style=flat-square)
![Group](https://img.shields.io/badge/Section%20S4-Group%204-coral?style=flat-square)

---

## 🌊 About the Game

**Pearl Diver: Bahrain's Treasure** is a 3D third-person collectathon built in Unity, inspired by Bahrain's historic pearling industry — a tradition recognised by UNESCO as an Intangible Cultural Heritage of Humanity.

You play as a traditional Bahraini pearl diver exploring five distinct underwater environments across the Arabian Gulf. Dive in, collect every hidden pearl, avoid hostile sea creatures, and surface before your air runs out.

---

## 👥 Team

| Name | Student ID | Level | Role |
|------|-----------|-------|------|
| Malak Almajed | 202300641 | Level 1 – Shallow Reef | 🗺️ Level Designer & Lead Developer |
| Fatema Ahmed | 202200688 | Level 2 – Coral Garden | 🎭 Character, Sound & Animation Developer |
| Zahra Almosawi | 202305120 | Level 3 – Deep Cave | 🖥️ UI, Camera, Reward Systems & HUD Developer |
| Mohamed Hasan | 202201470 | Level 4 – Treasure Cove | 🤖 Game Systems, AI & Publishing |

---

## 🎮 Gameplay

### Core Loop
```
🤿 Dive In → 🔍 Explore → 🫧 Collect Pearls → 🦀 Avoid Enemies
```

### ⌨️ Controls

| Action | Key |
|--------|-----|
| Move | `W` `A` `S` `D` |
| Look Around | 🖱️ Mouse |
| Swim Up | `Space` |
| Swim Down | `Left Ctrl` / `C` |
| Collect Pearl | Automatic on contact |
| Exit Level | `E` at exit point |
| Pause | `Esc` |

---

## 🗺️ Levels

| # | Level | Vibe | Difficulty | Est. Time |
|---|-------|------|-----------|-----------|
| 1 | 🏝️ **Shallow Reef** | Bright, open, turquoise | ⭐ Very Easy | 3–5 min |
| 2 | 🌸 **Coral Garden** | Dense, vibrant, colourful | ⭐⭐ Easy–Medium | 4–6 min |
| 3 | 🕳️ **Deep Cave** | Dark, claustrophobic, tense | ⭐⭐⭐ Hard | 6–8 min |
| 4 | 💛 **Treasure Cove** | Golden, epic, unforgiving | ⭐⭐⭐⭐ Very Hard | 7–10 min |

> 🕐 **Total playtime:** ~20–35 minutes for a full run

---

## ✨ Features

### 🔧 Core Systems
- 🏊 **Movement & Navigation** — Smooth 3D WASD/mouse controls with idle, swim, and hit animations
- 🤖 **AI & Spawning System** — Enemy FSM (Finite State Machine) with idle, patrol, and chase states
- 📊 **Progression & Win/Loss Logic** — Pearl counting, level timer, medal tracking, sequential unlocks
- 💨 **Health & Hazard System** — Depleting air meter + heart lives + 2-second invincibility frames
- 🖥️ **UI Systems** — Full HUD (hearts, air meter, timer, pearl counter) + all game screens

### 🌟 Custom Features
- 🎲 **Procedural Level Generation** *(Advanced)* — Seeded randomisation repositions coral, rocks, pearls, and enemies each run for endless replayability
- 💨 **Air Meter / Breath System** — Constantly depleting oxygen adds urgency to every dive
- 🛡️ **Invincibility Frames** — 2-second immunity window after taking damage prevents unfair consecutive hits
- 🥇 **Speedrun Medal System** — Bronze / Silver / Gold time thresholds per level
- 🌟 **Hidden Golden Pearls** — One secret per level grants an extra life for thorough explorers
- 🌊 **Underwater Post-Processing** — Caustic light ripples, depth-of-field blur, and vignette shading per level
- 🗂️ **Level Select Screen** — Tracks best times, pearl counts, and medals for every completed level

---

## 🎨 Art & Audio

### Visual Style
> Stylized realism inspired by **ABZU** — alive and luminous without being photorealistic

| Level | Palette |
|-------|---------|
| 🏝️ Shallow Reef | Bright turquoise & sandy gold |
| 🌸 Coral Garden | Magentas, oranges, deep greens |
| 🕳️ Deep Cave | Deep blues, blacks, bioluminescent accents |
| 💛 Treasure Cove | Gold ambient light with deep ocean blue |

### 🎵 Music & Sound
- 🎼 Ambient electronic soundtrack that shifts dynamically with gameplay state
- 🔊 Distinct audio cues for pearl collection, air warnings, damage, and level completion

---

## 🦈 Enemies

| Enemy | Behaviour |
|-------|-----------|
| 🪼 **Jellyfish** | Slow vertical drift — deceptively peaceful |
| 🦀 **Crab** | Side-to-side scuttle, reverses at terrain |
| ⚡ **Electric Eel** | Slow curving movement, glows blue in the dark |

---

## 🏗️ Tech Stack

| Tool | Usage |
|------|-------|
| 🎮 Unity 6 (URP) | Game engine & rendering pipeline |
| 💻 C# / Visual Studio 2022 | Scripting & logic |
| 🎨 Blender | 3D model cleanup & refinement |
| 🤖 AI Modelling Tools | Asset generation (with manual cleanup) |
| 🎬 Unity Animator Controller | Character & enemy animations |
| 🔊 Unity Audio Mixer | Sound processing & balancing |
| 🗂️ GitHub | Version control |
| 📋 Trello | Sprint task tracking (SCRUM) |

---

## 🚀 Platforms

| Platform | Build | Notes |
|----------|-------|-------|
| 🖥️ Windows | `.exe` | Stable, performance-optimised |
| 🌐 WebGL | Browser | No install required — runs in any modern browser |

---

## 💡 Inspirations

- 🌊 **ABZU (2016)** — Visual tone; the calm-but-alive quality of underwater worlds
- 🐠 **Subnautica** — Audio/visual depth cues and ocean exploration as core gameplay
- 🏛️ **Bahrain's Pearling Path (UNESCO)** — Cultural backbone: narrative, character, and environments
- 🎮 **Crash Bandicoot** — Collectathon format, lives system, and score-chasing replayability

---

<p align="center">
  Made with 🤿 in Bahrain &nbsp;•&nbsp; IT8101 Games Development &nbsp;•&nbsp; Section S4 – Group 4
</p>
