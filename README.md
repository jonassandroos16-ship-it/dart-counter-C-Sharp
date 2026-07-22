# Dart Wars (C# Port)

A Blazor WebAssembly port of the Dart Wars dart counter app.

## Features

- **9 Game Modes**: 501, 301, 701, 101, Around the Clock, Practice, Killer, Speed 101, High Score, Battle
- **Power-Ups**: 13 power-ups with charge system (Surge, Cripple, Steal, Freeze, Shield, etc.)
- **XP & Levels**: Gain XP from matches, level up, earn power-up and attribute points
- **Titles**: 70+ unlockable titles based on game and lifetime achievements
- **Badges**: 17 unlockable badges
- **Team Mode**: Team-based matches with team assignment
- **Showdown**: Pre-match VS intro screen with player cards
- **Welcome Overlay**: Animated intro screen
- **Popups**: Score milestones, level-ups, title unlocks, kill notifications
- **Stats**: Comprehensive player statistics with visit averages, 180s, checkouts
- **History**: Match history with detailed visit breakdowns
- **Settings**: Theme, accent color, sound, XP config, power-up scaling, cloud sync
- **Cloud Sync**: Supabase backend for data persistence

## Tech Stack

- Blazor WebAssembly (.NET 8)
- Supabase (PostgreSQL + REST API)
- Web Audio API for sound synthesis

## Build

```bash
dotnet publish -c Release -o publish
```

## Deploy

GitHub Pages via GitHub Actions workflow.
